using Microsoft.Extensions.DependencyInjection;
using SimpleMenuDemo;
using StatefulMenu;
using StatefulMenu.Core.Interfaces;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("Выберите пример:");
Console.WriteLine("1. Простой пример (рекомендуется)");
Console.WriteLine("2. Сложный пример с командами");
Console.Write("Введите номер: ");

var choice = Console.ReadLine();
Console.WriteLine();

if (choice == "1")
{
    await SimpleProgram.RunAsync();
}
else
{
    // Сложный пример с командами
    var services = new ServiceCollection()
        .AddStatefulMenu(typeof(SimpleMenuDemo.Providers.HomeMenuProvider).Assembly);

    var provider = services.BuildServiceProvider();
    var nav = provider.GetRequiredService<INavigationService>();
    var root = provider.GetRequiredService<StatefulMenu.Commands.Interfaces.IMenuProvider>();

    await nav.RunAsync(root);
}
