using Application.DTOs.Patient;

namespace Application.Services.Interfaces;

public interface IPatientService
{
    Task<IReadOnlyList<BasePatientProfileDto>> GetByUser(Guid userId, CancellationToken ct = default);
    Task Delete(Guid patientId, CancellationToken ct = default);
    Task<BasePatientProfileDto> Create(string lpu, string first, string last, CancellationToken ct = default);
}