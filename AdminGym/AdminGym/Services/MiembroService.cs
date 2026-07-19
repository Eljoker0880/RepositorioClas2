using AdminGym.Models;
using AdminGym.Repositories;

namespace AdminGym.Services;

public class MiembroService(MiembroRepository miembroRepository)
{
    private readonly MiembroRepository _repository = miembroRepository;

    public List<Miembro> FindAll()
    {
        return _repository.FindAll();
    }

    public bool Create(Miembro miembro)
    {
        return _repository.Create(miembro);
    }

    public bool Delete(int id)
    {
        return _repository.Delete(id);
    }

    public Miembro? FindById(int id)
    {
        return _repository.FindById(id);
    }
}
