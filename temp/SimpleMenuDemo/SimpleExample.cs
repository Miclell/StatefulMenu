using StatefulMenu.Commands.Interfaces;
using StatefulMenu.Core.Models;

namespace SimpleMenuDemo;

/// <summary>
/// Простой пример использования StatefulMenu без сложной архитектуры команд.
/// Этот подход более понятный и прямолинейный.
/// </summary>
public class SimpleExampleProvider : IMenuProvider
{
    public Task<MenuState> CreateMenuAsync(CancellationToken ct = default)
    {
        var items = new[]
        {
            // Простое действие
            new MenuItem("Сказать привет", async _ =>
            {
                Console.WriteLine();
                Console.WriteLine("Привет! Это простое действие.");
                Console.WriteLine("Нажмите любую клавишу...");
                Console.ReadKey(true);
                return MenuResult.None(); // Остаемся на том же экране
            }, hotkey: ConsoleKey.H), // Хоткей H

            // Открытие подменю
            new MenuItem("Открыть подменю", async _ =>
            {
                var submenu = new MenuState("Подменю", new[]
                {
                    new MenuItem("Действие 1", async _ =>
                    {
                        Console.WriteLine("Выполняем действие 1");
                        Console.ReadKey(true);
                        return MenuResult.None();
                    }),
                    new MenuItem("Действие 2", async _ =>
                    {
                        Console.WriteLine("Выполняем действие 2");
                        Console.ReadKey(true);
                        return MenuResult.None();
                    }),
                    new MenuItem("Назад", _ => Task.FromResult(MenuResult.Pop()), hotkey: ConsoleKey.Escape)
                });
                return MenuResult.Push(submenu); // Открываем новое меню
            }),

            // Динамическое меню с данными
            new MenuItem("Список животных", async _ =>
            {
                var animals = new[] { "Кот", "Собака", "Птица", "Рыба" };
                var animalItems = animals
                    .Select(animal => new MenuItem($"Выбрать {animal}", async _ =>
                    {
                        Console.WriteLine($"Вы выбрали: {animal}");
                        Console.ReadKey(true);
                        return MenuResult.None();
                    }))
                    .Append(new MenuItem("Назад", _ => Task.FromResult(MenuResult.Pop())))
                    .ToArray();

                var animalMenu = new MenuState("Выберите животное", animalItems);
                return MenuResult.Push(animalMenu);
            }),

            // Выход
            new MenuItem("Выход", _ => Task.FromResult(MenuResult.Exit()), hotkey: ConsoleKey.E)
        };

        return Task.FromResult(new MenuState("Простое главное меню", items));
    }
}
