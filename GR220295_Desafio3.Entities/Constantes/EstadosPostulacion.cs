namespace GR220295_Desafio3.Entities.Constantes;

public static class EstadosPostulacion
{
    public const string EnRevision = "En revisión";
    public const string Aprobado = "Aprobado";
    public const string Rechazado = "Rechazado";

    public static readonly string[] Todos =
    {
        EnRevision,
        Aprobado,
        Rechazado
    };
}
