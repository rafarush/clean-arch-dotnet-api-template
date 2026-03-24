using CleanArchTemplate.SharedKernel.Models.Auth.Input;
using CleanArchTemplate.SharedKernel.Models.Auth.Output;

namespace CleanArchTemplate.Application.Services.Auth.JwtService;

public interface IJwtService
{
    TokenOutput CreateToken(TokenInput input);
}