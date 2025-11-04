using ClinicDemo.CLI.Menus.PatientMenu.CreatePatientFlow.Commands;
using ClinicDemo.CLI.Menus.PatientMenu.ShowPatientsFlow.Commands;
using Microsoft.Extensions.DependencyInjection;
using StatefulMenu.Commands.BuiltIn;
using StatefulMenu.Commands.Interfaces;
using StatefulMenu.Core.Models;

namespace ClinicDemo.CLI.Menus.PatientMenu;

public class ShowPatientMenuProvider(IServiceProvider serviceProvider) : IMenuProvider
{
    public Task<MenuState> CreateMenuAsync(CancellationToken cancellationToken = default)
    {
        var commands = new IMenuCommand[]
        {
            serviceProvider.GetRequiredService<RunCreatePatientFlowCommand>(),
            serviceProvider.GetRequiredService<RunShowPatientsFlowCommand>(),
            serviceProvider.GetRequiredService<BackCommand>()
        };

        var items = commands
            .Select(c => new MenuItem(c.Title, _ => c.ExecuteAsync(cancellationToken)))
            .Append(MenuItem.Back())
            .ToList();

        var header = new MenuHeaderOptions
        {
            Separator = " | ",
            Segments = new List<Func<string>>
            {
                () => "Пациенты",
                () => $"Всего команд: {items.Count - 1}"
            }
        };

        return Task.FromResult(new MenuState("Меню пациента", items, header: header));
    }
}