using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Query;
using CleanArchTemplate.Infrastructure.Repositories.User;
using CleanArchTemplate.SharedKernel.Models.User.Output;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchTemplate.Application.Features.User.Queries;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<Result<UserOutput>>;


internal sealed class GetUserByIdQueryHandler(
    IUserRepository userRepository
    ) : IQueryHandler<GetUserByIdQuery, Result<UserOutput>>
{
    public async Task<Result<UserOutput>> Handle(GetUserByIdQuery query, CancellationToken ct)
    {
        var user = await userRepository.GetAsync(query.UserId, ct);
        return user is null ? 
            Result<UserOutput>.Failure("User not found", ErrorType.NotFound) 
            : Result<UserOutput>.Success(user.ToOutput());
    }
}