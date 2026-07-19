global using AdminGym.Models;
using AdminGym.Services;
using Spectre.Console;
using System.IO;

class Program
{
    public static bool running = true;
    public static MiembroService miembroService = new MiembroService();
    public static MembresiaService membresiaService = new MembresiaService();

    public static void LimpiarConsola()
    {
        try
        {
            Console.Write("\u001b[2J\u001b[H");
        }
        catch (IOException)
        {
            AnsiConsole.Clear();
        }
    }

    public static Table CrearTablaDetallada(int? limite = null)
    {
        var table = new Table();
        table.Border(TableBorder.Rounded);

        table.AddColumn(new TableColumn("Id").Width(6).NoWrap());
        table.AddColumn(new TableColumn("Nombre").Width(16).NoWrap());
        table.AddColumn(new TableColumn("Apellido").Width(16).NoWrap());
        table.AddColumn(new TableColumn("Teléfono").Width(16).NoWrap());
        table.AddColumn(new TableColumn("Tipo").Width(14).NoWrap());
        table.AddColumn(new TableColumn("Inicio").Width(14).NoWrap());
        table.AddColumn(new TableColumn("Vencimiento").Width(14).NoWrap());
        table.AddColumn(new TableColumn("Estado").Width(20).NoWrap());

        List<Miembro> miembros = miembroService.FindAll();

        if (limite.HasValue)
            miembros = miembros.Take(limite.Value).ToList();

        foreach (Miembro miembro in miembros)
        {
            Membresia? membresia = membresiaService.FindByMiembroId(miembro.id);

            string estado = "[grey]Sin membresía[/]";
            string vencimiento = membresia?.Vencimiento.ToString("dd/MM/yyyy") ?? "-";

            if (membresia != null)
            {
                TimeSpan tiempoRestante = membresia.Vencimiento - DateTime.Now;

                if (tiempoRestante.TotalSeconds < 0)
                {
                    estado = "[red]🔴 Vencida[/]";
                }
                else
                {
                    double alertaSegundos = 15 * 24 * 60 * 60;

                    if (membresia.Tipo == "30 Segundos")
                        alertaSegundos = 10;
                    else if (membresia.Tipo == "1 Día")
                        alertaSegundos = 24 * 60 * 60;
                    else if (membresia.Tipo == "1 Semana")
                        alertaSegundos = 3 * 24 * 60 * 60;

                    estado = tiempoRestante.TotalSeconds <= alertaSegundos
                        ? "[yellow]🟡 Próxima a vencer[/]"
                        : "[green]🟢 Activo[/]";
                }
            }

            table.AddRow(
                miembro.id.ToString(),
                miembro.Nombre,
                miembro.Apellido,
                miembro.Telefono,
                membresia?.Tipo ?? "-",