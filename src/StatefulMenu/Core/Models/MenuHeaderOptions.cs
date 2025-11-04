namespace StatefulMenu.Core.Models;

public sealed class MenuHeaderOptions
{
    public string Separator { get; init; } = " | ";

    // Сегменты, вычисляемые на лету при каждой отрисовке
    public IReadOnlyList<Func<string>> Segments { get; init; } = Array.Empty<Func<string>>();
}


