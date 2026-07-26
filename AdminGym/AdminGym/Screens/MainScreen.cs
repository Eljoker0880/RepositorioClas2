using AdminGym.Models;
using AdminGym.Services;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace AdminGym.Screens;


public class MainScreen(MiembroService miembroService, MembresiaService membresiaService)
{
    private void ActualizarMiembro()
    {
        LimpiarConsola();

        int id = AnsiConsole.Ask<int>(
            "Ingrese el ID del miembro:"
        );

        Miembro? miembro = _miembroService.FindById(id);

        if (miembro == null)
        {
            AnsiConsole.MarkupLine(
                "[red]Miembro no encontrado.[/]"
            );
            return;
        }

        AnsiConsole.MarkupLine(
            $"Nombre actual: [yellow]{miembro.Nombre}[/]"
        );

        AnsiConsole.MarkupLine(
            $"Apellido actual: [yellow]{miembro.Apellido}[/]"
        );

        AnsiConsole.MarkupLine(
            $"Teléfono actual: [yellow]{miembro.Telefono}[/]"
        );

        miembro.Nombre = AnsiConsole.Ask<string>(
            "Nuevo nombre:"
        );

        miembro.Apellido = AnsiConsole.Ask<string>(
            "Nuevo apellido:"
        );

        miembro.Telefono = AnsiConsole.Ask<string>(
            "Nuevo teléfono:"
        );

        bool actualizado = _miembroService.Update(miembro);

        if (actualizado)
        {
            AnsiConsole.MarkupLine(
                "[green]Miembro actualizado correctamente.[/]"
            );
        }
        else
        {
            AnsiConsole.MarkupLine(
                "[red]Error al actualizar.[/]"
            );
        }
    }
    private readonly MiembroService _miembroService = miembroService;
    private readonly MembresiaService _membresiaService = membresiaService;
    private bool running = true;

    private readonly string[] opcionesMenu =
    [
        "1. Agregar miembro",
    "2. Buscar miembro",
    "3. Mostrar inactivos",
    "4. Actualizar miembro",
    "5. Eliminar",
    "6. Salir"
    ];

    public void Show()
    {
        while (running)
        {
            string opcion = MostrarMenuSeleccion();

            switch (opcion)
            {
                case "1. Agregar miembro":
                    AgregarMiembro();
                    break;

                case "2. Buscar miembro":
                    BuscarMiembro();
                    break;

                case "3. Mostrar inactivos":
                    MostrarMiembrosInactivos();
                    break;

                case "4. Actualizar miembro":
                    ActualizarMiembro();
                    break;

                case "5. Eliminar":
                    EliminarMiembro();
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
                LimpiarConsola();
            }
        }
    }

    private static void LimpiarConsola()
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

    private IRenderable CrearTablaMenuResumen(int? limite = null)
    {
        var table = new Table();
        table.Border(TableBorder.Rounded);

        table.AddColumn(new TableColumn("[blue]Td[/]").Width(4).NoWrap());
        table.AddColumn(new TableColumn("[blue]Nombre[/]").Width(10).NoWrap());
        table.AddColumn(new TableColumn("[blue]Apellido[/]").Width(10).NoWrap());
        table.AddColumn(new TableColumn("[blue]Teléfono[/]").Width(12).NoWrap());
        table.AddColumn(new TableColumn("[blue]Tipo[/]").Width(13).NoWrap());
        table.AddColumn(new TableColumn("[blue]Estado[/]").Width(22).NoWrap());

        List<Miembro> miembros = _miembroService.FindAll();
        if (miembros.Count == 0)
        {
            return new Panel(
                "[yellow]Aún no se han registrado miembros.[/]"
            )
            
            .BorderColor(Color.Blue);
        }


        if (limite.HasValue)
            miembros = miembros.Take(limite.Value).ToList();

        foreach (Miembro miembro in miembros)
        {
            Membresia? membresia = _membresiaService.FindByMiembroId(miembro.id);

            table.AddRow(
                            miembro.id.ToString(),
                            miembro.Nombre,
                            miembro.Apellido,
                            miembro.Telefono,
                            membresia?.Tipo ?? "-",
                            _membresiaService.CalcularEstado(membresia)
                        );
        }

        return table;
    }

    private void MostrarMenuPrincipal(int seleccionado)
    {
        AnsiConsole.Write(
            new FigletText("ADMINGYM")
                .Centered()
                .Color(Color.Red));

        var columnas = new Columns(
            RenderPanelOpciones(seleccionado),
            new Panel(CrearTablaMenuResumen())
                .Header("[blue]MIEMBROS[/]")
                .BorderColor(Color.Blue)
        );

        AnsiConsole.Write(columnas);
    }

