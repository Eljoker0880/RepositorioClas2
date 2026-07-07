global using AdminGym.Models;
using AdminGym.Services;
using Spectre.Console;


class Program
{
    public static bool running = true;
    public static MiembroService miembroService = new MiembroService();
    public static MembresiaService membresiaService = new MembresiaService();
    public static void MostrarEncabezado()
    {

    }


    public static void Main(string[] args)
    {
        while (running)
        {
            AnsiConsole.Clear();

            AnsiConsole.Write(
                new FigletText("AdminGym")
                    .Centered()
                    .Color(Color.Red)
            );

            AnsiConsole.WriteLine();

            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Indica una acción a realizar:[/]")
                    .AddChoices(
                        "1. Mostrar Miembros",
                        "2. Agregar Miembro",
                        "3. Buscar Miembro",
                        "4. Eliminar Miembro",
                        "5. Salir"
                    )
            );

            List<Miembro> miembros = miembroService.FindAll();

            switch (option)
            {
                case "1. Mostrar Miembros":

                    AnsiConsole.Clear();

                    List<Miembro> lista = null;

                    AnsiConsole.Status()
                        .Spinner(Spinner.Known.Dots)
                        .SpinnerStyle(Style.Parse("blue"))
                        .Start("Cargando miembros...", ctx =>
                        {
                            Thread.Sleep(1200); // simulación de carga
                            lista = miembroService.FindAll();
                        });

                    var table = new Table();

                    table.AddColumn("Id");
                    table.AddColumn("Nombre");
                    table.AddColumn("Apellido");
                    table.AddColumn("Teléfono");
                    table.AddColumn("Tipo");
                    table.AddColumn("Inicio");
                    table.AddColumn("Vencimiento");
                    table.AddColumn("Estado");

                    DateTime hoy = DateTime.Today;

                    foreach (Miembro miembro in lista)
                    {
                        Membresia? membresia = membresiaService.FindByMiembroId(miembro.id);
                        string estado = "[grey]Sin membresía[/]";

                        if (membresia != null)
                        {
                            int diasRestantes = (membresia.MembresiaVencimiento - DateTime.Today).Days;

                            if (diasRestantes < 0)
                            {
                                estado = "[red]🔴 Vencida[/]";
                            }
                            else if (diasRestantes <= 15)
                            {
                                estado = "[yellow]🟡 Próxima a vencer[/]";
                            }
                            else
                            {
                                estado = "[green]🟢 Activa[/]";
                            }
                        }

                        if (membresia != null && membresia.MembresiaVencimiento <= hoy)
                        {
                            AnsiConsole.MarkupLine(
                                $"[red]⚠ La membresía de {miembro.Nombre} {miembro.Apellido} ha vencido.[/]"
                            );
                        }

                        table.AddRow(
                            miembro.id.ToString(),
                            miembro.Nombre,
                            miembro.Apellido,
                            miembro.Telefono,
                            membresia?.TipoMembresia ?? "-",
                            membresia?.MembresiaInicio.ToString("dd/MM/yyyy") ?? "-",
                            membresia?.MembresiaVencimiento.ToString("dd/MM/yyyy") ?? "-",
                            estado
                        );
                    }

                    AnsiConsole.Write(table);
                    break;

                case "2. Agregar Miembro":

                    AnsiConsole.Clear();
                    AnsiConsole.WriteLine();
                    AnsiConsole.Write(
                        new Panel("[cyan]Completa los datos personales del miembro[/]")
                            .Header("DATOS PERSONALES")
                            .BorderColor(Color.Cyan)
                    );

                    string nombre = AnsiConsole.Ask<string>("Nombre:");
                    string apellido = AnsiConsole.Ask<string>("Apellido:");
                    string telefono = AnsiConsole.Ask<string>("Teléfono:");

                    

                    AnsiConsole.Write(
                        new Panel("[yellow]⚠ AVISO: En este gimnasio solo se permiten miembros mayores de 15 años en adelante.[/]")
                            .Header("FECHA DE NACIMIENTO")
                            .BorderColor(Color.Yellow)
                    );

                    AnsiConsole.MarkupLine("[grey]Primero selecciona el año[/]");
                    int anio = AnsiConsole.Prompt(
                        new SelectionPrompt<int>()
                            .AddChoices(Enumerable.Range(1950, 80))
                    );

                    AnsiConsole.MarkupLine("[grey]Ahora selecciona el mes[/]");
                    int mes = AnsiConsole.Prompt(
                        new SelectionPrompt<int>()
                            .AddChoices(Enumerable.Range(1, 12))
                    );

                    int diasEnMes = DateTime.DaysInMonth(anio, mes);

                    AnsiConsole.MarkupLine("[grey]Ahora selecciona el día[/]");
                    int dia = AnsiConsole.Prompt(
                        new SelectionPrompt<int>()
                            .AddChoices(Enumerable.Range(1, diasEnMes))
                    );

                    DateTime fechaNacimiento = new DateTime(anio, mes, dia);

                    int edad = DateTime.Today.Year - fechaNacimiento.Year;

                    if (fechaNacimiento.Date > DateTime.Today.AddYears(-edad))
                        edad--;

                    if (edad < 15)
                    {
                        AnsiConsole.Clear();
                        AnsiConsole.MarkupLine("[red]Error: No es posible registrar miembros menores de 15 años.[/]");
                        break;
                    }
                    

                    AnsiConsole.Write(
                        new Panel("[magenta]Selecciona el tipo de membresía[/]")
                            .Header("MEMBRESÍA")
                            .BorderColor(Color.Magenta)
                    );

                    string tipo = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .AddChoices(
                                "1 Día",
                                "1 Semana",
                                "1 Mes"
                            ));

                    DateTime membresiaInicio = DateTime.Today;
                    DateTime membresiaVencimiento;

                    if (tipo == "1 Día")
                        membresiaVencimiento = membresiaInicio.AddDays(1);
                    else if (tipo == "1 Semana")
                        membresiaVencimiento = membresiaInicio.AddDays(7);
                    else
                        membresiaVencimiento = membresiaInicio.AddMonths(1);

                    int maxId = miembros.Count > 0 ? miembros.Max(c => c.id) : 0;

                    Miembro nuevoMiembro = new Miembro
                    {
                        id = maxId + 1,
                        Nombre = nombre,
                        Apellido = apellido,
                        Telefono = telefono,
                        FechaNacimiento = fechaNacimiento
                    };
                    Membresia nuevaMembresia = new Membresia
                    {
                        id = maxId + 1,
                        TipoMembresia = tipo,
                        MembresiaInicio = membresiaInicio,
                        MembresiaVencimiento = membresiaVencimiento,
                        MiembroId = nuevoMiembro.id
                    };
                    AnsiConsole.Clear();

                    bool creado = false;

                    AnsiConsole.Status()
                        .Spinner(Spinner.Known.Star)
                        .SpinnerStyle(Style.Parse("green"))
                        .Start("Registrando miembro...", ctx =>
                        {
                            Thread.Sleep(2000); // simulación de proceso
                            bool miembroCreado = miembroService.Create(nuevoMiembro);
                            bool membresiaCreada = membresiaService.Create(nuevaMembresia);

                            creado = miembroCreado && membresiaCreada;
                        });

                    if (creado)
                        AnsiConsole.MarkupLine("[green]Miembro agregado correctamente.[/]");
                    else
                        AnsiConsole.MarkupLine("[red]Error al agregar el miembro.[/]");

                    break;

                case "4. Eliminar Miembro":

                    AnsiConsole.Clear();

                    int id = AnsiConsole.Ask<int>("Indica el ID del miembro:");

                    AnsiConsole.Clear();

                    var confirmado = AnsiConsole.Confirm($"¿Estás seguro de que deseas eliminar el miembro con el ID {id}?");

                    if (!confirmado)
                    {
                        AnsiConsole.Clear();
                        AnsiConsole.MarkupLine("[yellow]⚠ Eliminación cancelada.[/]");
                        break;
                    }

                    bool eliminado = false;

                    AnsiConsole.Clear();
                    AnsiConsole.Status()
                        .Spinner(Spinner.Known.Dots)
                        .SpinnerStyle(Style.Parse("green"))
                        .Start("Eliminando miembro...", ctx =>
                        {
                            Thread.Sleep(2000);
                            eliminado = miembroService.Delete(id);
                        });

                    if (eliminado)
                        AnsiConsole.MarkupLine($"[green]✔ Miembro con ID {id} eliminado correctamente.[/]");
                    else
                        AnsiConsole.MarkupLine($"[red]❌ Miembro con ID {id} no encontrado.[/]");

                    break;
                case "3. Buscar Miembro":

                    AnsiConsole.Clear();

                    int idBuscar = AnsiConsole.Ask<int>("Ingrese el ID del miembro:");

                    Miembro? miembroEncontrado = null;


                    AnsiConsole.Clear();
                    AnsiConsole.Status()
                        .Spinner(Spinner.Known.Dots)
                        .SpinnerStyle(Style.Parse("green"))
                        .Start("Buscando miembro...", ctx =>
                        {
                            Thread.Sleep(2000); // simulación de proceso
                            miembroEncontrado = miembroService.FindById(idBuscar);
                        });

                    if (miembroEncontrado == null)
                    {
                        AnsiConsole.Clear();
                        AnsiConsole.MarkupLine($"[red] Miembro con ID {idBuscar} no encontrado.[/]");
                    }
                    else
                    {
                        AnsiConsole.Clear();
                        AnsiConsole.MarkupLine("[green] Miembro encontrado[/]");
                        AnsiConsole.WriteLine();
                        var tabla = new Table();

                        tabla.AddColumn("Id");
                        tabla.AddColumn("Nombre");
                        tabla.AddColumn("Apellido");
                        tabla.AddColumn("Teléfono");
                        tabla.AddColumn("Tipo");
                        tabla.AddColumn("Inicio");
                        tabla.AddColumn("Vencimiento");
                        tabla.AddColumn("Estado");

                        Membresia? membresia = membresiaService.FindByMiembroId(miembroEncontrado.id);
                        string estado = "[grey]Sin membresía[/]";

                        if (membresia != null)
                        {
                            int diasRestantes = (membresia.MembresiaVencimiento - DateTime.Today).Days;

                            if (diasRestantes < 0)
                                estado = "[red]🔴 Vencida[/]";
                            else if (diasRestantes <= 15)
                                estado = "[yellow]🟡 Próxima a vencer[/]";
                            else
                                estado = "[green]🟢 Activa[/]";
                        }

                        tabla.AddRow(
                            miembroEncontrado.id.ToString(),
                            miembroEncontrado.Nombre,
                            miembroEncontrado.Apellido,
                            miembroEncontrado.Telefono,
                            membresia?.TipoMembresia ?? "Sin membresía",
                            membresia?.MembresiaInicio.ToString("dd/MM/yyyy") ?? "-",
                            membresia?.MembresiaVencimiento.ToString("dd/MM/yyyy") ?? "-",
                            estado
                        );

                        AnsiConsole.Write(tabla);
                    }

                    break;

                case "5. Salir":

                    running = false;
                    AnsiConsole.MarkupLine("[yellow]Fin de la aplicación.[/]");
                    break;
            }
            if (running)
            {
                AnsiConsole.MarkupLine("\n[grey]Presiona una tecla para volver al menú...[/]");
                Console.ReadKey(true);
            }
        }
    }
}



