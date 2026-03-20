using System.Security.Cryptography;

namespace CleanArchTemplate.Infrastructure.Services.Auth.Options;

public sealed class PasswordHashServiceOptions
{
    public const string SectionName = "PasswordHash";
    public required int SaltSize {get; set;}
    public required int HashSize {get; set;}
    public required int Iterations {get; set;}
    public required string AlgorithmName { get; set; }
    public HashAlgorithmName Algorithm => new HashAlgorithmName(AlgorithmName);

}