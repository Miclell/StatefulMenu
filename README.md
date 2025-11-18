# StatefulMenu

![NuGet](https://img.shields.io/nuget/v/StatefulMenu.svg)
![Tests](https://github.com/Miclell/StatefulMenu/actions/workflows/dotnet.yml/badge.svg)
[English README](./README.en.md)
Документация: [docs/ru](./docs/ru/guide.md) · [docs/en](./docs/en/guide.md)

Минималистичная библиотека для построения «состоящих из экранов» консольных меню на .NET с навигацией по стеку, хоткеями, локализацией и безопасным вводом данных.

## Возможности
- ✅ Интерактивные меню и навигация по стеку экранов
- ✅ Хоткеи, стрелки, выбор по цифрам (многозначный), Esc для «назад», нулевой пункт `0`
- ✅ Ввод моделей из консоли: обязательные/необязательные поля, `nullable`
- ✅ Валидация: RegEx и пользовательские валидаторы/конвертеры
- ✅ Локализация (ru/en) из коробки
- ✅ DI-расширение и авто‑регистрация команд/провайдеров меню
- ✅ Кастомный хэдер (`MenuHeaderOptions`), скрытые пункты (`MenuItem.Hidden`), хелперы `MenuItem.Back/Exit`

## Установка
```bash
dotnet add package StatefulMenu
```

## Быстрый старт
1) Подключите сервисы в DI и запустите навигацию с корневого меню.

```csharp
using Microsoft.Extensions.DependencyInjection;
using StatefulMenu;
using StatefulMenu.Commands.Interfaces;
using StatefulMenu.Core.Interfaces;
using StatefulMenu.Core.Models;

var services = new ServiceCollection()
    // Сканирует текущее сборочное дерево и регистрирует IMenuProvider/IMenuCommand
    .AddStatefulMenu();

var provider = services.BuildServiceProvider();
var nav = provider.GetRequiredService<INavigationService>();

// Корневой провайдер меню (см. пример ниже)
var root = provider.GetRequiredService<IMenuProvider>();

await nav.RunAsync(root);
```

2) Реализуйте `IMenuProvider` и верните `MenuState` с пунктами `MenuItem`.

```csharp
using StatefulMenu.Commands.Interfaces;
using StatefulMenu.Core.Models;

public class HomeMenuProvider : IMenuProvider
{
    public Task<MenuState> CreateMenuAsync(CancellationToken ct = default)
    {
        var items = new[]
        {
            new MenuItem("Сказать привет", async _ =>
            {
                Console.WriteLine("Привет!");
                return MenuResult.None();
            }, hotkey: ConsoleKey.H),

            new MenuItem("Подменю", async _ =>
            {
                var sub = new MenuState("Подменю", new[]
                {
                    new MenuItem("Назад", _ => Task.FromResult(MenuResult.Pop()))
                });
                return MenuResult.Push(sub);
            }) ,

            new MenuItem("Выход", _ => Task.FromResult(MenuResult.Exit()), hotkey: ConsoleKey.E)
        };

        return Task.FromResult(new MenuState("Главное меню", items));
    }
}
```

Навигация управляется значениями `MenuResult`:
- `MenuResult.None()` — остаться на экране
- `MenuResult.Push(state)` — открыть новый экран
- `MenuResult.Replace(state)` — заменить текущий
- `MenuResult.Pop(count)` — вернуться назад на `count`
- `MenuResult.Exit()` — завершить цикл

## Управление с клавиатуры
- **Enter/цифра (1..N)**: выбрать пункт
- **↑/↓**: перемещение по пунктам
- **Esc**: назад (`Pop`)
- **Хоткеи**: если у `MenuItem` задан `ConsoleKey`, нажатие сразу выполнит его действие
- **0**: нулевой пункт «Назад/Выход» (можно выбирать и стрелками)

## Ввод данных из консоли
Сервис `IConsoleInputService` позволяет запрашивать модель, помечая свойства атрибутом `InputField`.

```csharp
using StatefulMenu.Core.Attributes;
using StatefulMenu.Core.Interfaces;

public record CreateUserModel(
    [property: InputField("Email", Pattern = @"^[^@]+@[^@]+\\.[^@]+$", ErrorMessage = "Некорректный email")]
    string Email,

    [property: InputField("Возраст", IsRequired = false)]
    int? Age,

    [property: InputField("Активен")] 
    bool IsActive
);

// Где-то в обработчике меню
var input = provider.GetRequiredService<IConsoleInputService>();
var model = await input.ReadModelAsync<CreateUserModel>();
```

Поддерживается:
- **Обязательные/необязательные поля** (`IsRequired`)
- **Nullable-типы**
- **RegEx** (`Pattern` + `ErrorMessage`)
- **Пользовательские валидаторы/конвертеры** через `Validators`/`Converters` (типы с публичным методом `bool Validate(string input, out string? error)` и конвертеры, возвращающие значение/ошибку)
- **Enum**-поля (с подсказкой допустимых значений)
- **Заголовок формы** через атрибут класса `[InputModel("Заголовок")]`

Локализация сообщений (ru/en) выбирается автоматически по `CultureInfo.CurrentCulture`.

## Общие сервисы
- `INavigationService` — управление стеком экранов, события `Navigating`/`Navigated`
- `IDataService` — простой ключ/значение стор для обмена данными между экранами
- `MenuRenderer` — отрисовка текущего экрана в консоль (вьюпорт с центрированием, хэдер, 0‑пункт)

## DI и авто‑регистрация
`services.AddStatefulMenu()`:
- Регистрирует локализатор, рендерер, ввод, навигацию и стор
- Сканирует указанные сборки (или вызывающую по умолчанию) на реализации `IMenuProvider` и `IMenuCommand` и регистрирует их

## Советы по структуре
- Держите каждый экран как `IMenuProvider`
- Возвращайте `MenuResult` из действий пунктов меню
- Для сложных сценариев храните данные в `IDataService`
 - Для обратной совместимости: просто добавьте `BackCommand/ExitCommand/HomeCommand` в список команд — они автоматически рендерятся как «0»

## Примеры
- Полный демо‑проект: `temp/ClinicDemo`

## Требования
- .NET 9.0+

## Лицензия
MIT — см. `LICENSE`.