namespace Nubulus.Backend.Api.Features.User.Common;

/// <summary>
/// DTO per usuaris disponibles per compartir amb un compte.
/// Només conté la informació mínima necessària per seleccionar usuaris.
/// </summary>
public class UserToShareDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
