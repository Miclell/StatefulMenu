using Application.DTOs.Patient;
using Application.Services.Interfaces;

namespace Application.Services.Implementation;

public class InMemoryPatientService : IPatientService
{
    private readonly List<BasePatientProfileDto> _patients = new();

    public InMemoryPatientService()
    {
        // Seed >10 to test scrolling
        for (var i = 1; i <= 25; i++)
            _patients.Add(new BasePatientProfileDto(Guid.NewGuid(), $"LPU{i:D2}", $"Name{i}", $"Surname{i}"));
    }

    public Task<IReadOnlyList<BasePatientProfileDto>> GetByUser(Guid userId, CancellationToken ct = default)
    {
        return Task.FromResult((IReadOnlyList<BasePatientProfileDto>)_patients.ToList());
    }

    public Task Delete(Guid patientId, CancellationToken ct = default)
    {
        _patients.RemoveAll(p => p.Id == patientId);
        return Task.CompletedTask;
    }

    public Task<BasePatientProfileDto> Create(string lpu, string first, string last, CancellationToken ct = default)
    {
        var dto = new BasePatientProfileDto(Guid.NewGuid(), lpu, first, last);
        _patients.Add(dto);
        return Task.FromResult(dto);
    }
}