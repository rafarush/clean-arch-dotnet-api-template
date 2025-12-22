using CleanArchTemplate.SharedKernel.Models.User.Input;

namespace CleanArchTemplate.Aplication.Features.User.Commands;

public sealed record UpdateUserCommand(UpdateUserInput Input);