    private Panel RenderPanelOpciones(int seleccionado)
    {
        var lineas = new List<string>();

        for (int i = 0; i < opcionesMenu.Length; i++)
        {
            if (i == seleccionado)
                lineas.Add($"[red]➤ {opcionesMenu[i]}[/]");
            else
                lineas.Add($"  {opcionesMenu[i]}");
        }

        var contenido = new Markup(string.Join("\n", lineas));

        var panel = new Panel(contenido)
            .Header("[red]MENÚ[/]")
            .BorderColor(Color.Red);

        panel.Width = 30;

        return panel;
    }

    private string MostrarMenuSeleccion()
    {
        int seleccionado = 0;

        while (true)
        {
            LimpiarConsola();
            MostrarMenuPrincipal(seleccionado);

            var tecla = Console.ReadKey(true).Key;

            if (tecla == ConsoleKey.UpArrow)
                seleccionado = (seleccionado - 1 + opcionesMenu.Length) % opcionesMenu.Length;
            else if (tecla == ConsoleKey.DownArrow)
                seleccionado = (seleccionado + 1) % opcionesMenu.Length;
            else if (tecla == ConsoleKey.Enter)
                return opcionesMenu[seleccionado];
        }
    }

    private void AgregarMiembro()
    {
        List<Miembro> miembros = _miembroService.FindAll();
        LimpiarConsola();
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
            LimpiarConsola();
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
                .AddChoices("1 Día", "1 Semana", "1 Mes")
        );

        DateTime inscripcion = DateTime.Now;
        DateTime vencimiento = _membresiaService.CalcularVencimiento(tipo, inscripcion);



        Miembro nuevoMiembro = new Miembro
        {

            Nombre = nombre,
            Apellido = apellido,
            Telefono = telefono,
            Fecha = fechaNacimiento
        };

        Membresia nuevaMembresia = new Membresia
        {

            Tipo = tipo,
            Inscripcion = inscripcion,
            Vencimiento = vencimiento,
            id_miembro = nuevoMiembro.id
        };

        LimpiarConsola();

        var resumen = new Table();
        resumen.Border(TableBorder.Rounded);
        resumen.AddColumn("[cyan]Campo[/]");
        resumen.AddColumn("[cyan]Valor[/]");
        resumen.AddRow("Nombre", $"{nombre} {apellido}");
        resumen.AddRow("Teléfono", telefono);
        resumen.AddRow("Fecha de nacimiento", fechaNacimiento.ToString("dd/MM/yyyy"));
        resumen.AddRow("Tipo de membresía", tipo);
        resumen.AddRow("Vencimiento", vencimiento.ToString("dd/MM/yyyy"));

        AnsiConsole.Write(
            new Panel(resumen)
                .Header("[cyan]RESUMEN DEL NUEVO MIEMBRO[/]")
                .BorderColor(Color.Cyan)
        );

        bool confirmarAgregar = AnsiConsole.Confirm("¿Deseas agregar este miembro?");

        if (!confirmarAgregar)
        {
            LimpiarConsola();
            AnsiConsole.MarkupLine("[yellow]⚠ Registro cancelado. No se agregó ningún miembro.[/]");
            return;
        }

        LimpiarConsola();

        bool creado = false;

        AnsiConsole.Status()
            .Spinner(Spinner.Known.Star)
            .SpinnerStyle(Style.Parse("green"))
            .Start("Registrando miembro...", ctx =>
            {
                Thread.Sleep(2000);
                int idGenerado = _miembroService.Create(nuevoMiembro);

                nuevaMembresia.id_miembro = idGenerado;

                bool membresiaCreada = _membresiaService.Create(nuevaMembresia);

                creado = membresiaCreada;
            });

