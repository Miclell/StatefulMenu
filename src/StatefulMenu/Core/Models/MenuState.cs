namespace StatefulMenu.Core.Models;

public class MenuState
{
    public MenuState(string? title, IEnumerable<MenuItem> items, object? snapshot = null, MenuHeaderOptions? header = null)
    {
        Title = title;
        Items = items is IReadOnlyList<MenuItem> list ? list : new List<MenuItem>(items);
        Snapshot = snapshot;
        Header = header;
    }

    public string? Title { get; }
    public IReadOnlyList<MenuItem> Items { get; }
    public object? Snapshot { get; }
    public MenuHeaderOptions? Header { get; }
}