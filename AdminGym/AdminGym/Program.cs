using AdminGym.Repositories;
using AdminGym.Screens;
using AdminGym.Services;

class Program
{
    public static void Main(string[] args)
    {
        // Se mantiene el buffer de pantalla alterna: evita que terminales
        // basadas en bloques mezclen el historial de comandos con las
        // pantallas redibujadas de la app.
        Console.Write("\u001b[?1049h");

        try
        {
            MiembroRepository miembroRepository = new();
            MembresiaRepository membresiaRepository = new();

            MiembroService miembroService = new(miembroRepository);
            MembresiaService membresiaService = new(membresiaRepository);

            MainScreen screen = new(miembroService, membresiaService);

            screen.Show();
        }
        finally
        {
            Console.Write("\u001b[?1049l");
        }
    }
}
