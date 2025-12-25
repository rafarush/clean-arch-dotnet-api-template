namespace CleanArchTemplate.Infrastructure.Services.Auth.PermissionService;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(Guid userId, string policyName, CancellationToken ct = default);
}