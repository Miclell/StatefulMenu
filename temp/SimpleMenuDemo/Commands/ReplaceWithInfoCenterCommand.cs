using StatefulMenu.Commands.Interfaces;
using StatefulMenu.Core.Models;

namespace SimpleMenuDemo.Commands;

/// <summary>
/// Заменяет текущий экран информационным центром (Replace), откуда можно вернуться назад.
/// </summary>
public class ReplaceWithInfoCenterCommand : IMenuCommand
{
    public string Title => "Инфо-центр (Replace текущего экрана)";

    public Task<MenuResult> ExecuteAsync(CancellationToken ct = default)
    {
        var infoItems = new List<MenuItem>
        {
            new("О программе", _ =>
            {
                Console.WriteLine("Демо сложного меню на StatefulMenu");
                Console.WriteLine("Нажмите клавишу...");
                Console.ReadKey(true);
                return Task.FromResult(MenuResult.None());
            }),
            new("Назад", _ => Task.FromResult(MenuResult.Pop()))
        };

        var infoState = new MenuState("Инфо-центр", infoItems);
        return Task.FromResult(MenuResult.Replace(infoState));
    }
}


