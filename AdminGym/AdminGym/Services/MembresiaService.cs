using AdminGym.Models;
using AdminGym.Repositories;

namespace AdminGym.Services;

public class MembresiaService(MembresiaRepository membresiaRepository)
{
    private readonly MembresiaRepository _repository = membresiaRepository;

    public List<Membresia> FindAll()
    {
        return _repository.FindAll();
    }

    public bool Create(Membresia membresia)
    {
        return _repository.Create(membresia);
    }

    public bool Delete(int id)
    {
        return _repository.Delete(id);
    }

    public Membresia? FindById(int id)
    {
        return _repository.FindById(id);
    }

    public Membresia? FindByMiembroId(int miembroId)
    {
        return _repository.FindByMiembroId(miembroId);
    }

    // Lógica de negocio: antes esta misma lógica estaba copiada y pegada en
    // 3 lugares distintos dentro de Program.cs (panel de inicio, tabla de
    // "Buscar miembro" y la tabla de vencidos). Ahora vive en un solo lugar.
    public string CalcularEstado(Membresia? membresia)
    {
        if (membresia == null)
            return "[grey]Sin membresía[/]";

        TimeSpan tiempoRestante = membresia.Vencimiento - DateTime.Now;

        if (tiempoRestante.TotalSeconds < 0)
            return "[red]🔴 Vencida[/]";

        double alertaSegundos = 15 * 24 * 60 * 60;

        if (membresia.Tipo == "30 Segundos")
            alertaSegundos = 10;
        else if (membresia.Tipo == "1 Día")
            alertaSegundos = 24 * 60 * 60;
        else if (membresia.Tipo == "1 Semana")
            alertaSegundos = 3 * 24 * 60 * 60;

        return tiempoRestante.TotalSeconds <= alertaSegundos
            ? "[yellow]🟡 Próxima a vencer[/]"
            : "[green]🟢 Activo[/]";
    }

    // Lógica de negocio: antes este mismo switch estaba copiado en
    // AgregarMiembro() y en ReinscribirMiembro().
    public DateTime CalcularVencimiento(string tipo, DateTime desde)
    {
        return tipo switch
        {
            "30 Segundos" => desde.AddSeconds(30),
            "1 Día" => desde.AddDays(1),
            "1 Semana" => desde.AddDays(7),
            _ => desde.AddMonths(1)
        };
    }
}
