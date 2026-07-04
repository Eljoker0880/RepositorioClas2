global using AdminGym.Models;
using AdminGym.Services;
using Spectre.Console;


class Program
{
    public static bool running = true;
    public static ClienteService clienteService = new ClienteService();
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
                        "1. Mostrar clientes",
                        "2. Agregar cliente",
                        "3. Buscar cliente",
                        "4. Eliminar cliente",
                        "5. Salir"
                    )
            );

            List<Cliente> clientes = clienteService.FindAll();

            switch (option)
            {
                case "1. Mostrar clientes":

                    AnsiConsole.Clear();

                    List<Cliente> lista = null;

                    AnsiConsole.Status()
                        .Spinner(Spinner.Known.Dots)
                        .SpinnerStyle(Style.Parse("blue"))
                        .Start("Cargando clientes...", ctx =>
                        {
                            Thread.Sleep(1200); // simulación de carga
                            lista = clienteService.FindAll();
                        });

                    var table = new Table();

                    table.AddColumn("Id");
                    table.AddColumn("Nombre");
                    table.AddColumn("Apellido");
                    table.AddColumn("Teléfono");
                    table.AddColumn("Tipo");
                    table.AddColumn("Inicio");
                    table.AddColumn("Vencimiento");

                    DateTime hoy = DateTime.Today;

                    foreach (Cliente cliente in lista)
                    {
                        if (cliente.MembresiaVencimiento <= hoy)
                        {
                            AnsiConsole.MarkupLine(
                                $"[red]⚠ La membresía de {cliente.Nombre} {cliente.Apellido} ha vencido.[/]"
                            );
                        }

                        table.AddRow(
                            cliente.id.ToString(),
                            cliente.Nombre,
                            cliente.Apellido,
                            cliente.Telefono,
                            cliente.TipoMembresia,
                            cliente.MembresiaInicio.ToString("dd/MM/yyyy"),
                            cliente.MembresiaVencimiento.ToString("dd/MM/yyyy")
                        );
                    }

                    AnsiConsole.Write(table);
                    break;

                case "2. Agregar cliente":

                    AnsiConsole.Clear();
                    AnsiConsole.WriteLine();
                    AnsiConsole.Write(
                        new Panel("[cyan]Completa los datos personales del cliente[/]")
                            .Header("DATOS PERSONALES")
                            .BorderColor(Color.Cyan)
                    );

                    string nombre = AnsiConsole.Ask<string>("Nombre:");
                    string apellido = AnsiConsole.Ask<string>("Apellido:");
                    string telefono = AnsiConsole.Ask<string>("Teléfono:");

                    

                    AnsiConsole.Write(
                        new Panel("[yellow]⚠ AVISO: En este gimnasio solo se permiten clientes mayores de 15 años en adelante.[/]")
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
                        AnsiConsole.MarkupLine("[red]Error: No es posible registrar clientes menores de 15 años.[/]");
                        break;
                    }
                    

                    AnsiConsole.Write(
                        new Panel("[magenta]Selecciona el tipo de membresía del cliente[/]")
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

                    int maxId = clientes.Count > 0 ? clientes.Max(c => c.id) : 0;

                    Cliente nuevoCliente = new Cliente
                    {
                        id = maxId + 1,
                        Nombre = nombre,
                        Apellido = apellido,
                        Telefono = telefono,
                        FechaNacimiento = fechaNacimiento,
                        TipoMembresia = tipo,
                        MembresiaInicio = membresiaInicio,
                        MembresiaVencimiento = membresiaVencimiento
                    };
                    AnsiConsole.Clear();

                    bool creado = false;

                    AnsiConsole.Status()
                        .Spinner(Spinner.Known.Star)
                        .SpinnerStyle(Style.Parse("green"))
                        .Start("Registrando cliente...", ctx =>
                        {
                            Thread.Sleep(2000); // simulación de proceso
                            creado = clienteService.Create(nuevoCliente);
                        });

                    if (creado)
                        AnsiConsole.MarkupLine("[green]Cliente agregado correctamente.[/]");
                    else
                        AnsiConsole.MarkupLine("[red]Error al agregar cliente.[/]");

                    break;

                case "4. Eliminar cliente":

                    AnsiConsole.Clear();

                    int id = AnsiConsole.Ask<int>("Indica el ID del cliente:");

                    AnsiConsole.Clear();

                    var confirmado = AnsiConsole.Confirm($"¿Estás seguro de que deseas eliminar el cliente con el ID {id}?");

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
                        .Start("Eliminando cliente...", ctx =>
                        {
                            Thread.Sleep(2000);
                            eliminado = clienteService.Delete(id);
                        });

                    if (eliminado)
                        AnsiConsole.MarkupLine($"[green]✔ Cliente con ID {id} eliminado correctamente.[/]");
                    else
                        AnsiConsole.MarkupLine($"[red]❌ Cliente con ID {id} no encontrado.[/]");

                    break;
                case "3. Buscar cliente":

                    AnsiConsole.Clear();

                    int idBuscar = AnsiConsole.Ask<int>("Ingrese el ID del cliente:");

                    Cliente? clienteEncontrado = null;


                    AnsiConsole.Clear();
                    AnsiConsole.Status()
                        .Spinner(Spinner.Known.Dots)
                        .SpinnerStyle(Style.Parse("green"))
                        .Start("Buscando cliente...", ctx =>
                        {
                            Thread.Sleep(2000); // simulación de proceso
                            clienteEncontrado = clienteService.FindById(idBuscar);
                        });

                    if (clienteEncontrado == null)
                    {
                        AnsiConsole.Clear();
                        AnsiConsole.MarkupLine($"[red] Cliente con ID {idBuscar} no encontrado.[/]");
                    }
                    else
                    {
                        AnsiConsole.Clear();
                        AnsiConsole.MarkupLine("[green] Cliente encontrado[/]");
                        AnsiConsole.WriteLine();
                        var tabla = new Table();

                        tabla.AddColumn("Id");
                        tabla.AddColumn("Nombre");
                        tabla.AddColumn("Apellido");
                        tabla.AddColumn("Teléfono");
                        tabla.AddColumn("Tipo");
                        tabla.AddColumn("Inicio");
                        tabla.AddColumn("Vencimiento");

                        tabla.AddRow(
                            clienteEncontrado.id.ToString(),
                            clienteEncontrado.Nombre,
                            clienteEncontrado.Apellido,
                            clienteEncontrado.Telefono,
                            clienteEncontrado.TipoMembresia,
                            clienteEncontrado.MembresiaInicio.ToShortDateString(),
                            clienteEncontrado.MembresiaVencimiento.ToShortDateString()
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



