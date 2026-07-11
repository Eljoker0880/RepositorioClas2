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
    public static void MostrarMiembros()
    {
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
                TimeSpan tiempoRestante = membresia.Vencimiento - DateTime.Now;

                if (tiempoRestante.TotalSeconds < 0)
                {
                    estado = "[red]🔴 Vencida[/]";
                }
                else
                {
                    double alertaSegundos = 15 * 24 * 60 * 60; // 15 días

                    if (membresia.Tipo == "30 Segundos")
                        alertaSegundos = 10;
                    else if (membresia.Tipo == "1 Día")
                        alertaSegundos = 24 * 60 * 60;
                    else if (membresia.Tipo == "1 Semana")
                        alertaSegundos = 3 * 24 * 60 * 60;

                    if (tiempoRestante.TotalSeconds <= alertaSegundos)
                        estado = "[yellow]🟡 Próxima a vencer[/]";
                    else
                        estado = "[green]🟢 Activo[/]";
                }
            }
            string tiempo = "-";

            if (membresia != null)
            {
                TimeSpan restante = membresia.Vencimiento - DateTime.Now;

                if (restante.TotalSeconds > 0)
                {
                    tiempo = restante.ToString(@"hh\:mm\:ss");
                }
                else
                {
                    tiempo = "00:00:00";
                }
            }

            table.AddRow(
                miembro.id.ToString(),
                miembro.Nombre,
                miembro.Apellido,
                miembro.Telefono,
                membresia?.Tipo ?? "-",
                membresia?.Inscripcion.ToString("dd/MM/yyyy") ?? "-",
                tiempo,
                estado
            );
        }

        AnsiConsole.Write(table);

    }

    public static void AgregarMiembro()
    {
        List<Miembro> miembros = miembroService.FindAll();
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


            return;
        }


        AnsiConsole.Write(
            new Panel("[magenta]Selecciona el tipo de membresía[/]")
                .Header("MEMBRESÍA")
                .BorderColor(Color.Magenta)
        );

        string tipo = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .AddChoices(
                    "30 Segundos",
                    "1 Día",
                    "1 Semana",
                    "1 Mes"
                ));

        DateTime inscripcion = DateTime.Now;
        DateTime vencimiento;
        if (tipo == "30 Segundos")
            vencimiento = inscripcion.AddSeconds(30);
        else if (tipo == "1 Día")
            vencimiento = inscripcion.AddDays(1);
        else if (tipo == "1 Semana")
            vencimiento = inscripcion.AddDays(7);
        else
            vencimiento = inscripcion.AddMonths(1);

        int maxId = miembros.Count > 0 ? miembros.Max(c => c.id) : 0;

        Miembro nuevoMiembro = new Miembro
        {
            id = maxId + 1,
            Nombre = nombre,
            Apellido = apellido,
            Telefono = telefono,
            Fecha = fechaNacimiento
        };
        Membresia nuevaMembresia = new Membresia
        {
            id = maxId + 1,
            Tipo = tipo,
            Inscripcion = inscripcion,
            Vencimiento = vencimiento,
            id_miembro = nuevoMiembro.id
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
    "4. Reinscribir Miembro",
    "5. Eliminar Miembro",
    "6. Salir"
)
            );

            List<Miembro> miembros = miembroService.FindAll();

            switch (option)
            {
                case "1. Mostrar Miembros":

                    MostrarMiembros();

                    break;
                    

                case "2. Agregar Miembro":

                    AgregarMiembro();

                    break;

                case "4. Reinscribir Miembro":

                    AnsiConsole.Clear();

                    var tabla = new Table();

                    tabla.AddColumn("ID");
                    tabla.AddColumn("Nombre");
                    tabla.AddColumn("Apellido");
                    tabla.AddColumn("Tipo");
                    tabla.AddColumn("Estado");

                    bool hayVencidos = false;

                    foreach (Miembro miembro in miembroService.FindAll())
                    {
                        Membresia? membresia = membresiaService.FindByMiembroId(miembro.id);

                        if (membresia != null && membresia.Vencimiento <= DateTime.Now)
                        {
                            hayVencidos = true;

                            tabla.AddRow(
                                miembro.id.ToString(),
                                miembro.Nombre,
                                miembro.Apellido,
                                membresia.Tipo,
                                "[red]🔴 Vencida[/]"
                            );
                        }
                    }

                    if (!hayVencidos)
                    {
                        AnsiConsole.Clear();

                        AnsiConsole.Write(
                            new Panel(
                                "[green]No existen miembros con membresías vencidas.[/]"
                            )
                            .Header("[green]INFORMACIÓN[/]")
                            .BorderColor(Color.Green)
                        );


                        break;
                    }

                    AnsiConsole.Write(tabla);
                    string accion = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("\n[yellow]¿Qué deseas hacer?[/]")
        .AddChoices(
            "Reinscribir miembro",
            "Eliminar miembro",
            "Volver al menú"
        )
);
                    if (accion == "Volver al menú")
                    {
                        break;
                    }
                    if (accion == "Eliminar miembro")
                    {
                        bool salirEliminar = false;

                        while (!salirEliminar)
                        {
                            AnsiConsole.Clear();

                            var tablaEliminar = new Table();

                            tablaEliminar.AddColumn("ID");
                            tablaEliminar.AddColumn("Nombre");
                            tablaEliminar.AddColumn("Apellido");
                            tablaEliminar.AddColumn("Tipo");
                            tablaEliminar.AddColumn("Estado");

                            foreach (Miembro m in miembroService.FindAll())
                            {
                                Membresia? mem = membresiaService.FindByMiembroId(m.id);

                                if (mem != null && mem.Vencimiento <= DateTime.Now)
                                {
                                    tablaEliminar.AddRow(
                                        m.id.ToString(),
                                        m.Nombre,
                                        m.Apellido,
                                        mem.Tipo,
                                        "[red]🔴 Vencida[/]"
                                    );
                                }
                            }

                            AnsiConsole.Write(tablaEliminar);

                            int idEliminar = AnsiConsole.Ask<int>(
                                "\nIngrese el ID del miembro que desea eliminar:");

                            Miembro? miembroEliminar = miembroService.FindById(idEliminar);

                            if (miembroEliminar == null)
                            {
                                string opcion = AnsiConsole.Prompt(
                                    new SelectionPrompt<string>()
                                        .Title("[red]❌ El miembro no existe.[/]\n\n¿Qué deseas hacer?")
                                        .AddChoices(
                                            "Volver a intentar",
                                            "Volver al menú"
                                        ));

                                if (opcion == "Volver al menú")
                                {
                                    salirEliminar = true;
                                }

                                continue;
                            }

                            Membresia? membresiaEliminar =
                                membresiaService.FindByMiembroId(miembroEliminar.id);

                            if (membresiaEliminar != null &&
                                membresiaEliminar.Vencimiento > DateTime.Now)
                            {
                                string opcion = AnsiConsole.Prompt(
                                    new SelectionPrompt<string>()
                                        .Title("[yellow]⚠ Este miembro aún tiene una membresía vigente.[/]\n\n¿Qué deseas hacer?")
                                        .AddChoices(
                                            "Volver a intentar",
                                            "Volver al menú"
                                        ));

                                if (opcion == "Volver al menú")
                                {
                                    salirEliminar = true;
                                }

                                continue;
                            }

                            bool confirmar = AnsiConsole.Confirm(
                                $"¿Estás seguro de eliminar a {miembroEliminar.Nombre} {miembroEliminar.Apellido}?");

                            if (!confirmar)
                            {
                                continue;
                            }

                            bool miembroEliminado = miembroService.Delete(idEliminar);

                            if (miembroEliminado)
                            {
                                AnsiConsole.Clear();

                                AnsiConsole.MarkupLine($"[green]✔ Miembro con ID {idEliminar} eliminado correctamente.[/]");

                            }

                            salirEliminar = true;
                        }

                        break;
                    }
                    bool salirReinscripcion = false;

                        while (!salirReinscripcion)
                        {



                            int idReinscribir = AnsiConsole.Ask<int>(
                                "\nIngrese el ID del miembro a reinscribir:");

                            Miembro? miembro = miembroService.FindById(idReinscribir);

                            if (miembro == null)
                            {
                                string opcionError = AnsiConsole.Prompt(
                                    new SelectionPrompt<string>()
                                        .Title($"[red]❌ El miembro con ID {idReinscribir} no existe.[/]\n\n¿Qué deseas hacer?")
                                        .AddChoices(
                                            "Volver a intentar",
                                            "Volver al menú"
                                        ));

                                if (opcionError == "Volver al menú")
                                {
                                    salirReinscripcion = true;
                                }

                                continue;
                            }

                            Membresia? membresia = membresiaService.FindByMiembroId(miembro.id);

                            if (membresia == null)
                            {
                                string opcionError = AnsiConsole.Prompt(
                                    new SelectionPrompt<string>()
                                        .Title("[red]❌ Este miembro no tiene una membresía registrada.[/]\n\n¿Qué deseas hacer?")
                                        .AddChoices(
                                            "Volver a intentar",
                                            "Volver al menú"
                                        ));

                                if (opcionError == "Volver al menú")
                                {
                                    salirReinscripcion = true;
                                }

                                continue;
                            }

                            if (membresia.Vencimiento > DateTime.Now)
                            {
                                string opcionError = AnsiConsole.Prompt(
                                    new SelectionPrompt<string>()
                                        .Title("[yellow]⚠ Este miembro todavía tiene una membresía vigente.[/]\n\n¿Qué deseas hacer?")
                                        .AddChoices(
                                            "Volver a intentar",
                                            "Volver al menú"
                                        ));

                                if (opcionError == "Volver al menú")
                                {
                                    salirReinscripcion = true;
                                }

                                continue;
                            }
                        bool confirmar = AnsiConsole.Confirm(
$"¿Estás seguro de reinscribir a {miembro.Nombre} {miembro.Apellido}?");

                        if (!confirmar)
                            {
                                continue;
                            }



                        AnsiConsole.Write(
                            new Panel("[magenta]Selecciona el tipo de membresía[/]")
                                .Header("MEMBRESÍA")
                                .BorderColor(Color.Magenta)
                        );

                        string nuevoTipo = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .AddChoices(
                                    "30 Segundos",
                                    "1 Día",
                                    "1 Semana",
                                    "1 Mes"
                                ));

                        membresia.Tipo = nuevoTipo;
                            membresia.Inscripcion = DateTime.Now;

                            if (nuevoTipo == "30 Segundos")
                            {
                                membresia.Vencimiento = DateTime.Now.AddSeconds(30);
                            }
                            else if (nuevoTipo == "1 Día")
                            {
                                membresia.Vencimiento = DateTime.Now.AddDays(1);
                            }
                            else if (nuevoTipo == "1 Semana")
                            {
                                membresia.Vencimiento = DateTime.Now.AddDays(7);
                            }
                            else
                            {
                                membresia.Vencimiento = DateTime.Now.AddMonths(1);
                            }

                            AnsiConsole.Clear();

                        AnsiConsole.MarkupLine("[green]Miembro agregado correctamente.[/]");


                        break;

                    }

                    break;

                case "5. Eliminar Miembro":

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
                        var tablaBusqueda = new Table();

                        tablaBusqueda.AddColumn("Id");
                        tablaBusqueda.AddColumn("Nombre");
                        tablaBusqueda.AddColumn("Apellido");
                        tablaBusqueda.AddColumn("Teléfono");
                        tablaBusqueda.AddColumn("Tipo");
                        tablaBusqueda.AddColumn("Inicio");
                        tablaBusqueda.AddColumn("Vencimiento");
                        tablaBusqueda.AddColumn("Estado");

                        Membresia? membresia = membresiaService.FindByMiembroId(miembroEncontrado.id);
                        string estado = "[grey]Sin membresía[/]";

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

                                if (tiempoRestante.TotalSeconds <= alertaSegundos)
                                    estado = "[yellow]🟡 Próxima a vencer[/]";
                                else
                                    estado = "[green]🟢 Activo[/]";
                            }
                        }

                        tablaBusqueda.AddRow(
                            miembroEncontrado.id.ToString(),
                            miembroEncontrado.Nombre,
                            miembroEncontrado.Apellido,
                            miembroEncontrado.Telefono,
                            membresia?.Tipo ?? "Sin membresía",
                            membresia?.Inscripcion.ToString("dd/MM/yyyy") ?? "-",
                            membresia?.Vencimiento.ToString("dd/MM/yyyy") ?? "-",
                            estado
                        );

                        AnsiConsole.Write(tablaBusqueda);
                    }

                    break;

                case "6. Salir":

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



