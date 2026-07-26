using AdminGym.Data;
using AdminGym.Models;
using Microsoft.Data.Sqlite;

namespace AdminGym.Repositories;

public class MiembroRepository
{
    private readonly Database _database;

    public MiembroRepository(Database database)
    {
        _database = database;
    }

    public List<Miembro> FindAll()
    {
        List<Miembro> miembros = new();

        using SqliteConnection connection = _database.GetConnection();
        connection.Open();

        SqliteCommand command = connection.CreateCommand();

        command.CommandText = @"
            SELECT id, nombre, apellido, telefono, fecha
            FROM miembros";

        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            miembros.Add(new Miembro
            {
                id = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Apellido = reader.GetString(2),
                Telefono = reader.GetString(3),
                Fecha = DateTime.Parse(reader.GetString(4))
            });
        }

        return miembros;
    }


    public int Create(Miembro miembro)
    {
        {
            using SqliteConnection connection = _database.GetConnection();
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = @"
            INSERT INTO miembros
            (nombre, apellido, telefono, fecha)
            VALUES
            ($nombre, $apellido, $telefono, $fecha)";

            command.Parameters.AddWithValue("$nombre", miembro.Nombre);
            command.Parameters.AddWithValue("$apellido", miembro.Apellido);
            command.Parameters.AddWithValue("$telefono", miembro.Telefono);
            command.Parameters.AddWithValue("$fecha", miembro.Fecha.ToString("yyyy-MM-dd"));

            command.ExecuteNonQuery();

            SqliteCommand idCommand = connection.CreateCommand();

            idCommand.CommandText = "SELECT last_insert_rowid();";

            long idGenerado = (long)idCommand.ExecuteScalar();

            return (int)idGenerado;
        }
    }


    public bool Update(Miembro miembro)
    {
        using SqliteConnection connection = _database.GetConnection();
        connection.Open();

        SqliteCommand command = connection.CreateCommand();

        command.CommandText = @"
            UPDATE miembros
            SET
                nombre = $nombre,
                apellido = $apellido,
                telefono = $telefono,
                fecha = $fecha
            WHERE id = $id;
        ";

        command.Parameters.AddWithValue("$id", miembro.id);
        command.Parameters.AddWithValue("$nombre", miembro.Nombre);
        command.Parameters.AddWithValue("$apellido", miembro.Apellido);
        command.Parameters.AddWithValue("$telefono", miembro.Telefono);
        command.Parameters.AddWithValue("$fecha", miembro.Fecha.ToString("yyyy-MM-dd"));

        return command.ExecuteNonQuery() > 0;
    }


    public bool Delete(int id)
    {
        using SqliteConnection connection = _database.GetConnection();
        connection.Open();

        SqliteCommand command = connection.CreateCommand();

        command.CommandText = "DELETE FROM miembros WHERE id = $id";

        command.Parameters.AddWithValue("$id", id);

        return command.ExecuteNonQuery() > 0;
    }


    public Miembro? FindById(int id)
    {
        using SqliteConnection connection = _database.GetConnection();
        connection.Open();

        SqliteCommand command = connection.CreateCommand();

        command.CommandText = @"
            SELECT id, nombre, apellido, telefono, fecha
            FROM miembros
            WHERE id = $id";

        command.Parameters.AddWithValue("$id", id);

        using SqliteDataReader reader = command.ExecuteReader();

        if (!reader.Read())
            return null;

        return new Miembro
        {
            id = reader.GetInt32(0),
            Nombre = reader.GetString(1),
            Apellido = reader.GetString(2),
            Telefono = reader.GetString(3),
            Fecha = DateTime.Parse(reader.GetString(4))
        };
    }
}