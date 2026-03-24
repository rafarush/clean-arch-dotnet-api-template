using System.Security.Cryptography;

namespace CleanArchTemplate.Application.Services.Auth.PasswordHashService.Options;

public sealed class PasswordHashServiceOptions
{
    public const string Section = "PasswordHash";
    public required int SaltSize {get; set;}
    public required int HashSize {get; set;}
    public required int Iterations {get; set;}
    public required string AlgorithmName { get; set; }
    public HashAlgorithmName Algorithm => new HashAlgorithmName(AlgorithmName);

}