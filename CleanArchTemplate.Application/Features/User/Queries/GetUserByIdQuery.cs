using CleanArchTemplate.Aplication.Abstractions.Cqrs.Query;
using CleanArchTemplate.Infrastructure.Repositories.User;
using CleanArchTemplate.SharedKernel.Models.Input.User.Models;
using CleanArchTemplate.SharedKernel.Models.Input.User.Models.Output;
using CleanArchTemplate.SharedKernel.Models.User.Output;
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
        return user?.ToOutput();
    }
}