using tl2_tp8_2025_slackku.Models;
namespace tl2_tp8_2025_slackku.Interfaces;

public interface IUserRepository
{
    Usuario GetUser(string username, string password);
    bool Edit(Usuario usuario);
    bool Delete(int id);
}
