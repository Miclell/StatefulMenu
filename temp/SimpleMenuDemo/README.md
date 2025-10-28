# SimpleMenuDemo — добавление команд и динамических пунктов

## Структура
- `Program.cs` — DI и запуск навигации
- `Providers/HomeMenuProvider.cs` — корневой поставщик экрана (`IMenuProvider` создаёт `MenuState`)
- `Commands/*.cs` — команды (`IMenuCommand`), отображаются как пункты меню

`AddStatefulMenu(...)` автоматически сканирует сборку и регистрирует все `IMenuProvider` и `IMenuCommand`.

## Добавление команды
Создайте класс в `Commands`, реализующий `IMenuCommand` — он автоматически появится в меню (если провайдер собирает команды из DI):

```csharp
using StatefulMenu.Commands.Interfaces;
using StatefulMenu.Core.Models;

namespace SimpleMenuDemo.Commands;

public class MyFeatureCommand : IMenuCommand
{
    public string Title => "Моя команда";

    public Task<MenuResult> ExecuteAsync(CancellationToken ct = default)
    {
        Console.WriteLine();
        Console.WriteLine("Выполняю мою команду...");
        Console.ReadKey(true);
        return Task.FromResult(MenuResult.None());
    }
}
```

`HomeMenuProvider` превращает команды в `MenuItem`:

```csharp
var items = _commands
    .Select(cmd => new MenuItem(cmd.Title, _ => cmd.ExecuteAsync(ct)))
    .ToList();
return new MenuState("Главное меню (через команды)", items);
```

## Динамические списки (например, животные)
Два способа:

### А) Через отдельные командные классы
Аналогично `animals.Select(a => new SelectAnimalCommand(a))`:

```csharp
public class SelectAnimalCommand : IMenuCommand
{
    private readonly string _animal;
    public SelectAnimalCommand(string animal) => _animal = animal;
    public string Title => _animal;
    public Task<MenuResult> ExecuteAsync(CancellationToken ct = default)
    {
        Console.WriteLine($"Вы выбрали: {_animal}");
        Console.ReadKey(true);
        return Task.FromResult(MenuResult.None());
    }
}
```

Регистрация статического списка:
```csharp
services.AddTransient<IMenuCommand>(_ => new SelectAnimalCommand("Птица"));
services.AddTransient<IMenuCommand>(_ => new SelectAnimalCommand("Рыба"));
services.AddTransient<IMenuCommand>(_ => new SelectAnimalCommand("Лев"));
```

Для реально динамических данных (БД/файл) удобнее собирать пункты в провайдере.

### Б) Прямо в провайдере без классов-команд

```csharp
public class AnimalsMenuProvider : IMenuProvider
{
    private readonly IEnumerable<string> _animals;
    public AnimalsMenuProvider(IEnumerable<string> animals) => _animals = animals;

    public Task<MenuState> CreateMenuAsync(CancellationToken ct = default)
    {
        var items = _animals
            .Select(animal => new MenuItem(
                title: animal,
                action: _ =>
                {
                    Console.WriteLine($"Вы выбрали: {animal}");
                    Console.ReadKey(true);
                    return Task.FromResult(MenuResult.None());
                }))
            .Append(new MenuItem("Назад", _ => Task.FromResult(MenuResult.Pop())))
            .ToList();

        return Task.FromResult(new MenuState("Животные", items));
    }
}
```

Открывайте этот экран через команду верхнего уровня, возвращающую `MenuResult.Push(state)`.

## Сложный пример (через команды)

Покрывает сценарии:
- Динамический список с переходом в подменю: `OpenProductsCommand`
- Replace текущего экрана: `ReplaceWithInfoCenterCommand`
- Форма ввода с `InputField` и `IDataService`: `OpenUserFormCommand` + `UserForm`
- Хоткеи и стандартные команды: `SayHelloCommand`, `ExitCommand`, `SubmenuCommand`

Запуск:
1. Соберите проект демо
2. В `Program.cs` выберите пункт `2` — «Сложный пример с командами»

Архитектура:
- Команды (`Commands/*`) реализуют `IMenuCommand` и инкапсулируют логику действий
- Провайдер (`Providers/HomeMenuProvider.cs`) собирает команды из DI и строит `MenuState`
- Навигация управляется через возвращаемые `MenuResult` (`Push`, `Replace`, `Pop`, `Exit`)