using Microsoft.Extensions.DependencyInjection;
using SimpleMenuDemo.Models;
using StatefulMenu.Commands.Interfaces;
using StatefulMenu.Core.Interfaces;
using StatefulMenu.Core.Models;

namespace SimpleMenuDemo.Commands;

/// <summary>
/// Показывает форму ввода пользователя (InputField) и сохраняет результат в IDataService.
/// Затем предлагает действия с введёнными данными.
/// </summary>
public class OpenUserFormCommand(IServiceProvider serviceProvider) : IMenuCommand
{
    public string Title => "Форма пользователя (ввод и хранение данных)";

    public async Task<MenuResult> ExecuteAsync(CancellationToken ct = default)
    {
        var input = serviceProvider.GetRequiredService<IConsoleInputService>();
        var data = serviceProvider.GetRequiredService<IDataService>();

        var form = await input.ReadModelAsync<UserForm>(ct);
        if (form is null)
        {
            return MenuResult.None();
        }

        data.Set("lastUser", form);

        var detailsItems = new List<MenuItem>
        {
            new("Показать введённые данные", _ =>
            {
                if (data.TryGet<UserForm>("lastUser", out var saved) && saved is not null)
                {
                    Console.WriteLine($"Имя: {saved.Name}");
                    Console.WriteLine($"Возраст: {saved.Age?.ToString() ?? "—"}");
                    Console.WriteLine($"Email: {saved.Email}");
                }
                Console.WriteLine("Нажмите клавишу...");
                Console.ReadKey(true);
                return Task.FromResult(MenuResult.None());
            }),
            new("Очистить и назад", _ =>
            {
                data.Remove("lastUser");
                return Task.FromResult(MenuResult.Pop());
            })
        };

        var detailsState = new MenuState("Пользователь — детали", detailsItems);
        return MenuResult.Push(detailsState);
    }
}


