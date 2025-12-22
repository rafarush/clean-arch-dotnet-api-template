using CleanArchTemplate.Aplication.Abstractions.Cqrs;
using CleanArchTemplate.Aplication.Abstractions.Cqrs.Query;
using CleanArchTemplate.Aplication.Abstractions.Models.Input;
using CleanArchTemplate.Aplication.Features.User.Validators;
using CleanArchTemplate.Infrastructure.Repositories.User;
using CleanArchTemplate.SharedKernel.Models.General.Output;
using CleanArchTemplate.SharedKernel.Models.Input.User.Models.Output;
using CleanArchTemplate.SharedKernel.Models.User.Input;
using CleanArchTemplate.SharedKernel.Models.User.Output;
using CleanArchTemplate.SharedKernel.Models.User.Params;
using FluentValidation;

namespace CleanArchTemplate.Aplication.Features.User.Queries;

public sealed record SearchUsersQuery(SearchUsersParams UsersParams) : BasePaginatedQuery<UserOutput>
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
        // if (!query.IsOffsetFieldValid())
        // {
        //     return Result<PaginatedOutput<UserOutput>>.Failure("Offset field is invalid", ErrorType.Validation);
        // }

        // if (query.OffsetPage < 1)
        // {
        //     return Result<PaginatedOutput<UserOutput>>.Failure("Offset page is invalid", ErrorType.Validation);
        // }
        //
        // if (query.Limit < 1)
        // {
        //     return Result<PaginatedOutput<UserOutput>>.Failure("Offset page is invalid", ErrorType.Validation);
        // }
        
        await searchUserValidator.ValidateAndThrowAsync<SearchUsersQuery>(query, ct);
        
        var result = await userRepository.SearchUsersAsync(query.UsersParams,  ct);

        return Result<PaginatedOutput<UserOutput>>.Success(result);
    }
}