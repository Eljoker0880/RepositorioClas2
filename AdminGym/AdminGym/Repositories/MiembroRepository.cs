using AdminGym.Models;

namespace AdminGym.Repositories;

public class MiembroRepository
{
    private readonly List<Miembro> _miembros =
    [
        new Miembro
        {
            id = 1,
            Nombre = "Demo",
            Apellido = "Demo",
            Telefono = "123456789",
            Fecha = new DateTime(1990, 1, 1)
        }
    ];

    public List<Miembro> FindAll()
    {
        return _miembros;
    }

    public bool Create(Miembro miembro)
    {
        _miembros.Add(miembro);
        return true;
    }

    public bool Delete(int id)
    {
        Miembro? miembro = _miembros.FirstOrDefault(m => m.id == id);

        if (miembro == null)
        {
            return false;
        }

        _miembros.Remove(miembro);
        return true;
    }

    public Miembro? FindById(int id)
    {
        return _miembros.FirstOrDefault(m => m.id == id);
    }
}
