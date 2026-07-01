using System.Security.Cryptography;
using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Query;
using CleanArchTemplate.Application.Services.Auth.OAuthService;
using CleanArchTemplate.Domain.AuthProvider;
using CleanArchTemplate.SharedKernel.Models.Auth.Output;

namespace CleanArchTemplate.Application.Features.Auth.Queries;

public sealed record GetOAuthUrlQuery(OAuthProviderType Provider) : IQuery<Result<OAuthUrlOutput>>;

internal sealed class GetOAuthUrlQueryHandler(IOAuthService oAuthService)
    : IQueryHandler<GetOAuthUrlQuery, Result<OAuthUrlOutput>>
{
    public Task<Result<OAuthUrlOutput>> Handle(GetOAuthUrlQuery query, CancellationToken ct)
    {
        var state = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var url = oAuthService.GetAuthorizationUrl(query.Provider, state);
        return Task.FromResult(Result<OAuthUrlOutput>.Success(new OAuthUrlOutput(url, state)));
    }
}
