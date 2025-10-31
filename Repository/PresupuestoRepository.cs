using Microsoft.Data.Sqlite;
using Models;

namespace Repository
{
    public class PresupuestoRepository
    {
        private string connectionString = "DataSource=Tienda.db;";
        public bool Crear(Presupuesto presu) // El detalle del presupuesto se establece aqui
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();

            string queryStringPresupuesto = "INSERT INTO Presupuestos (NombreDestinatario, FechaCreacion) VALUES(@name, @date); SELECT last_insert_rowid();";

            var commandFirst = new SqliteCommand(queryStringPresupuesto, connection);
            commandFirst.Transaction = transaction; 

            commandFirst.Parameters.AddWithValue("@name", presu.NombreDestinatario);
            commandFirst.Parameters.AddWithValue("@date", presu.FechaCreacion);

            var idPresupuestoGenerado = (long)commandFirst.ExecuteScalar();

            string queryStringPresupuestoDetalle = "INSERT INTO PresupuestosDetalle (idProducto, idPresupuesto, Cantidad) VALUES(@idProd, @idPresu, @cant)";


            int adderCheck = 0;
            presu.Detalle.ForEach(d =>
            {
                var commandLast = new SqliteCommand(queryStringPresupuestoDetalle, connection);
                commandLast.Transaction = transaction;

                commandLast.Parameters.AddWithValue("@idProd", d.Producto.IdProducto);
                commandLast.Parameters.AddWithValue("@idPresu", idPresupuestoGenerado);
                commandLast.Parameters.AddWithValue("@cant", d.Cantidad);

                adderCheck += commandLast.ExecuteNonQuery();
            });
            transaction.Commit();
            return presu.Detalle.Count() == adderCheck;
        }

        public List<Presupuesto> Listar()
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            string queryString = "SELECT * FROM Presupuestos p INNER JOIN PresupuestosDetalle pd ON p.idPresupuesto = pd.idPresupuesto INNER JOIN Productos pr ON pd.idProducto = pr.idProducto";
            using var command = new SqliteCommand(queryString, connection);

            using var reader = command.ExecuteReader();
            var presupuestos = MapearPresupuestos(reader);
            return presupuestos;
        }

        public List<Presupuesto> MapearPresupuestos(SqliteDataReader reader)
        {
            List<Presupuesto> listaPresupuesto = new();
            while (reader.Read())
            {
                Console.WriteLine(reader.ToString());
                var presupuesto = new Presupuesto
                {
                    IdPresupuesto = Convert.ToInt32(reader["idPresupuesto"]),
                    NombreDestinatario = reader["NombreDestinatario"].ToString(),
                    FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                    Detalle = new List<PresupuestoDetalle>()
                };
                presupuesto.Detalle.Add(new PresupuestoDetalle
                {
                    Cantidad = Convert.ToInt32(reader["Cantidad"]),
                    Producto = new Producto
                    {
                        IdProducto = Convert.ToInt32(reader["idProducto"]),
                        Descripcion = reader["Descripcion"].ToString(),
                        Precio = Convert.ToDouble(reader["Precio"])
                    },
                });

                listaPresupuesto.Add(presupuesto);
            }
            return listaPresupuesto;
        }
        public Presupuesto Obtener(int id)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            string queryString = "SELECT * FROM Presupuestos p INNER JOIN PresupuestosDetalle pd ON p.idPresupuesto = pd.idPresupuesto INNER JOIN Productos pr ON pd.idProducto = pr.idProducto WHERE p.idPresupuesto = @id";
            using var command = new SqliteCommand(queryString, connection);

            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var presupuesto = new Presupuesto
                {
                    IdPresupuesto = Convert.ToInt32(reader["idPresupuesto"]),
                    NombreDestinatario = reader["NombreDestinatario"].ToString(),
                    FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                    Detalle = new List<PresupuestoDetalle>()
                };
                presupuesto.Detalle.Add(new PresupuestoDetalle
                {
                    Cantidad = Convert.ToInt32(reader["Cantidad"]),
                    Producto = new Producto
                    {
                        IdProducto = Convert.ToInt32(reader["idProducto"]),
                        Descripcion = reader["Descripcion"].ToString(),
                        Precio = Convert.ToDouble(reader["Precio"])
                    },
                });
                return presupuesto;
            }
            return null;
        }

        public bool Agregar(int idPresupuesto, Producto prod, int cantidad)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            string queryString = "INSERT INTO PresupuestosDetalle (idProducto, idPresupuesto, cantidad) VALUES(@idProd, @idPresu, @cant)";
            using var command = new SqliteCommand(queryString, connection);

            command.Parameters.AddWithValue("@idPresu", idPresupuesto);
            command.Parameters.AddWithValue("@idProd", prod.IdProducto);
            command.Parameters.AddWithValue("@cant", cantidad);

            return command.ExecuteNonQuery() == 1;
        }
        public bool Eliminar(int id)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            string queryString = "DELETE FROM Presupuestos WHERE idPresupuesto = @id";
            using var command = new SqliteCommand(queryString, connection);

            command.Parameters.AddWithValue("@id", id);

            return command.ExecuteNonQuery() == 1;
        }
    }
}