using TheFullStackTeam.Domain.Entities;

namespace TheFullStackTeam.Domain.Services;

public interface IMonikerService
{
    Task<string> GenerateMonikerAsync<T>(string baseText) where T : IdentifiableEntity;
}
