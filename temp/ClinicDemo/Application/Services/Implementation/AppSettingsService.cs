using Application.Services.Interfaces;

namespace Application.Services.Implementation;

public class AppSettingsService : IAppSettingsService
{
    private readonly Guid _userId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    public Task<Guid> GetDefaultUserIdAsync()
    {
        return Task.FromResult(_userId);
    }
}