        if (creado)
            AnsiConsole.MarkupLine("[green]Miembro agregado correctamente.[/]");
        else
            AnsiConsole.MarkupLine("[red]Error al agregar el miembro.[/]");
    }

    private void BuscarMiembro()
    {
        LimpiarConsola();

        int idBuscar = AnsiConsole.Ask<int>("Ingrese el ID del miembro:");

        Miembro? miembroEncontrado = null;

        LimpiarConsola();
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse("green"))
            .Start("Buscando miembro...", ctx =>
            {
                Thread.Sleep(2000);
                miembroEncontrado = _miembroService.FindById(idBuscar);
            });

        if (miembroEncontrado == null)
        {
            LimpiarConsola();
            AnsiConsole.MarkupLine($"[red]Miembro con ID {idBuscar} no encontrado.[/]");
            return;
        }

        LimpiarConsola();
        AnsiConsole.MarkupLine("[green]Miembro encontrado[/]");
        AnsiConsole.WriteLine();

        var tablaBusqueda = new Table();
        tablaBusqueda.Border(TableBorder.Rounded);

        tablaBusqueda.AddColumn("[green]Id[/]");
        tablaBusqueda.AddColumn("[green]Nombre[/]");
        tablaBusqueda.AddColumn("[green]Apellido[/]");
        tablaBusqueda.AddColumn("[green]Teléfono[/]");
        tablaBusqueda.AddColumn("[green]Tipo[/]");
        tablaBusqueda.AddColumn("[green]Inicio[/]");
        tablaBusqueda.AddColumn("[green]Vencimiento[/]");
        tablaBusqueda.AddColumn("[green]Estado[/]");

        Membresia? membresia = _membresiaService.FindByMiembroId(miembroEncontrado.id);

        tablaBusqueda.AddRow(
            miembroEncontrado.id.ToString(),
            miembroEncontrado.Nombre,
            miembroEncontrado.Apellido,
            miembroEncontrado.Telefono,
            membresia?.Tipo ?? "Sin membresía",
            membresia?.Inscripcion.ToString("dd/MM/yyyy") ?? "-",
            membresia?.Vencimiento.ToString("dd/MM/yyyy") ?? "-",
            _membresiaService.CalcularEstado(membresia)
        );

        AnsiConsole.Write(tablaBusqueda);
    }

    private void MostrarMiembrosInactivos()
    {
        LimpiarConsola();

        var tabla = new Table();
        tabla.Border(TableBorder.Rounded);
        tabla.Expand();
        tabla.Title("[yellow]Miembros[/]");

        tabla.AddColumn("[yellow]ID[/]");
        tabla.AddColumn("[yellow]Nombre[/]");
        tabla.AddColumn("[yellow]Apellido[/]");
        tabla.AddColumn("[yellow]Tipo[/]");
        tabla.AddColumn("[yellow]Estado[/]");

        bool hayVencidos = false;

        foreach (Miembro miembro in _miembroService.FindAll())
        {
            Membresia? membresia = _membresiaService.FindByMiembroId(miembro.id);

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
            LimpiarConsola();
            AnsiConsole.Write(
                new Panel("[green]No existen miembros con membresías vencidas.[/]")
                    .Header("[green]INFORMACIÓN[/]")
                    .BorderColor(Color.Green)
            );
            return;
        }

        AnsiConsole.Write(tabla);

        string accion = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\n[yellow]¿Qué deseas hacer?[/]")
                .AddChoices("Reinscribir miembro", "Eliminar miembro", "Volver al menú")
        );

        if (accion == "Volver al menú")
            return;

        if (accion == "Eliminar miembro")
        {
            EliminarMiembroVencido();
            return;
        }

        ReinscribirMiembro();
    }

    private void EliminarMiembroVencido()
    {
        bool salirEliminar = false;

        while (!salirEliminar)
        {
            LimpiarConsola();

            var tablaEliminar = new Table();
            tablaEliminar.Border(TableBorder.Rounded);

            tablaEliminar.AddColumn("[yellow]ID[/]");
            tablaEliminar.AddColumn("[yellow]Nombre[/]");
            tablaEliminar.AddColumn("[yellow]Apellido[/]");
            tablaEliminar.AddColumn("[yellow]Tipo[/]");
            tablaEliminar.AddColumn("[yellow]Estado[/]");

            foreach (Miembro m in _miembroService.FindAll())
            {
                Membresia? mem = _membresiaService.FindByMiembroId(m.id);

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

            int idEliminar = AnsiConsole.Ask<int>("\nIngrese el ID del miembro que desea eliminar:");
            Miembro? miembroEliminar = _miembroService.FindById(idEliminar);

            if (miembroEliminar == null)
            {
                string opcionmenu = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[red]❌ El miembro no existe.[/]\n\n¿Qué deseas hacer?")
                        .AddChoices("Volver a intentar", "Volver al menú"));

                if (opcionmenu == "Volver al menú")
                    salirEliminar = true;

                continue;
            }

            Membresia? membresiaEliminar = _membresiaService.FindByMiembroId(miembroEliminar.id);

            if (membresiaEliminar != null && membresiaEliminar.Vencimiento > DateTime.Now)
            {
                string opcionmenu = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]⚠ Este miembro aún tiene una membresía vigente.[/]\n\n¿Qué deseas hacer?")
                        .AddChoices("Volver a intentar", "Volver al menú"));

                if (opcionmenu == "Volver al menú")
                    salirEliminar = true;

                continue;
            }

            bool confirmar = AnsiConsole.Confirm(
                $"¿Estás seguro de eliminar a {miembroEliminar.Nombre} {miembroEliminar.Apellido}?");

            if (!confirmar)
                continue;

            if (membresiaEliminar != null)
            {
                _membresiaService.Delete(membresiaEliminar.id);
            }

            bool miembroEliminado = _miembroService.Delete(idEliminar);

            if (miembroEliminado)
            {
                LimpiarConsola();
                AnsiConsole.MarkupLine($"[green]✔ Miembro con ID {idEliminar} eliminado correctamente.[/]");
            }

            salirEliminar = true;
        }
    }

    private void ReinscribirMiembro()
    {
        bool salirReinscripcion = false;

        while (!salirReinscripcion)
        {
            int idReinscribir = AnsiConsole.Ask<int>("\nIngrese el ID del miembro a reinscribir:");
            Miembro? miembro = _miembroService.FindById(idReinscribir);

            if (miembro == null)
            {
                string opcionError = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title($"[red]❌ El miembro con ID {idReinscribir} no existe.[/]\n\n¿Qué deseas hacer?")
                        .AddChoices("Volver a intentar", "Volver al menú"));

                if (opcionError == "Volver al menú")
                    salirReinscripcion = true;

                continue;
            }

            Membresia? membresia = _membresiaService.FindByMiembroId(miembro.id);

            if (membresia == null)
            {
                string opcionError = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[red]❌ Este miembro no tiene una membresía registrada.[/]\n\n¿Qué deseas hacer?")
                        .AddChoices("Volver a intentar", "Volver al menú"));

                if (opcionError == "Volver al menú")
                    salirReinscripcion = true;

                continue;
            }

            if (membresia.Vencimiento > DateTime.Now)
            {
                string opcionError = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]⚠ Este miembro todavía tiene una membresía vigente.[/]\n\n¿Qué deseas hacer?")
                        .AddChoices("Volver a intentar", "Volver al menú"));

                if (opcionError == "Volver al menú")
                    salirReinscripcion = true;

                continue;
            }

            bool confirmar = AnsiConsole.Confirm(
                $"¿Estás seguro de reinscribir a {miembro.Nombre} {miembro.Apellido}?");

            if (!confirmar)
            {
                string opcionError = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Has cancelado la reinscripción.[/]\n\n¿Qué deseas hacer?")
                        .AddChoices("Volver a intentar", "Volver al menú"));

                if (opcionError == "Volver al menú")
                    salirReinscripcion = true;

                continue;
            }

            AnsiConsole.Write(
                new Panel("[magenta]Selecciona el tipo de membresía[/]")
                    .Header("MEMBRESÍA")
                    .BorderColor(Color.Magenta)
            );

            string nuevoTipo = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .AddChoices("1 Día", "1 Semana", "1 Mes"));

            DateTime ahora = DateTime.Now;

            membresia.Tipo = nuevoTipo;
            membresia.Inscripcion = ahora;
            membresia.Vencimiento = _membresiaService.CalcularVencimiento(nuevoTipo, ahora);

            bool actualizado = _membresiaService.Update(membresia);

            LimpiarConsola();

            if (actualizado)
            {
                AnsiConsole.MarkupLine("[green]Miembro reinscrito correctamente.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Error al actualizar la membresía.[/]");
            }
            salirReinscripcion = true;
        }
    }

    private void EliminarMiembro()
    {
        LimpiarConsola();

        int id = AnsiConsole.Ask<int>("Indica el ID del miembro:");

        LimpiarConsola();

        var confirmado = AnsiConsole.Confirm($"¿Estás seguro de que deseas eliminar el miembro con el ID {id}?");

        if (!confirmado)
        {
            LimpiarConsola();
            AnsiConsole.MarkupLine("[yellow]⚠ Eliminación cancelada.[/]");
            return;
        }

        bool eliminado = false;
        Membresia? membresiaAEliminar = _membresiaService.FindByMiembroId(id);

        LimpiarConsola();
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse("green"))
.Start("Eliminando miembro...", ctx =>
{
    Thread.Sleep(2000);

    // Primero eliminamos la membresía
    if (membresiaAEliminar != null)
    {
        _membresiaService.Delete(membresiaAEliminar.id);
    }

    // Después eliminamos el miembro
    eliminado = _miembroService.Delete(id);
});

        if (eliminado)
            AnsiConsole.MarkupLine($"[green]✔ Miembro con ID {id} eliminado correctamente.[/]");
        else
            AnsiConsole.MarkupLine($"[red]❌ Miembro con ID {id} no encontrado.[/]");
    }
}
