using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using CleanArchTemplate.Aplication.Features.Auth.Models.Input;
using CleanArchTemplate.Aplication.Features.Auth.Models.Output;
using CleanArchTemplate.Aplication.Features.Auth.Options;
using CleanArchTemplate.Domain.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchTemplate.Aplication.Features.Auth.Services;

public class JwtService(IHostingEnvironment hostingEnvironment, IOptions<JwtOptions> jwtOptions) : IJwtService
{
    public TokenOutput CreateToken(TokenInput input)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        List<Claim> claims =
        [
            new(AppClaims.UserId, input.User.ToString()),
            new(AppClaims.Policies, JsonSerializer.Serialize(input.Policies)),
            new(AppClaims.Email, input.Email),
            new(AppClaims.Role, JsonSerializer.Serialize(input.Policies))
        ];
        
        var securityAccessToken = new JwtSecurityToken(
            issuer: jwtOptions.Value.Issuer,
            audience: jwtOptions.Value.Audience,
            claims: claims,
            notBefore: DateTime.Now,
            expires: hostingEnvironment.IsStaging() ? DateTime.Now.AddHours(1) : DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);
        var securityRefreshToken = new JwtSecurityToken(
            issuer: jwtOptions.Value.Issuer,
            audience: jwtOptions.Value.Audience,
            claims: claims,
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddHours(12),
            signingCredentials: credentials);
        
        var handler = new JwtSecurityTokenHandler();
        var accessToken = handler.WriteToken(securityAccessToken);
        var refreshToken = handler.WriteToken(securityRefreshToken);
        return new TokenOutput(accessToken, refreshToken, input.User, input.Email, input.Policies, input.Roles);
    }
}