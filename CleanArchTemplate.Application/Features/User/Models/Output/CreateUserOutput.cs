using CleanArchTemplate.Application.Features.User.Models.Output;

namespace CleanArchTemplate.Aplication.Features.User.Models.Output;

public sealed record CreateUserOutput(Guid Id, UserOutput Output);