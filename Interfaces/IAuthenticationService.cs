using tl2_tp8_2025_slackku.Models;
namespace tl2_tp8_2025_slackku.Interfaces;

public interface IAuthenticationService
{
    bool Login(string username, string password);
    void Logout();
    bool IsAuthenticated();
    // Verifica si el usuario actual tiene el rol requerido (ej. "Administrador").
    bool HasAccessLevel(string requiredAccessLevel);
}
