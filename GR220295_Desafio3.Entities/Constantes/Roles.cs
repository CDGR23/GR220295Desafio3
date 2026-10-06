namespace GR220295_Desafio3.Entities.Constantes;

public static class Roles
{
    public const string Administrador = "Administrador";
    public const string Agente = "Agente de Selección";
    public const string Candidato = "Candidato";

    public static readonly string[] Todos =
    {
        Administrador,
        Agente,
        Candidato
    };
}
