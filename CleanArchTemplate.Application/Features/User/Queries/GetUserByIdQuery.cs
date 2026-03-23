using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Query;
using CleanArchTemplate.Application.Repositories.User;
using CleanArchTemplate.SharedKernel.Models.User.Output;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchTemplate.Application.Features.User.Queries;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<Result<UserDetailsOutput>>;


internal sealed class GetUserByIdQueryHandler(
    IUserRepository userRepository
    ) : IQueryHandler<GetUserByIdQuery, Result<UserDetailsOutput>>
{
    public async Task<Result<UserDetailsOutput>> Handle(GetUserByIdQuery query, CancellationToken ct)
    {
        var user = await userRepository.GetWithRelationsAsync(query.UserId, ct);
        return user is null ? 
            Result<UserDetailsOutput>.Failure("User not found", ErrorType.NotFound) 
            : Result<UserDetailsOutput>.Success(user.ToDetailsOutput());
    }
}