using AdminGym.Data;
using AdminGym.Models;
using Microsoft.Data.Sqlite;

namespace AdminGym.Repositories;

public class MembresiaRepository
{
    public bool Update(Membresia membresia)
    {
        using SqliteConnection connection = _database.GetConnection();
        connection.Open();

        SqliteCommand command = connection.CreateCommand();

        command.CommandText = @"
        UPDATE membresias
        SET
            tipo = $tipo,
            inscripcion = $inscripcion,
            vencimiento = $vencimiento
        WHERE id = $id";

        command.Parameters.AddWithValue("$id", membresia.id);
        command.Parameters.AddWithValue("$tipo", membresia.Tipo);
        command.Parameters.AddWithValue("$inscripcion", membresia.Inscripcion.ToString("yyyy-MM-dd HH:mm:ss"));
        command.Parameters.AddWithValue("$vencimiento", membresia.Vencimiento.ToString("yyyy-MM-dd HH:mm:ss"));

        return command.ExecuteNonQuery() > 0;
    }
    private readonly Database _database;

    public MembresiaRepository(Database database)
    {
        _database = database;
    }

    public List<Membresia> FindAll()
    {
        List<Membresia> membresias = new();

        using SqliteConnection connection = _database.GetConnection();
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT id, tipo, inscripcion, vencimiento, id_miembro
                                FROM membresias";

        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            membresias.Add(new Membresia
            {
                id = reader.GetInt32(0),
                Tipo = reader.GetString(1),
                Inscripcion = DateTime.Parse(reader.GetString(2)),
                Vencimiento = DateTime.Parse(reader.GetString(3)),
                id_miembro = reader.GetInt32(4)
            });
        }

        return membresias;
    }

    public bool Create(Membresia membresia)
    {
        using SqliteConnection connection = _database.GetConnection();
        connection.Open();

        SqliteCommand command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO membresias
            (tipo, inscripcion, vencimiento, id_miembro)
            VALUES
            ($tipo,$inscripcion,$vencimiento,$idMiembro)";

        command.Parameters.AddWithValue("$tipo", membresia.Tipo);
        command.Parameters.AddWithValue("$inscripcion", membresia.Inscripcion.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$vencimiento", membresia.Vencimiento.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$idMiembro", membresia.id_miembro);

        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        using SqliteConnection connection = _database.GetConnection();
        connection.Open();

        SqliteCommand command = connection.CreateCommand();

        command.CommandText = "DELETE FROM membresias WHERE id=$id";
        command.Parameters.AddWithValue("$id", id);

        return command.ExecuteNonQuery() > 0;
    }

    public Membresia? FindById(int id)
    {
        using SqliteConnection connection = _database.GetConnection();
        connection.Open();

        SqliteCommand command = connection.CreateCommand();

        command.CommandText = @"
            SELECT id, tipo, inscripcion, vencimiento, id_miembro
            FROM membresias
            WHERE id=$id";

        command.Parameters.AddWithValue("$id", id);

        using SqliteDataReader reader = command.ExecuteReader();

        if (!reader.Read())
            return null;

        return new Membresia
        {
            id = reader.GetInt32(0),
            Tipo = reader.GetString(1),
            Inscripcion = DateTime.Parse(reader.GetString(2)),
            Vencimiento = DateTime.Parse(reader.GetString(3)),
            id_miembro = reader.GetInt32(4)
        };
    }

    public Membresia? FindByMiembroId(int miembroId)
    {
        using SqliteConnection connection = _database.GetConnection();
        connection.Open();

        SqliteCommand command = connection.CreateCommand();

        command.CommandText = @"
            SELECT id, tipo, inscripcion, vencimiento, id_miembro
            FROM membresias
            WHERE id_miembro=$id";

        command.Parameters.AddWithValue("$id", miembroId);

        using SqliteDataReader reader = command.ExecuteReader();

        if (!reader.Read())
            return null;

        return new Membresia
        {
            id = reader.GetInt32(0),
            Tipo = reader.GetString(1),
            Inscripcion = DateTime.Parse(reader.GetString(2)),
            Vencimiento = DateTime.Parse(reader.GetString(3)),
            id_miembro = reader.GetInt32(4)
        };
    }
}