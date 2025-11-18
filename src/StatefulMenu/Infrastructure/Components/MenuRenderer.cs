using StatefulMenu.Core.Models;

namespace StatefulMenu.Infrastructure.Components;

public class MenuRenderer
{
    public void Render(MenuState state, int selectedIndex = 0)
    {
        if (!Console.IsOutputRedirected && Console.CursorVisible)
        {
            try
            {
                Console.Clear();
            }
            catch (IOException)
            {
                // Ignoring
            }
        }
        
        if (state.Header is { } header)
        {
            var segs = new List<string>();
            if (!string.IsNullOrWhiteSpace(state.Title)) segs.Add(state.Title!);
            if (header.Segments.Count > 0) segs.AddRange(header.Segments.Select(SafeInvoke));
            var headerLine = string.Join(header.Separator, segs);
            Console.WriteLine(headerLine);
            var width = GetSafeWindowWidth();
            Console.WriteLine(new string('-', Math.Min(width, Math.Max(0, headerLine.Length))));
        }
        else
        {
            Console.WriteLine($"=== {state.Title} ===");
            Console.WriteLine();
        }

        var zeroItem = state.Items.FirstOrDefault(MenuItemUtilities.IsZero);
        var regularItems = state.Items.Where(x => !MenuItemUtilities.IsZero(x) && !x.IsHidden).ToList();

        var headerLines = state.Header is {Segments.Count: > 0} ? 2 : 0; // header + underline
        var footerLines = zeroItem != null ? 2 : 1; // zero + hint
        var windowHeight = GetSafeWindowHeight();
        var available = Math.Max(3, windowHeight - (3 + headerLines + footerLines));
        var selForRegular = Math.Min(selectedIndex, Math.Max(0, regularItems.Count - 1));
        var start = Math.Max(0, Math.Min(selForRegular - available / 2, Math.Max(0, regularItems.Count - available)));
        var end = Math.Min(regularItems.Count, start + available);

        for (var i = start; i < end; i++)
        {
            var item = regularItems[i];
            var prefix = i == selectedIndex ? "> " : "  ";
            var hotkey = item.Hotkey.HasValue ? $"[{item.Hotkey}] " : string.Empty;
            Console.WriteLine($"{prefix}{i + 1}. {hotkey}{item.Title}");
        }

        // Indicators if truncated
        if (start > 0) Console.WriteLine("  ...");
        if (end < regularItems.Count) Console.WriteLine("  ...");

        if (zeroItem != null)
        {
            var zeroSelected = selectedIndex == regularItems.Count;
            var prefix = zeroSelected ? "> " : "  ";
            var hotkey = zeroItem.Hotkey.HasValue ? $"[{zeroItem.Hotkey}] " : string.Empty;
            Console.WriteLine($"{prefix}0. {hotkey}{zeroItem.Title}");
        }

        Console.WriteLine();
        Console.Write("Enter/цифра — выбрать, ↑/↓ — навигация, Esc — назад");
    }

    private static string SafeInvoke(Func<string> f)
    {
        try { return f(); }
        catch { return string.Empty; }
    }

    private static int GetSafeWindowWidth()
    {
        try { return Console.WindowWidth; } catch { return 80; }
    }

    private static int GetSafeWindowHeight()
    {
        try { return Console.WindowHeight; } catch { return 25; }
    }
}