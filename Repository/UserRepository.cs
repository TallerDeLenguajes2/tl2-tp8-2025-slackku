using Microsoft.Data.Sqlite;
using tl2_tp8_2025_slackku.Interfaces;
using tl2_tp8_2025_slackku.Models;

namespace tl2_tp8_2025_slackku.Repository;

public class UserRepository : IUserRepository
{
    private string connectionString = "DataSource=DB/Tienda.db;";
    public Usuario GetUser(string username, string password)
    {
        Usuario user = null;
        const string sql = @"SELECT idUsuario, nombre, username, password, rol
                            FROM Usuarios WHERE username = @usern AND password = @pass";
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("usern", username);
        command.Parameters.AddWithValue("pass", password);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            user = new Usuario
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Username = reader.GetString(2),
                Passwork = reader.GetString(3),
                Rol = reader.GetString(4)
            };
        }
        return user;
    }

    public bool Edit(Usuario usuario)
    {
        throw new NotImplementedException();
    }

    public bool Delete(int id)
    {
        throw new NotImplementedException();
    }
}