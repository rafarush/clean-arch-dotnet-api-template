using CleanArchTemplate.SharedKernel.Models.General.Input;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanArchTemplate.SharedKernel.Models.User.Input;

public class SearchUsersInput : BasePaginatedInput
{
    /// <summary>Ordering field. Valid values: "name", "lastName", "created_at", "updated_at", "email", "id"</summary>
    [SwaggerSchema(Description = "Ordering field. Valid values: name, lastName, created_at, updated_at, email, id")]
    public override required string OffsetField { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
}