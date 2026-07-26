using AdminGym.Data;
using AdminGym.Repositories;
using AdminGym.Screens;
using AdminGym.Services;

class Program
{
    public static void Main(string[] args)
    {
        Console.Write("\u001b[?1049h");

        try
        {
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database");

            Directory.CreateDirectory(folder);

            string dbPath = Path.Combine(folder, "AdminGym.db");

            Database database = new(dbPath);

            MiembroRepository miembroRepository = new(database);
            MembresiaRepository membresiaRepository = new(database);

            MiembroService miembroService = new(miembroRepository);
            MembresiaService membresiaService = new(membresiaRepository);

            MainScreen screen = new(miembroService, membresiaService);

            screen.Show();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Console.ReadKey();
        }
        finally
        {
            Console.Write("\u001b[?1049l");
        }
    }
}