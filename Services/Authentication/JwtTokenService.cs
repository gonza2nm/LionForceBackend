
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace lion_force_be.Services.Authentication;

public class JwtTokenService
{
  private readonly string _issuer;
  private readonly string _audience;
  private readonly string _key;

  public JwtTokenService(IConfiguration conf)
  {
    _issuer = conf["JwtSettings:Issuer"] ?? throw new ArgumentNullException(nameof(_issuer));
    _audience = conf["JwtSettings:Audience"] ?? throw new ArgumentNullException(nameof(_audience));
    _key = conf["JwtSettings:SecretKey"] ?? throw new ArgumentNullException(nameof(_key));
  }

  public string GenerateToken(string dni, string name, string rol)
  {

    var claims = new[]{
      new Claim(JwtRegisteredClaimNames.Sub, dni),
      new Claim("name",name),
      new Claim(ClaimTypes.Role, rol),
      new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
    };
    var keyBytes = Encoding.UTF8.GetBytes(_key);
    var signingKey = new SymmetricSecurityKey(keyBytes);
    var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      Expires = DateTime.UtcNow.AddHours(12),
      Issuer = _issuer,
      Audience = _audience,
      SigningCredentials = signingCredentials
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(securityToken);
  }
}