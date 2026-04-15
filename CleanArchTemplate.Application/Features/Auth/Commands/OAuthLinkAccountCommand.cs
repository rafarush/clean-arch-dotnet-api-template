using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Repositories.AuthProvider;
using CleanArchTemplate.Application.Repositories.User;
using CleanArchTemplate.Application.Services.Auth.OAuthService;
using CleanArchTemplate.Domain.AuthProvider;

namespace CleanArchTemplate.Application.Features.Auth.Commands;

public sealed record OAuthLinkAccountCommand(Guid UserId, OAuthProviderType Provider, string Code) 
    : ICommand<Result<bool>>;

internal sealed class OAuthLinkAccountCommandHandler(
    IOAuthService oAuthService,
    IAuthProviderRepository authProviderRepository,
    IUserRepository userRepository
    ) : ICommandHandler<OAuthLinkAccountCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(OAuthLinkAccountCommand command, CancellationToken ct)
    {
        var existingProvider = await authProviderRepository.GetByUserAndProviderAsync(command.UserId, command.Provider, ct);
        if (existingProvider is not null)
            return Result<bool>.Failure("Account already linked to this provider", ErrorType.Conflict);

        var user = await userRepository.GetAsync(command.UserId, ct);
        if (user is null)
            return Result<bool>.Failure("User not found", ErrorType.NotFound);

        var userInfo = await oAuthService.GetUserInfoAsync(command.Provider, command.Code, ct);

        var providerAlreadyLinked = await authProviderRepository.GetByProviderAsync(command.Provider, userInfo.ProviderId, ct);
        if (providerAlreadyLinked is not null && providerAlreadyLinked.UserId != command.UserId)
            return Result<bool>.Failure("This provider account is already linked to another user", ErrorType.Conflict);

        var authProvider = new AuthProvider
        {
            UserId = command.UserId,
            Provider = command.Provider,
            ProviderUserId = userInfo.ProviderId
        };

        await authProviderRepository.CreateAsync(authProvider, ct);

        return Result<bool>.Success(true);
    }
}
