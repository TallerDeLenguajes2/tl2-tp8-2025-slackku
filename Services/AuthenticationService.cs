using tl2_tp8_2025_slackku.Interfaces;
namespace tl2_tp8_2025_slackku.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public bool Login(string username, string password)
    {
        var context = _httpContextAccessor.HttpContext;
        var user = _userRepository.GetUser(username, password);
        if (user != null)
        {
            if (context == null)
            {
                throw new InvalidOperationException("HttpContext no está disponible.");
            }
            context.Session.SetString("IsAuthenticated", "true");
            context.Session.SetString("User", user.Username);
            context.Session.SetString("UserNombre", user.Name);
            context.Session.SetString("Rol", user.Rol);
            //es el tipo de acceso/rol admin o cliente
            return true;
        }
        return false;
    }

    public void Logout()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
        {
            throw new InvalidOperationException("HttpContext no está disponible.");
        }
        context.Session.Clear();
    }

    public bool IsAuthenticated()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
        {
            throw new InvalidOperationException("HttpContext no está disponible.");
        }
        return context.Session.GetString("IsAuthenticated") == "true";
    }
    public bool HasAccessLevel(string requiredAccessLevel)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
        {
            throw new InvalidOperationException("HttpContext no está disponible.");
        }
        return context.Session.GetString("Rol") == requiredAccessLevel;
    }

}