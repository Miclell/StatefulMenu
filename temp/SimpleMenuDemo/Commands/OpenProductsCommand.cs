using StatefulMenu.Commands.Interfaces;
using StatefulMenu.Core.Models;

namespace SimpleMenuDemo.Commands;

/// <summary>
/// Динамический список товаров с переходом в подменю и возвратом назад.
/// </summary>
public class OpenProductsCommand : IMenuCommand
{
    public string Title => "Товары (динамический список)";

    public Task<MenuResult> ExecuteAsync(CancellationToken ct = default)
    {
        var products = new[]
        {
            new { Id = 1, Name = "Ноутбук" },
            new { Id = 2, Name = "Смартфон" },
            new { Id = 3, Name = "Наушники" }
        };

        var items = products
            .Select(p => new MenuItem(
                title: $"Открыть {p.Name}",
                action: _ =>
                {
                    Console.WriteLine($"Карточка: {p.Name} (Id={p.Id})");
                    Console.WriteLine("Нажмите клавишу...");
                    Console.ReadKey(true);
                    return Task.FromResult(MenuResult.None());
                }))
            .Append(new MenuItem("Назад", _ => Task.FromResult(MenuResult.Pop())))
            .ToList();

        var state = new MenuState("Товары", items);
        return Task.FromResult(MenuResult.Push(state));
    }
}


