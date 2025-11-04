using Application.Services.Interfaces;
using StatefulMenu.Commands.Interfaces;
using StatefulMenu.Core.Attributes;
using StatefulMenu.Core.Interfaces;
using StatefulMenu.Core.Models;
using StatefulMenu.Infrastructure.Services;

namespace ClinicDemo.CLI.Menus.PatientMenu.CreatePatientFlow.Commands;

public class RunCreatePatientFlowCommand(IPatientService patientService, IDataService dataService) : IMenuCommand
{
    public string Title => "Создать пациента";

    public async Task<MenuResult> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var input = new ConsoleInputService();
        var model = await input.ReadModelAsync<CreatePatientModel>(cancellationToken);
        if (model is null) return MenuResult.None();

        var created = await patientService.Create(model.Lpu, model.First, model.Last, cancellationToken);
        dataService.Set("LastCreatedPatient", created);

        Console.WriteLine(
            $"Создан пациент: {created.LpuShortName} | {created.PatientFirstName} {created.PatientLastName}");
        Console.WriteLine("Нажмите любую клавишу...");
        Console.ReadKey(true);
        return MenuResult.None();
    }

    [InputModel("Новый пациент")]
    public sealed record CreatePatientModel(
        [property: InputField("LPU")] string Lpu,
        [property: InputField("Имя")] string First,
        [property: InputField("Фамилия")] string Last
    );
}