using Application.DTOs.Patient;
using Application.Services.Interfaces;
using ClinicDemo.CLI.Menus.MainMenu;
using Microsoft.Extensions.DependencyInjection;
using StatefulMenu.Commands.Interfaces;
using StatefulMenu.Core.Interfaces;
using StatefulMenu.Core.Models;

namespace ClinicDemo.CLI.Menus.PatientMenu.ShowPatientsFlow.Commands;

public class DeletePatientCommand(
    IDataService dataService,
    IPatientService patientService,
    IServiceProvider serviceProvider) : IMenuCommand
{
    public string Title { get; } = "Удалить пациента";

    public async Task<MenuResult> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        dataService.TryGet<BasePatientProfileDto>(nameof(BasePatientProfileDto), out var patient);
        if (patient is null) return MenuResult.None();

        await patientService.Delete(patient.Id, cancellationToken);
        dataService.Remove(nameof(BasePatientProfileDto));

        Console.WriteLine(
            $"Пациент {patient.LpuShortName} | {patient.PatientFirstName} {patient.PatientLastName} успешно удален!\nНажмите любую клавишу для продолжения..");
        Console.ReadKey();

        var main = serviceProvider.GetRequiredService<MainMenuProvider>();
        return MenuResult.Replace(await main.CreateMenuAsync(cancellationToken));
    }
}