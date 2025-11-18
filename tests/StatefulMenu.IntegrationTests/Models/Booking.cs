using System;
using StatefulMenu.Core.Attributes;

namespace StatefulMenu.IntegrationTests.Models;

public record Booking(
    [property: InputField("Date", Order = 1)]
    DateOnly Date,
    [property: InputField("Time", Order = 2)]
    TimeOnly Time,
    [property: InputField("Doctor", Order = 3)]
    string Doctor
);