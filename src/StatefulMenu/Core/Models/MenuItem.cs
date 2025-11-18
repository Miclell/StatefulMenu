using System;
using System.Threading;
using System.Threading.Tasks;

namespace StatefulMenu.Core.Models;

public sealed class MenuItem
{
    public MenuItem(string title, Func<CancellationToken, Task<MenuResult>> action, ConsoleKey? hotkey = null, bool isZeroIndex = false, bool isHidden = false)
    {
        Title = title;
        Action = action;
        Hotkey = hotkey;
        IsZeroIndex = isZeroIndex;
        IsHidden = isHidden;
    }

    public string Title { get; }
    public Func<CancellationToken, Task<MenuResult>> Action { get; }
    public ConsoleKey? Hotkey { get; }
    public bool IsZeroIndex { get; }
    public bool IsHidden { get; }

    public static MenuItem Back(string? title = null, ConsoleKey? hotkey = null)
    {
        return new MenuItem(title ?? "Назад", _ => Task.FromResult<MenuResult>(MenuResult.Pop()), hotkey, isZeroIndex: true);
    }

    public static MenuItem Exit(string? title = null, ConsoleKey? hotkey = null)
    {
        return new MenuItem(title ?? "Выход", _ => Task.FromResult<MenuResult>(MenuResult.Exit()), hotkey, isZeroIndex: true);
    }

    public static MenuItem Hidden(string title, Func<CancellationToken, Task<MenuResult>> action, ConsoleKey? hotkey = null, bool isZeroIndex = false)
    {
        return new MenuItem(title, action, hotkey, isZeroIndex: isZeroIndex, isHidden: true);
    }
}