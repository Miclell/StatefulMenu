using Application.Services.Interfaces;
using ClinicDemo.CLI.Menus.PatientMenu.ShowPatientsFlow.Commands;
using StatefulMenu.Commands.BuiltIn;
using StatefulMenu.Commands.Interfaces;
using StatefulMenu.Core.Models;

namespace ClinicDemo.CLI.Menus.PatientMenu.ShowPatientsFlow.Providers;

public class ShowPatientsProvider(
    IServiceProvider serviceProvider,
    IPatientService patientService,
    IAppSettingsService appSettings) : IMenuProvider
{
    public async Task<MenuState> CreateMenuAsync(CancellationToken cancellationToken = default)
    {
        var userId = await appSettings.GetDefaultUserIdAsync();
        var patients = await patientService.GetByUser(userId, cancellationToken);

        var commands = patients
            .Select(p => new PatientSelectionCommand(p, serviceProvider))
            .Cast<IMenuCommand>()
            .Append(new BackCommand())
            .ToList();

        var items = commands
            .Select(c => new MenuItem(c.Title, _ => c.ExecuteAsync(cancellationToken)))
            .Append(MenuItem.Back())
            .ToList();

        var header = new MenuHeaderOptions
        {
            Separator = " | ",
            Segments = new List<Func<string>>
            {
                () => $"Пациентов: {patients.Count}",
                () => "Выберите пациента"
            }
        };

        return new MenuState("Список пациентов", items, header: header);
    }
}