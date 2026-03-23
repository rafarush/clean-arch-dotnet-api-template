using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Features.User;
using CleanArchTemplate.Application.Repositories.Security.Role;
using CleanArchTemplate.Application.Repositories.User;
using CleanArchTemplate.Application.Services.Auth.PasswordHashService;
using CleanArchTemplate.Application.Services.Email;
using CleanArchTemplate.Application.Services.Email.Abstractions;
using CleanArchTemplate.Application.Services.Email.TemplateModels;
using CleanArchTemplate.Domain.Security;
using CleanArchTemplate.SharedKernel.Models.Auth.Input;
using CleanArchTemplate.SharedKernel.Models.User.Output;
using FluentValidation;

namespace CleanArchTemplate.Application.Features.Auth.Commands;

public sealed record SignUpCommand(SignUpInput Input) : ICommand<Result<CreateUserOutput>>;


internal sealed class SignUpCommandHandler(
    IUserRepository userRepository,
    IValidator<SignUpInput> signUpValidator,
    IRoleRepository roleRepository,
    IEmailService emailService,
    IPasswordHashService passwordHashService) : ICommandHandler<SignUpCommand, Result<CreateUserOutput>>
{
    public async Task<Result<CreateUserOutput>> Handle(SignUpCommand command, CancellationToken ct)
    {
        if (await userRepository.GetByEmailAsync(command.Input.Email, ct) is not null)
            return Result<CreateUserOutput>.Failure("This email is already in use", ErrorType.Conflict);
        
        await signUpValidator.ValidateAndThrowAsync(command.Input, ct);
        
        var clientRole = await roleRepository.GetByNameAsync("User", ct);
        if (clientRole is null)
            return Result<CreateUserOutput>.InternalError("Default role not found");
        
        var pass = await passwordHashService.HashPassword(command.Input.Password);
        var user = command.Input.ToUser(pass);
        var roles = new List<Role>([clientRole]);
        var userId = await userRepository.CreateAsync(user, ct, roles);
        // TODO: Verification Code or token to Confirm User
        await emailService.SendWithTemplateAsync(
            new EmailMessage(user.Email, "Confirm your email"),
            templateKey: "Auth/ConfirmEmailTemplate",
            model: new ConfirmEmailModel(user.Name + " " + user.LastName, "C0D3", 10),
            ct
            );
        
        return Result<CreateUserOutput>.Success(new (userId, user.ToOutput()), "User created successfully");
    }
}