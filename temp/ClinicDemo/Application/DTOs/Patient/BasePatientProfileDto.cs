namespace Application.DTOs.Patient;

public record BasePatientProfileDto(Guid Id, string LpuShortName, string PatientFirstName, string PatientLastName);