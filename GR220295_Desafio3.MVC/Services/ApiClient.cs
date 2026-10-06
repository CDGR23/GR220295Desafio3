using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using GR220295_Desafio3.Entities.Dtos;
using GR220295_Desafio3.MVC.Models;

namespace GR220295_Desafio3.MVC.Services;

public class ApiClient(HttpClient cliente, IHttpContextAccessor contexto)
{
    private void AgregarToken()
    {
        cliente.DefaultRequestHeaders.Authorization = null;
        var token = contexto.HttpContext?.User.FindFirst("Token")?.Value;
        if (!string.IsNullOrWhiteSpace(token))
        {
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<(bool Exito, string Mensaje, RespuestaLoginDto? Datos)> IniciarSesionAsync(IniciarSesionDto dto)
    {
        var respuesta = await cliente.PostAsJsonAsync("api/usuarios/iniciar-sesion", dto);
        var texto = await respuesta.Content.ReadAsStringAsync();
        if (!respuesta.IsSuccessStatusCode) return (false, texto, null);
        var datos = await respuesta.Content.ReadFromJsonAsync<RespuestaLoginDto>();
        return datos is null ? (false, "Respuesta inválida de la API.", null) : (true, string.Empty, datos);
    }

    public async Task<(bool Exito, string Mensaje)> RegistrarAsync(RegistrarUsuarioDto dto)
    {
        var respuesta = await cliente.PostAsJsonAsync("api/usuarios/registrar", dto);
        if (respuesta.IsSuccessStatusCode) return (true, string.Empty);
        return (false, await respuesta.Content.ReadAsStringAsync());
    }


    public async Task<UsuarioDto?> ObtenerPerfilAsync()
    {
        AgregarToken();
        var respuesta = await cliente.GetAsync("api/usuarios/perfil");
        if (!respuesta.IsSuccessStatusCode) return null;
        return await respuesta.Content.ReadFromJsonAsync<UsuarioDto>();
    }

    public async Task<(bool Exito, string Mensaje, UsuarioDto? Perfil)> ActualizarPerfilAsync(ActualizarPerfilDto dto)
    {
        AgregarToken();
        var respuesta = await cliente.PutAsJsonAsync("api/usuarios/perfil", dto);
        if (!respuesta.IsSuccessStatusCode) return (false, await respuesta.Content.ReadAsStringAsync(), null);
        var perfil = await respuesta.Content.ReadFromJsonAsync<UsuarioDto>();
        return (true, "Perfil actualizado correctamente.", perfil);
    }

    public async Task<List<PlazaDto>> ObtenerPlazasAsync(bool soloDisponibles = false)
    {
        var respuesta = await cliente.GetAsync($"api/plazas?soloDisponibles={soloDisponibles.ToString().ToLower()}");
        if (!respuesta.IsSuccessStatusCode) return [];
        return await respuesta.Content.ReadFromJsonAsync<List<PlazaDto>>() ?? [];
    }

    public async Task<PlazaDto?> ObtenerPlazaAsync(int id)
    {
        var respuesta = await cliente.GetAsync($"api/plazas/{id}");
        if (!respuesta.IsSuccessStatusCode) return null;
        return await respuesta.Content.ReadFromJsonAsync<PlazaDto>();
    }

    public async Task<(bool Exito, string Mensaje)> CrearPlazaAsync(CrearPlazaDto dto)
    {
        AgregarToken();
        var respuesta = await cliente.PostAsJsonAsync("api/plazas", dto);
        return await ObtenerResultadoAsync(respuesta);
    }

    public async Task<(bool Exito, string Mensaje)> ActualizarPlazaAsync(int id, ActualizarPlazaDto dto)
    {
        AgregarToken();
        var respuesta = await cliente.PutAsJsonAsync($"api/plazas/{id}", dto);
        return await ObtenerResultadoAsync(respuesta);
    }

    public async Task<(bool Exito, string Mensaje)> EliminarPlazaAsync(int id)
    {
        AgregarToken();
        var respuesta = await cliente.DeleteAsync($"api/plazas/{id}");
        return await ObtenerResultadoAsync(respuesta);
    }

    public async Task<(bool Exito, string Mensaje)> CambiarEstadoPlazaAsync(int id, string estado)
    {
        AgregarToken();
        var respuesta = await cliente.PutAsJsonAsync($"api/plazas/{id}/estado", new EstadoPlazaDto { Estado = estado });
        return await ObtenerResultadoAsync(respuesta);
    }

    public async Task<(bool Exito, string Mensaje)> PublicarPlazaAsync(int id)
    {
        AgregarToken();
        var respuesta = await cliente.PutAsync($"api/plazas/{id}/publicar", null);
        return await ObtenerResultadoAsync(respuesta);
    }

    public async Task<(bool Exito, string Mensaje)> CrearPostulacionAsync(int idPlaza)
    {
        AgregarToken();
        var respuesta = await cliente.PostAsJsonAsync("api/postulaciones", new CrearPostulacionDto { IdPlaza = idPlaza });
        return await ObtenerResultadoAsync(respuesta);
    }

    public async Task<List<PostulacionDto>> ObtenerMisPostulacionesAsync()
    {
        AgregarToken();
        var respuesta = await cliente.GetAsync("api/postulaciones/mias");
        if (!respuesta.IsSuccessStatusCode) return [];
        return await respuesta.Content.ReadFromJsonAsync<List<PostulacionDto>>() ?? [];
    }

    public async Task<List<PostulacionDto>> ObtenerPostulacionesAsync()
    {
        AgregarToken();
        var respuesta = await cliente.GetAsync("api/postulaciones");
        if (!respuesta.IsSuccessStatusCode) return [];
        return await respuesta.Content.ReadFromJsonAsync<List<PostulacionDto>>() ?? [];
    }

    public async Task<(bool Exito, string Mensaje)> CambiarEstadoPostulacionAsync(int id, string estado)
    {
        AgregarToken();
        var respuesta = await cliente.PutAsJsonAsync($"api/postulaciones/{id}/estado", new CambiarEstadoPostulacionDto { Estado = estado });
        return await ObtenerResultadoAsync(respuesta);
    }

    public async Task<HojaDeVidaDto?> ObtenerMiHojaDeVidaAsync()
    {
        AgregarToken();
        var respuesta = await cliente.GetAsync("api/hojasDevida/mia");
        if (!respuesta.IsSuccessStatusCode) return null;
        return await respuesta.Content.ReadFromJsonAsync<HojaDeVidaDto>();
    }

    public async Task<(bool Exito, string Mensaje)> GuardarHojaDeVidaAsync(HojaDeVidaViewModel modelo)
    {
        AgregarToken();
        using var contenido = new MultipartFormDataContent();
        contenido.Add(new StringContent(modelo.FormacionAcademica), "FormacionAcademica");
        contenido.Add(new StringContent(modelo.ExperienciaLaboral), "ExperienciaLaboral");
        contenido.Add(new StringContent(modelo.Competencias), "Competencias");

        if (modelo.ArchivoCV is not null && modelo.ArchivoCV.Length > 0)
        {
            var archivo = new StreamContent(modelo.ArchivoCV.OpenReadStream());
            archivo.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            contenido.Add(archivo, "ArchivoCV", modelo.ArchivoCV.FileName);
        }

        var respuesta = await cliente.PostAsync("api/hojasdevida", contenido);
        return await ObtenerResultadoAsync(respuesta);
    }


    public async Task<(byte[]? Archivo, string Nombre)> DescargarCvAsync(string idUsuario)
    {
        AgregarToken();
        var respuesta = await cliente.GetAsync($"api/hojasdevida/archivo/{Uri.EscapeDataString(idUsuario)}");
        if (!respuesta.IsSuccessStatusCode) return (null, string.Empty);
        var bytes = await respuesta.Content.ReadAsByteArrayAsync();
        var nombre = respuesta.Content.Headers.ContentDisposition?.FileNameStar
            ?? respuesta.Content.Headers.ContentDisposition?.FileName
            ?? "CV.pdf";
        return (bytes, nombre.Trim('\"'));
    }

    public async Task<List<UsuarioDto>> ObtenerUsuariosAsync()
    {
        AgregarToken();
        var respuesta = await cliente.GetAsync("api/usuarios");
        if (!respuesta.IsSuccessStatusCode) return [];
        return await respuesta.Content.ReadFromJsonAsync<List<UsuarioDto>>() ?? [];
    }

    public async Task<(bool Exito, string Mensaje)> CambiarRolAsync(string id, string rol)
    {
        AgregarToken();
        var respuesta = await cliente.PutAsJsonAsync($"api/usuarios/{id}/rol", new CambiarRolDto { Rol = rol });
        return await ObtenerResultadoAsync(respuesta);
    }

    public async Task<(bool Exito, string Mensaje)> EliminarUsuarioAsync(string id)
    {
        AgregarToken();
        var respuesta = await cliente.DeleteAsync($"api/usuarios/{id}");
        return await ObtenerResultadoAsync(respuesta);
    }

    public async Task<(bool Exito, string Mensaje)> CrearUsuarioAdminAsync(RegistrarUsuarioDto dto, string rol)
    {
        AgregarToken();
        var respuesta = await cliente.PostAsJsonAsync($"api/usuarios/crear-por-administrador?rol={Uri.EscapeDataString(rol)}", dto);
        return await ObtenerResultadoAsync(respuesta);
    }

    private static async Task<(bool Exito, string Mensaje)> ObtenerResultadoAsync(HttpResponseMessage respuesta)
    {
        if (respuesta.IsSuccessStatusCode) return (true, "Operación realizada correctamente.");
        var mensaje = await respuesta.Content.ReadAsStringAsync();
        if (respuesta.StatusCode == HttpStatusCode.Unauthorized) mensaje = "No está autenticado o su sesión expiró.";
        if (respuesta.StatusCode == HttpStatusCode.Forbidden) mensaje = "No tiene permisos para realizar esta operación.";
        return (false, string.IsNullOrWhiteSpace(mensaje) ? "La API devolvió un error." : mensaje);
    }
}
