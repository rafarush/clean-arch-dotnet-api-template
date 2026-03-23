using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Query;
using CleanArchTemplate.Application.Abstractions.Models.Input;
using CleanArchTemplate.Application.Features.User.Validators;
using CleanArchTemplate.Application.Repositories.User;
using CleanArchTemplate.SharedKernel.Models.General.Output;
using CleanArchTemplate.SharedKernel.Models.User.Input;
using CleanArchTemplate.SharedKernel.Models.User.Output;
using FluentValidation;

namespace CleanArchTemplate.Application.Features.User.Queries;

public sealed record SearchUsersQuery(SearchUsersInput UsersInput) : BasePaginatedQuery<UserOutput>
{
    private static readonly string[] AllowedFields =
    [
        "name",
        "lastName",
        "created_at",
        "updated_at",
        "email",
        "id"
    ];

    public override bool IsOffsetFieldValid() => AllowedFields.Contains(OffsetField);
}

internal sealed class SearchUsersQueryHandler(
    IUserRepository userRepository,
    IValidator<SearchUsersQuery> searchUserValidator) : IQueryHandler<SearchUsersQuery, Result<PaginatedOutput<UserOutput>>>
{
    public async Task<Result<PaginatedOutput<UserOutput>>> Handle(SearchUsersQuery query, CancellationToken ct)
    {
        
        await searchUserValidator.ValidateAndThrowAsync<SearchUsersQuery>(query, ct);
        
        var result = await userRepository.SearchUsersAsync(query.UsersInput,  ct);

        return Result<PaginatedOutput<UserOutput>>.Success(result);
    }
}