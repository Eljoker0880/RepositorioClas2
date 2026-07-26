using Microsoft.Data.Sqlite;

namespace AdminGym.Data;

public class Database
{
    private readonly string _connectionString;

    public Database(string databasePath)
    {
        _connectionString = $"Data Source={databasePath}";
        Initialize();
    }

    public SqliteConnection GetConnection()
    {
        return new SqliteConnection(_connectionString);
    }

    private void Initialize()
    {
        using SqliteConnection connection = GetConnection();

        connection.Open();

        SqliteCommand command = connection.CreateCommand();

        command.CommandText = @"
        CREATE TABLE IF NOT EXISTS miembros(
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            nombre TEXT NOT NULL,
            apellido TEXT NOT NULL,
            telefono TEXT NOT NULL,
            fecha TEXT NOT NULL
        );

        CREATE TABLE IF NOT EXISTS membresias(
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            tipo TEXT NOT NULL,
            inscripcion TEXT NOT NULL,
            vencimiento TEXT NOT NULL,
            id_miembro INTEGER NOT NULL,
            FOREIGN KEY(id_miembro) REFERENCES miembros(id)
        );
        ";

        command.ExecuteNonQuery();
    }
}