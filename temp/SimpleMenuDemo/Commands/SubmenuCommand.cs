using StatefulMenu.Commands.Interfaces;
using StatefulMenu.Core.Models;

namespace SimpleMenuDemo.Commands;

public class SubmenuCommand : IMenuCommand
{
    public string Title => "Подменю";

    public Task<MenuResult> ExecuteAsync(CancellationToken ct = default)
    {
        var sub = new MenuState("Подменю", new List<MenuItem>
        {
            new("Назад", _ => Task.FromResult(MenuResult.Pop()))
        });
        return Task.FromResult(MenuResult.Push(sub));
    }
}


