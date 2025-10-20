using FriendsSociety.Shaurya.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public static class JwtTokenHelper
{
    public static string GenerateJwtToken(User user, IEnumerable<string> roles, IConfiguration config)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (config == null) throw new ArgumentNullException(nameof(config));

        // Support multiple common configuration keys for the secret
        var secret = config["JwtSettings:Secret"] ?? config["Jwt:Key"] ?? config["JwtSettings:Key"];
        if (string.IsNullOrWhiteSpace(secret))
            throw new InvalidOperationException("JWT secret is not configured. Set 'JwtSettings:Secret' or 'Jwt:Key' in configuration.");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (roles != null)
        {
            foreach (var role in roles)
            {
                if (!string.IsNullOrWhiteSpace(role))
                    claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var issuer = config["JwtSettings:Issuer"] ?? config["Jwt:Issuer"];
        var audience = config["JwtSettings:Audience"] ?? config["Jwt:Audience"];

        // expiry in hours (optional)
        var expiryHours = 1;
        var expiryConfig = config["JwtSettings:ExpiryHours"] ?? config["Jwt:ExpiryHours"];
        if (!string.IsNullOrWhiteSpace(expiryConfig) && int.TryParse(expiryConfig, out var parsed))
        {
            expiryHours = parsed;
        }

        var token = new JwtSecurityToken(
            issuer: string.IsNullOrWhiteSpace(issuer) ? null : issuer,
            audience: string.IsNullOrWhiteSpace(audience) ? null : audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expiryHours),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
