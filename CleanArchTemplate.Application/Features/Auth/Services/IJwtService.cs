using CleanArchTemplate.SharedKernel.Models.Auth.Input;
using CleanArchTemplate.SharedKernel.Models.Auth.Output;

namespace CleanArchTemplate.Application.Features.Auth.Services;

public interface IJwtService
{
    TokenOutput CreateToken(TokenInput input);
}