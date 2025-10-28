using Microsoft.Extensions.DependencyInjection;
using StatefulMenu;
using StatefulMenu.Core.Interfaces;

namespace SimpleMenuDemo;

/// <summary>
/// Простая программа, демонстрирующая правильное использование StatefulMenu.
/// Этот подход намного проще, чем архитектура с отдельными командами.
/// </summary>
public static class SimpleProgram
{
    public static async Task RunAsync()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== Простой пример StatefulMenu ===");
        Console.WriteLine();

        // Настройка DI - регистрируем только наш простой провайдер
        var services = new ServiceCollection()
            .AddStatefulMenu(typeof(SimpleExampleProvider).Assembly);

        var provider = services.BuildServiceProvider();
        var nav = provider.GetRequiredService<INavigationService>();
        
        // Используем наш простой провайдер
        var root = provider.GetRequiredService<SimpleExampleProvider>();

        Console.WriteLine("Управление:");
        Console.WriteLine("- Цифры 1-9: выбрать пункт");
        Console.WriteLine("- Enter: выполнить выбранное действие");
        Console.WriteLine("- Стрелки ↑↓: навигация");
        Console.WriteLine("- Esc: назад");
        Console.WriteLine("- Хоткеи: H (привет), E (выход)");
        Console.WriteLine();
        Console.WriteLine("Нажмите любую клавишу для начала...");
        Console.ReadKey(true);

        await nav.RunAsync(root);
    }
}
