using StatefulMenu.Commands.Interfaces;
using StatefulMenu.Core.Models;
using SimpleMenuDemo.Commands;

namespace SimpleMenuDemo.Providers;

public class HomeMenuProvider : IMenuProvider
{
    private readonly IEnumerable<IMenuCommand> _commands;

    public HomeMenuProvider(IEnumerable<IMenuCommand> commands)
    {
        _commands = commands;
    }

    public Task<MenuState> CreateMenuAsync(CancellationToken ct = default)
    {
        // Явно задаём порядок команд для демонстрации разных сценариев
        var ordered = _commands
            .OrderBy(c => c is SayHelloCommand ? 0 :
                           c is OpenProductsCommand ? 1 :
                           c is OpenUserFormCommand ? 2 :
                           c is ReplaceWithInfoCenterCommand ? 3 : 100)
            .ToList();

        var items = ordered
            .Select(cmd => new MenuItem(cmd.Title, _ => cmd.ExecuteAsync(ct)))
            .Append(new MenuItem("Назад", _ => Task.FromResult(MenuResult.Pop())))
            .ToList();

        return Task.FromResult(new MenuState("Главное меню (сложный пример)", items));
    }
}


