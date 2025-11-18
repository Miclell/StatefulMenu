using StatefulMenu.Core.Attributes;

namespace StatefulMenu.IntegrationTests.Models;

public class Person
{
    [InputField("Name", Order = 1)] public string Name { get; set; } = string.Empty;

    [InputField("Age", Order = 2)] public int Age { get; set; }

    [InputField("Active?", Order = 3)] public bool Active { get; set; }
}