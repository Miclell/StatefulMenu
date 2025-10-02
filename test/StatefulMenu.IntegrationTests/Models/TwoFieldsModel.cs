using StatefulMenu.Core.Attributes;

namespace StatefulMenu.IntegrationTests.Models;

public class TwoFieldsModel
{
    [InputField("Field1", Order = 1)] public string Field1 { get; set; } = string.Empty;

    [InputField("Field2", Order = 2)] public string Field2 { get; set; } = string.Empty;
}