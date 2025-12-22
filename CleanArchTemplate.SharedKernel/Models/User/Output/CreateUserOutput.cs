using CleanArchTemplate.SharedKernel.Models.User.Output;

namespace CleanArchTemplate.SharedKernel.Models.Input.User.Models.Output;

public sealed record CreateUserOutput(Guid Id, UserOutput Output);