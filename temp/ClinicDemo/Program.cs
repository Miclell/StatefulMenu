using System.Text;
using Application.Services.Implementation;
using Application.Services.Interfaces;
using ClinicDemo.CLI.Menus.MainMenu;
using Microsoft.Extensions.DependencyInjection;
using StatefulMenu;
using StatefulMenu.Commands.Interfaces;
using StatefulMenu.Core.Interfaces;

var services = new ServiceCollection()
    .AddStatefulMenu()
    .AddSingleton<IAppSettingsService, AppSettingsService>()
    .AddSingleton<IPatientService, InMemoryPatientService>();

var provider = services.BuildServiceProvider();
var nav = provider.GetRequiredService<INavigationService>();
var root = provider.GetRequiredService<IMenuProvider>();

Console.OutputEncoding = Encoding.UTF8;
await nav.RunAsync(root);