using Application.DTOs.Patient;
using ClinicDemo.CLI.Menus.PatientMenu.ShowPatientsFlow.Commands;
using Microsoft.Extensions.DependencyInjection;
using StatefulMenu.Commands.BuiltIn;
using StatefulMenu.Commands.Interfaces;
using StatefulMenu.Core.Interfaces;
using StatefulMenu.Core.Models;

namespace ClinicDemo.CLI.Menus.PatientMenu.ShowPatientsFlow.Providers;

public class PatientSelectionProvider(IServiceProvider serviceProvider) : IMenuProvider
{
    public Task<MenuState> CreateMenuAsync(CancellationToken cancellationToken = default)
    {
        var commands = new IMenuCommand[]
        {
            serviceProvider.GetRequiredService<DeletePatientCommand>(),
            serviceProvider.GetRequiredService<BackCommand>()
        };

        var items = commands
            .Select(c => new MenuItem(c.Title, _ => c.ExecuteAsync(cancellationToken)))
            .Append(MenuItem.Back())
            .ToList();

        var dataService = serviceProvider.GetRequiredService<IDataService>();
        dataService.TryGet<BasePatientProfileDto>(nameof(BasePatientProfileDto), out var patient);

        var title = patient is null
            ? "Действия с пациентом"
            : $"Действия для пациента {patient.LpuShortName} | {patient.PatientFirstName} {patient.PatientLastName}";

        return Task.FromResult(new MenuState(title, items));
    }
}