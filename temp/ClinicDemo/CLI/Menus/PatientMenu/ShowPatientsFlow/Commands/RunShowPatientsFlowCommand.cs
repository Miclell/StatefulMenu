using Application.Services.Interfaces;
using ClinicDemo.CLI.Menus.PatientMenu.ShowPatientsFlow.Providers;
using Microsoft.Extensions.DependencyInjection;
using StatefulMenu.Commands.Interfaces;
using StatefulMenu.Core.Models;

namespace ClinicDemo.CLI.Menus.PatientMenu.ShowPatientsFlow.Commands;

public class RunShowPatientsFlowCommand(
    IServiceProvider serviceProvider,
    IPatientService patientService,
    IAppSettingsService appSettings) : IMenuCommand
{
    public string Title => "Показать пациентов";

    public async Task<MenuResult> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var provider = serviceProvider.GetRequiredService<ShowPatientsProvider>();
        return MenuResult.Push(await provider.CreateMenuAsync(cancellationToken));
    }
}