using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GR220295_Desafio3.Entities.Models;
using Microsoft.IdentityModel.Tokens;

namespace GR220295_Desafio3.API.Services;

public class JwtTokenService(IConfiguration configuracion)
{
    public string CrearToken(Usuario usuario, IList<string> roles)
    {
        var clave = configuracion["Jwt:Clave"] ?? throw new InvalidOperationException("No se configuró la clave JWT.");
        var emisor = configuracion["Jwt:Emisor"] ?? "GR220295_Desafio3.API";
        var audiencia = configuracion["Jwt:Audiencia"] ?? "GR220295_Desafio3.MVC";
        var minutos = int.TryParse(configuracion["Jwt:MinutosExpiracion"], out var valor) ? valor : 120;

        var reclamos = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id),
            new(ClaimTypes.NameIdentifier, usuario.Id),
            new(ClaimTypes.Name, usuario.NombreCompleto),
            new(ClaimTypes.Email, usuario.Email ?? string.Empty)
        };

        reclamos.AddRange(roles.Select(rol => new Claim(ClaimTypes.Role, rol)));

        var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave));
        var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: emisor,
            audience: audiencia,
            claims: reclamos,
            expires: DateTime.UtcNow.AddMinutes(minutos),
            signingCredentials: credenciales);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
