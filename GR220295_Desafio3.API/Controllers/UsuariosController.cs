using System.Security.Claims;
using GR220295_Desafio3.Entities.Constantes;
using GR220295_Desafio3.Entities.Dtos;
using GR220295_Desafio3.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GR220295_Desafio3.API.Services;

namespace GR220295_Desafio3.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController(
    UserManager<Usuario> administradorUsuarios,
    JwtTokenService servicioToken) : ControllerBase
{
    [HttpPost("registrar")]
    [AllowAnonymous]
    public async Task<IActionResult> Registrar(RegistrarUsuarioDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.NombreCompleto) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest("Nombre, correo y contraseña son obligatorios.");
        }

        if (dto.NombreCompleto.Length > 150) return BadRequest("El nombre completo no puede exceder 150 caracteres.");

        var usuarioExistente = await administradorUsuarios.FindByEmailAsync(dto.Email);
        if (usuarioExistente is not null) return Conflict("El correo ya está registrado.");

        var usuario = new Usuario
        {
            UserName = dto.Email,
            Email = dto.Email,
            EmailConfirmed = true,
            NombreCompleto = dto.NombreCompleto.Trim(),
            Rol = Roles.Candidato
        };

        var resultado = await administradorUsuarios.CreateAsync(usuario, dto.Password);
        if (!resultado.Succeeded)
        {
            return BadRequest(resultado.Errors.Select(e => e.Description));
        }

        await administradorUsuarios.AddToRoleAsync(usuario, Roles.Candidato);

        return Ok(new UsuarioDto
        {
            Id = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            Email = usuario.Email ?? string.Empty,
            Rol = Roles.Candidato
        });
    }

    [HttpPost("iniciar-sesion")]
    [AllowAnonymous]
    public async Task<IActionResult> IniciarSesion(IniciarSesionDto dto)
    {
        var usuario = await administradorUsuarios.FindByEmailAsync(dto.Email);
        if (usuario is null) return Unauthorized("Correo o contraseña incorrectos.");

        var passwordCorrecto = await administradorUsuarios.CheckPasswordAsync(usuario, dto.Password);
        if (!passwordCorrecto) return Unauthorized("Correo o contraseña incorrectos.");

        var roles = await administradorUsuarios.GetRolesAsync(usuario);
        if (roles.Count == 0) return Forbid();

        var rol = roles.First();
        if (usuario.Rol != rol)
        {
            usuario.Rol = rol;
            await administradorUsuarios.UpdateAsync(usuario);
        }

        return Ok(new RespuestaLoginDto
        {
            Id = usuario.Id,
            Token = servicioToken.CrearToken(usuario, roles),
            NombreCompleto = usuario.NombreCompleto,
            Email = usuario.Email ?? string.Empty,
            Rol = rol
        });
    }

    [HttpPut("perfil")]
    [Authorize]
    public async Task<IActionResult> ActualizarPerfil(ActualizarPerfilDto dto)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id is null) return Unauthorized();

        var usuario = await administradorUsuarios.FindByIdAsync(id);
        if (usuario is null) return NotFound("Usuario no encontrado.");

        usuario.NombreCompleto = dto.NombreCompleto.Trim();
        usuario.PhoneNumber = dto.Telefono?.Trim();

        var resultado = await administradorUsuarios.UpdateAsync(usuario);
        if (!resultado.Succeeded) return BadRequest(resultado.Errors.Select(e => e.Description));

        var roles = await administradorUsuarios.GetRolesAsync(usuario);
        return Ok(new UsuarioDto
        {
            Id = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            Email = usuario.Email ?? string.Empty,
            Rol = roles.FirstOrDefault() ?? usuario.Rol
        });
    }

    [HttpGet("perfil")]
    [Authorize]
    public async Task<IActionResult> ObtenerPerfil()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id is null) return Unauthorized();

        var usuario = await administradorUsuarios.FindByIdAsync(id);
        if (usuario is null) return NotFound("Usuario no encontrado.");

        var roles = await administradorUsuarios.GetRolesAsync(usuario);
        return Ok(new UsuarioDto
        {
            Id = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            Email = usuario.Email ?? string.Empty,
            Rol = roles.FirstOrDefault() ?? usuario.Rol
        });
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<IActionResult> ObtenerUsuarios()
    {
        var usuarios = administradorUsuarios.Users
            .Select(u => new UsuarioDto
            {
                Id = u.Id,
                NombreCompleto = u.NombreCompleto,
                Email = u.Email ?? string.Empty,
                Rol = u.Rol
            })
            .ToList();

        foreach (var usuario in usuarios)
        {
            var entidad = await administradorUsuarios.FindByIdAsync(usuario.Id);
            if (entidad is not null)
            {
                var roles = await administradorUsuarios.GetRolesAsync(entidad);
                usuario.Rol = roles.FirstOrDefault() ?? usuario.Rol;
            }
        }

        return Ok(usuarios);
    }

    [HttpPost("crear-por-administrador")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<IActionResult> CrearPorAdministrador(RegistrarUsuarioDto dto, [FromQuery] string rol)
    {
        if (!Roles.Todos.Contains(rol)) return BadRequest("El rol no es válido.");
        if (await administradorUsuarios.FindByEmailAsync(dto.Email) is not null) return Conflict("El correo ya está registrado.");

        var usuario = new Usuario
        {
            UserName = dto.Email,
            Email = dto.Email,
            EmailConfirmed = true,
            NombreCompleto = dto.NombreCompleto.Trim(),
            Rol = rol
        };

        var resultado = await administradorUsuarios.CreateAsync(usuario, dto.Password);
        if (!resultado.Succeeded) return BadRequest(resultado.Errors.Select(e => e.Description));

        await administradorUsuarios.AddToRoleAsync(usuario, rol);

        return Ok(new UsuarioDto
        {
            Id = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            Email = usuario.Email ?? string.Empty,
            Rol = rol
        });
    }

    [HttpPut("{id}/rol")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<IActionResult> CambiarRol(string id, CambiarRolDto dto)
    {
        if (!Roles.Todos.Contains(dto.Rol)) return BadRequest("El rol no es válido.");

        var usuario = await administradorUsuarios.FindByIdAsync(id);
        if (usuario is null) return NotFound("Usuario no encontrado.");

        var rolesActuales = await administradorUsuarios.GetRolesAsync(usuario);
        if (rolesActuales.Count > 0) await administradorUsuarios.RemoveFromRolesAsync(usuario, rolesActuales);
        await administradorUsuarios.AddToRoleAsync(usuario, dto.Rol);

        usuario.Rol = dto.Rol;
        await administradorUsuarios.UpdateAsync(usuario);

        return Ok(new UsuarioDto
        {
            Id = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            Email = usuario.Email ?? string.Empty,
            Rol = dto.Rol
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<IActionResult> Eliminar(string id)
    {
        var usuario = await administradorUsuarios.FindByIdAsync(id);
        if (usuario is null) return NotFound("Usuario no encontrado.");
        if (usuario.Email == User.FindFirstValue(ClaimTypes.Email)) return BadRequest("No puede eliminar su propio usuario.");

        var resultado = await administradorUsuarios.DeleteAsync(usuario);
        if (!resultado.Succeeded) return BadRequest(resultado.Errors.Select(e => e.Description));

        return NoContent();
    }
}
