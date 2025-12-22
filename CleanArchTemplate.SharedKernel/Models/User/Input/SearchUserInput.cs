using CleanArchTemplate.SharedKernel.Models.General.Input;

namespace CleanArchTemplate.SharedKernel.Models.User.Input;

public class SearchUserInput : BasePaginatedParams
{
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}