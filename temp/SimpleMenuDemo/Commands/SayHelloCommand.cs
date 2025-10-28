using StatefulMenu.Commands.Interfaces;
using StatefulMenu.Core.Models;

namespace SimpleMenuDemo.Commands;

public class SayHelloCommand : IMenuCommand
{
    public string Title => "Сказать привет";

    public Task<MenuResult> ExecuteAsync(CancellationToken ct = default)
    {
        Console.WriteLine();
        Console.WriteLine("Привет из команды!");
        Console.WriteLine("Нажмите любую клавишу...");
        Console.ReadKey(true);
        return Task.FromResult(MenuResult.None());
    }
}


