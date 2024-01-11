using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class TokenService
{
    private readonly string _jwtSecretKey;
    private readonly ILogger<TokenService> _logger; 

    public TokenService(string jwtSecretKey, ILogger<TokenService> logger)
    {
        _jwtSecretKey = jwtSecretKey;
        _logger = logger;
    }

    public string? ValidateTokenAndGetUserId(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSecretKey);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            _logger.LogInformation("Token validated successfully for user ID: {UserId}", userId);

            return userId; 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");

            return null;
        }
    }
}
