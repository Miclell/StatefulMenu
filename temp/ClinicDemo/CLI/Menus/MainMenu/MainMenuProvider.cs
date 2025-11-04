using ClinicDemo.CLI.Menus.PatientMenu;
using Microsoft.Extensions.DependencyInjection;
using StatefulMenu.Commands.BuiltIn;
using StatefulMenu.Commands.Interfaces;
using StatefulMenu.Core.Models;

namespace ClinicDemo.CLI.Menus.MainMenu;

public class MainMenuProvider(IServiceProvider serviceProvider) : IMenuProvider
{
    public Task<MenuState> CreateMenuAsync(CancellationToken cancellationToken = default)
    {
        var commands = new IMenuCommand[]
        {
            serviceProvider.GetRequiredService<ShowPatientMenuCommand>(),
            serviceProvider.GetRequiredService<ExitCommand>()
        };

        var items = commands
            .Select(c => new MenuItem(c.Title, _ => c.ExecuteAsync(cancellationToken)))
            //.Append(MenuItem.Exit(isZeroIndex: true))
            .ToList();

        var header = new MenuHeaderOptions
        {
            Separator = " || ",
            Segments = new List<Func<string>>
            {
                () => "CLI Demo",
                () => DateTime.Now.ToString("HH:mm:ss")
            }
        };

        return Task.FromResult(new MenuState("Главное меню", items, header: header));
    }
}