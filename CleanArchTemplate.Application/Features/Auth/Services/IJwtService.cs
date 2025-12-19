using CleanArchTemplate.Aplication.Features.Auth.Models.Input;
using CleanArchTemplate.Aplication.Features.Auth.Models.Output;

namespace CleanArchTemplate.Aplication.Features.Auth.Services;

public interface IJwtService
{
    TokenOutput CreateToken(TokenInput input);
}