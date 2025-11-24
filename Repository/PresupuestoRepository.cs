using System.Transactions;
using Microsoft.Data.Sqlite;
using tl2_tp8_2025_slackku.Interfaces;
using tl2_tp8_2025_slackku.Models;

namespace tl2_tp8_2025_slackku.Repository
{
    public class PresupuestoRepository : IPresupuestoRepository
    {
        private string connectionString = "DataSource=DB/Tienda.db;";
        public bool Crear(Presupuesto presu) // Recibimos un presupuesto [con carga del detalle dentro]
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            string queryStringPresupuesto = "INSERT INTO Presupuestos (NombreDestinatario, FechaCreacion) VALUES(@name, @date); SELECT last_insert_rowid();";

            var command = new SqliteCommand(queryStringPresupuesto, connection);

            command.Parameters.AddWithValue("@name", presu.NombreDestinatario);
            command.Parameters.AddWithValue("@date", presu.FechaCreacion);

            return command.ExecuteNonQuery() == 1;
        }

        public List<Presupuesto> Listar()
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            string queryString = @"SELECT * FROM Presupuestos 
                                    LEFT JOIN PresupuestosDetalle USING(idPresupuesto)
                                    LEFT JOIN Productos USING(idProducto)";

            using var command = new SqliteCommand(queryString, connection);

            using var reader = command.ExecuteReader();
            var presupuestos = MapearPresupuestos(reader);
            return presupuestos;
        }

        private List<Presupuesto> MapearPresupuestos(SqliteDataReader reader)
        {
            var presupuestosDict = new Dictionary<int, Presupuesto>();
            while (reader.Read())
            {
                int idPresupuesto = Convert.ToInt32(reader["idPresupuesto"]);

                // Si es la primera vez que vemos este presupuesto, lo creamos
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

                // --- CORRECCIÓN CLAVE ---
                // Verificamos si la columna 'idProducto' NO es nula (DBNull)
                // Si es nula, significa que el presupuesto no tiene productos, así que no hacemos nada.
                if (reader["idProducto"] != DBNull.Value)
                {
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
            }
            return presupuestosDict.Values.ToList();
        }

        public Presupuesto Obtener(int id)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            // Asegúrate de que la consulta tenga LEFT JOIN
            string queryString = @"SELECT * FROM Presupuestos 
                           LEFT JOIN PresupuestosDetalle USING(idPresupuesto) 
                           LEFT JOIN Productos USING(idProducto) 
                           WHERE idPresupuesto = @id";

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

                if (reader["idProducto"] != DBNull.Value)
                {
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
            }
            return presupuesto;
        }

        public bool AgregarDetalle(int idPresupuesto, int idProducto, int cantidad)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            string queryString = "INSERT INTO PresupuestosDetalle (idProducto, idPresupuesto, cantidad) VALUES(@idProd, @idPresu, @cant)";
            using var command = new SqliteCommand(queryString, connection);

            command.Parameters.AddWithValue("@idPresu", idPresupuesto);
            command.Parameters.AddWithValue("@idProd", idProducto);
            command.Parameters.AddWithValue("@cant", cantidad);

            return command.ExecuteNonQuery() == 1;
        }

        public bool EliminarDetalle(int idPresupuesto, int idProducto)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                string queryDetalle = @"DELETE FROM PresupuestosDetalle 
                                        WHERE idPresupuesto = @idPresupuesto 
                                        AND idProducto = @idProducto";
                using var commandDetalle = new SqliteCommand(queryDetalle, connection, transaction);
                commandDetalle.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);
                commandDetalle.Parameters.AddWithValue("@idProducto", idProducto);
                int flag = commandDetalle.ExecuteNonQuery();
                transaction.Commit();
                return flag == 1;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
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
                int flag = commandDetalle.ExecuteNonQuery();

                string queryPresupuesto = "DELETE FROM Presupuestos WHERE idPresupuesto = @id";
                using var commandPresupuesto = new SqliteCommand(queryPresupuesto, connection, transaction);
                commandPresupuesto.Parameters.AddWithValue("@id", id);
                int afectados = commandPresupuesto.ExecuteNonQuery();
                transaction.Commit();
                return afectados > 0 && flag == 1;
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
                // Primero actualizamos los campos del presupuesto (cabecera)
                string queryHeader = @"UPDATE Presupuestos 
                                        SET NombreDestinatario = @name, FechaCreacion = @date 
                                        WHERE idPresupuesto = @id";
                using (var cmdHeader = new SqliteCommand(queryHeader, connection, transaction))
                {
                    cmdHeader.Parameters.AddWithValue("@name", pModified.NombreDestinatario);
                    cmdHeader.Parameters.AddWithValue("@date", pModified.FechaCreacion);
                    cmdHeader.Parameters.AddWithValue("@id", pModified.IdPresupuesto);
                    cmdHeader.ExecuteNonQuery();
                }

                // Si hay detalles, actualizamos las cantidades existentes.
                if (pModified.Detalle != null && pModified.Detalle.Any())
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