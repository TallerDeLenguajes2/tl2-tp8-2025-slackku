using System.Transactions;
using Microsoft.Data.Sqlite;
using tl2_tp8_2025_slackku.Models;

namespace tl2_tp8_2025_slackku.Repository
{
    public class PresupuestoRepository
    {
        private string connectionString = "DataSource=DB/Tienda.db;";
        public bool Crear(PresupuestoDTO presu) // Recibimos un presupuesto [con carga del detalle dentro]
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
                var commandLast = new SqliteCommand(queryStringPresupuestoDetalle, connection, transaction);
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
            string queryString = @"SELECT * FROM Presupuestos 
                                    INNER JOIN PresupuestosDetalle USING(idPresupuesto)
                                    INNER JOIN Productos USING(idProducto)";

            using var command = new SqliteCommand(queryString, connection);

            using var reader = command.ExecuteReader();
            var presupuestos = MapearPresupuestos(reader);
            return presupuestos;
        }

        private List<Presupuesto> MapearPresupuestos(SqliteDataReader reader)
        {
            List<Presupuesto> listaPresupuesto = new();
            var presupuestosDict = new Dictionary<int, Presupuesto>();
            while (reader.Read())
            {
                int idPresupuesto = Convert.ToInt32(reader["idPresupuesto"]);
                if (!presupuestosDict.ContainsKey(idPresupuesto))
                {
                    var presupuesto = new Presupuesto
                    {
                        IdPresupuesto = Convert.ToInt32(reader["idPresupuesto"]),
                        NombreDestinatario = reader["NombreDestinatario"].ToString(),
                        FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                        Detalle = new List<PresupuestoDetalle>()
                    };
                    presupuestosDict.Add(idPresupuesto, presupuesto);
                }

                var prod = new Producto
                {
                    IdProducto = Convert.ToInt32(reader["idProducto"]),
                    Descripcion = reader["Descripcion"].ToString(),
                    Precio = Convert.ToDouble(reader["Precio"])
                };
                var detalle = new PresupuestoDetalle
                {
                    Producto = prod,
                    Cantidad = Convert.ToInt32(reader["Cantidad"]),
                };
                presupuestosDict[idPresupuesto].Detalle.Add(detalle);
            }
            return presupuestosDict.Values.ToList();
        }

        public Presupuesto Obtener(int id)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            string queryString = "SELECT * FROM Presupuestos INNER JOIN PresupuestosDetalle USING(idPresupuesto) INNER JOIN Productos USING(idProducto) WHERE idPresupuesto = @id";
            using var command = new SqliteCommand(queryString, connection);

            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();
            Presupuesto presupuesto = null;
            while (reader.Read())
            {
                if (presupuesto == null)
                {
                    presupuesto = new Presupuesto
                    {
                        IdPresupuesto = Convert.ToInt32(reader["idPresupuesto"]),
                        NombreDestinatario = reader["NombreDestinatario"].ToString(),
                        FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                        Detalle = new List<PresupuestoDetalle>()
                    };
                }
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
            }
            return presupuesto;
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
            using var transaction = connection.BeginTransaction();
            try
            {
                string queryDetalle = "DELETE FROM PresupuestosDetalle WHERE idPresupuesto = @id";
                using var commandDetalle = new SqliteCommand(queryDetalle, connection, transaction);
                commandDetalle.Parameters.AddWithValue("@id", id);
                commandDetalle.ExecuteNonQuery();

                string queryPresupuesto = "DELETE FROM Presupuestos WHERE idPresupuesto = @id";
                using var commandPresupuesto = new SqliteCommand(queryPresupuesto, connection, transaction);
                commandPresupuesto.Parameters.AddWithValue("@id", id);
                int afectados = commandPresupuesto.ExecuteNonQuery();
                transaction.Commit();
                return afectados > 0;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }

        }

        public bool Modificar(Presupuesto pModified)
        {
            SqliteConnection connection = new SqliteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                foreach (PresupuestoDetalle det in pModified.Detalle)
                {
                    string queryString = @"UPDATE PresupuestosDetalle 
                                        SET Cantidad = @cantidad 
                                        WHERE idPresupuesto = @id AND idProducto = @idProd";

                    var command = new SqliteCommand(queryString, connection, transaction);
                    command.Parameters.AddWithValue("@id", pModified.IdPresupuesto);
                    command.Parameters.AddWithValue("@idProd", det.Producto.IdProducto);
                    command.Parameters.AddWithValue("@cantidad", det.Cantidad);

                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error al actualizar los detalles del presupuesto.", ex);
            }
            return true;
        }

    }
}