using CleanArchTemplate.Aplication.Abstractions.Cqrs.Query;
using CleanArchTemplate.Application.Features.User.Models;
using CleanArchTemplate.Application.Features.User.Models.Output;
using CleanArchTemplate.Infrastructure.Repositories.User;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchTemplate.Aplication.Features.User.Queries;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserOutput?>;


internal sealed class GetUserByIdQueryHandler(
    IUserRepository userRepository
    ) : IQueryHandler<GetUserByIdQuery, UserOutput?>
{
    public async Task<UserOutput?> Handle(GetUserByIdQuery query, CancellationToken ct)
    {
        var user = await userRepository.GetAsync(query.UserId, ct);
        return user is not null ? user.ToOutput() : null;
    }
}