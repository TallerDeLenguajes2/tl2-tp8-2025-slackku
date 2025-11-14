using Microsoft.Data.Sqlite;
using tl2_tp8_2025_slackku.Models;
using SQLitePCL;

namespace tl2_tp8_2025_slackku.Repository
{
    public class ProductoRepository
    {
        private string connectionString = "DataSource=DB/Tienda.db;";
        public bool Crear(ProductoDTO prod)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            string queryString = "INSERT INTO Productos (Descripcion, Precio) VALUES (@desc, @price)";
            var command = new SqliteCommand(queryString, connection);

            command.Parameters.AddWithValue("@desc", prod.Descripcion);
            command.Parameters.AddWithValue("@price", prod.Precio);

            // connection.Close(); No hace falta por el using con connection, al salir del scope se cierra
            return command.ExecuteNonQuery() == 1;
        }
        public bool Modificar(int id, ProductoDTO pModified)
        {
            SqliteConnection connection = new SqliteConnection(connectionString);
            connection.Open();

            string queryString = "UPDATE Productos SET Descripcion = @des, Precio = @price WHERE idProducto = @id"; // El punto pide solo cambiarle el nombre
            var command = new SqliteCommand(queryString, connection);

            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@des", pModified.Descripcion);
            command.Parameters.AddWithValue("@price", pModified.Precio);

            return command.ExecuteNonQuery() == 1;
        }
        
        public List<Producto> Listar()
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            string queryString = "SELECT * FROM Productos";
            using var command = new SqliteCommand(queryString, connection);

            using var reader = command.ExecuteReader();
            var productos = MapearProductos(reader);
            return productos;
        }

        public List<Producto> MapearProductos(SqliteDataReader reader)
        {
            List<Producto> listaProductos = new();
            while (reader.Read())
            {
                var producto = new Producto
                {
                    IdProducto = Convert.ToInt32(reader["idProducto"]),
                    Descripcion = reader["Descripcion"].ToString(),
                    Precio = Convert.ToDouble(reader["Precio"])
                };
                listaProductos.Add(producto);
            }
            return listaProductos;
        }
        public Producto Obtener(int id)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            string queryString = "SELECT * FROM Productos WHERE idProducto = @id";
            using var command = new SqliteCommand(queryString, connection);

            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var producto = new Producto
                {
                    IdProducto = Convert.ToInt32(reader["idProducto"]),
                    Descripcion = reader["Descripcion"].ToString(),
                    Precio = Convert.ToDouble(reader["Precio"]),
                };
                return producto;
            }
            return null;
        }

        public bool Eliminar(int id)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            string queryString = "DELETE FROM Productos WHERE idProducto = @id";
            using var command = new SqliteCommand(queryString, connection);

            command.Parameters.AddWithValue("@id", id);

            return command.ExecuteNonQuery() == 1;
        }
    }
}