using Account.Application.Abstractions.Services;
using Account.Application.DTOs;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Account.Infrastructure.Services;

public class TokenGenerator : ITokenGenerator
{
    private readonly JwtOptions options;

    public TokenGenerator(IOptions<JwtOptions> options)
    {
        this.options = options.Value;
    }

    public string Generate(Guid userId, string login, string role)
    {
        SigningCredentials creds = GetCredentionals();
        Dictionary<string, object> claims = GetClaims(userId, login, role);
        SecurityTokenDescriptor descriptor = GetDescriptor(creds, claims);

        return new JsonWebTokenHandler().CreateToken(descriptor); ;
    }

    private SigningCredentials GetCredentionals()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.options.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        return creds;
    }

    private static Dictionary<string, object> GetClaims(Guid userId, string login, string role)
    {
        return new Dictionary<string, object>
        {
            [JwtRegisteredClaimNames.Sub] = userId.ToString(),
            [JwtRegisteredClaimNames.PreferredUsername] = login,
            ["role"] = role,
            [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
        };
    }

    private SecurityTokenDescriptor GetDescriptor(SigningCredentials creds, Dictionary<string, object> claims)
    {
        return new SecurityTokenDescriptor
        {
            Issuer = this.options.Issuer,
            Audience = this.options.Audience,
            Claims = claims,
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(this.options.Expires),
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = creds
        };
    }

    

    

}
