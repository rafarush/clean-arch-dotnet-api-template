using CleanArchTemplate.SharedKernel.Models.General;
using CleanArchTemplate.SharedKernel.Models.General.Input;

namespace CleanArchTemplate.SharedKernel.Models.User.Params;

public class SearchUsersParams : BasePaginatedParams
{
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
}