using StatefulMenu.Commands.Interfaces;
using StatefulMenu.Core.Interfaces;
using StatefulMenu.Core.Models;
using StatefulMenu.Infrastructure.Components;

namespace StatefulMenu.Infrastructure.Services;

public class NavigationService(NavigationStack stack, MenuRenderer renderer, IServiceProvider serviceProvider)
    : INavigationService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public event EventHandler<NavigatingEventArgs>? Navigating;
    public event EventHandler<NavigatedEventArgs>? Navigated;

    public void Push(MenuState state)
    {
        var from = stack.Count > 0 ? stack.Peek() : null;
        var args = new NavigatingEventArgs(from, state);
        Navigating?.Invoke(this, args);
        if (args.Cancel) return;
        stack.Push(state);
        Navigated?.Invoke(this, new NavigatedEventArgs(from, state));
    }

    public void Replace(MenuState state)
    {
        var from = stack.Count > 0 ? stack.Pop() : null;
        var args = new NavigatingEventArgs(from, state);
        Navigating?.Invoke(this, args);
        if (args.Cancel)
        {
            if (from != null) stack.Push(from);
            return;
        }

        stack.Push(state);
        Navigated?.Invoke(this, new NavigatedEventArgs(from, state));
    }

    public bool TryPop(int count = 1)
    {
        if (stack.Count < count) return false;
        var from = stack.Peek();
        for (var i = 0; i < count; i++) stack.Pop();
        var to = stack.Count > 0 ? stack.Peek() : null;
        Navigated?.Invoke(this, new NavigatedEventArgs(from, to));
        return true;
    }

    public MenuState? Peek()
    {
        return stack.Count > 0 ? stack.Peek() : null;
    }

    public int Count => stack.Count;

    public async Task RunAsync(IMenuProvider rootProvider, CancellationToken ct = default)
    {
        var initial = await rootProvider.CreateMenuAsync(ct);
        Push(initial);

        var selectedIndex = 0;
        var numberBuffer = string.Empty;

        while (Count > 0 && !ct.IsCancellationRequested)
        {
            var current = Peek()!;
            var zeroItem = current.Items.FirstOrDefault(MenuItemUtilities.IsZero);
            var regularItems = current.Items.Where(x => !MenuItemUtilities.IsZero(x) && !x.IsHidden).ToList();
            if (current.Items.Count == 0)
            {
                renderer.Render(current, selectedIndex);
                _ = Console.ReadKey(true);
                TryPop();
                continue;
            }

            var totalCount = regularItems.Count + (zeroItem != null ? 1 : 0);
            if (selectedIndex < 0) selectedIndex = 0;
            if (selectedIndex >= totalCount) selectedIndex = Math.Max(0, totalCount - 1);

            renderer.Render(current, selectedIndex);

            var keyInfo = Console.ReadKey(true);

            if (ct.IsCancellationRequested) break;

            if (HandleNavigationKey(keyInfo, totalCount, ref selectedIndex)) continue;

            if (TryResolveHotkey(current, keyInfo, out var hotkeyIndex))
            {
                var hkItem = current.Items[hotkeyIndex];
                if (hkItem.IsZeroIndex)
                {
                    var zRes = await hkItem.Action(ct);
                    await ApplyResultAsync(zRes, ct);
                }
                else
                {
                    var regIdx = regularItems.IndexOf(hkItem);
                    if (regIdx >= 0) selectedIndex = regIdx;
                    var hkResult = await hkItem.Action(ct);
                    await ApplyResultAsync(hkResult, ct);
                }
                continue;
            }

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                if (!string.IsNullOrEmpty(numberBuffer))
                {
                    if (int.TryParse(numberBuffer, out var parsedIndex))
                    {
                        if (parsedIndex == 0 && zeroItem != null)
                        {
                            var zr = await zeroItem.Action(ct);
                            await ApplyResultAsync(zr, ct);
                        }
                        else if (parsedIndex >= 1 && parsedIndex <= regularItems.Count)
                        {
                            selectedIndex = parsedIndex - 1;
                            var numItem = regularItems[selectedIndex];
                            var numResult = await numItem.Action(ct);
                            await ApplyResultAsync(numResult, ct);
                        }
                    }
                    numberBuffer = string.Empty;
                }
                else
                {
                    if (zeroItem != null && selectedIndex == regularItems.Count)
                    {
                        var zr = await zeroItem.Action(ct);
                        await ApplyResultAsync(zr, ct);
                    }
                    else
                    {
                        var item = regularItems[selectedIndex];
                        var result = await item.Action(ct);
                        await ApplyResultAsync(result, ct);
                    }
                }
                continue;
            }

            // Digit selection (multi-digit 0..N)
            if (char.IsDigit(keyInfo.KeyChar))
            {
                // Immediate action for 0 if zeroItem exists
                if (keyInfo.KeyChar == '0' && zeroItem != null)
                {
                    var zr = await zeroItem.Action(ct);
                    await ApplyResultAsync(zr, ct);
                    numberBuffer = string.Empty;
                    continue;
                }

                numberBuffer += keyInfo.KeyChar;
                if (int.TryParse(numberBuffer, out var parsedIndex))
                {
                    if (parsedIndex >= 1 && parsedIndex <= regularItems.Count)
                    {
                        selectedIndex = parsedIndex - 1;
                    }
                    else
                    {
                        // вне диапазона — сбрасываем буфер
                        numberBuffer = string.Empty;
                    }
                }
                continue;
            }

            // Escape -> Back
            if (keyInfo.Key == ConsoleKey.Escape)
            {
                numberBuffer = string.Empty;
                TryPop();
                continue;
            }

            // Любая прочая клавиша — очистка буфера чисел
            numberBuffer = string.Empty;
        }
    }

    private static bool HandleNavigationKey(ConsoleKeyInfo keyInfo, int itemsCount, ref int selectedIndex)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.UpArrow:
                selectedIndex = (selectedIndex - 1 + itemsCount) % itemsCount;
                return true;
            case ConsoleKey.DownArrow:
                selectedIndex = (selectedIndex + 1) % itemsCount;
                return true;
            case ConsoleKey.Home:
                selectedIndex = 0;
                return true;
            case ConsoleKey.End:
                selectedIndex = Math.Max(0, itemsCount - 1);
                return true;
            default:
                return false;
        }
    }

    private static bool TryResolveHotkey(MenuState state, ConsoleKeyInfo keyInfo, out int index)
    {
        for (var i = 0; i < state.Items.Count; i++)
        {
            var hk = state.Items[i].Hotkey;
            if (hk.HasValue && hk.Value == keyInfo.Key)
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }

    private Task ApplyResultAsync(MenuResult result, CancellationToken ct)
    {
        switch (result)
        {
            case NoneResult:
                return Task.CompletedTask;
            case ExitResult:
                while (TryPop())
                {
                }

                return Task.CompletedTask;
            case PopResult pop:
                TryPop(pop.Count);
                return Task.CompletedTask;
            case PushResult push:
                Push(push.State);
                return Task.CompletedTask;
            case ReplaceResult replace:
                Replace(replace.State);
                return Task.CompletedTask;
            default:
                return Task.CompletedTask;
        }
    }
}