namespace Application.Services.Interfaces;

public interface IAppSettingsService
{
    Task<Guid> GetDefaultUserIdAsync();
}