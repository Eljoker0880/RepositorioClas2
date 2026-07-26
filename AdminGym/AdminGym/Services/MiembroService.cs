using AdminGym.Models;
using AdminGym.Repositories;

namespace AdminGym.Services;

public class MiembroService(MiembroRepository miembroRepository)
{
    private readonly MiembroRepository _repository = miembroRepository;

    public bool Update(Miembro miembro)
    {
        return _repository.Update(miembro);
    }

    public List<Miembro> FindAll()
    {
        return _repository.FindAll();
    }

    public int Create(Miembro miembro)
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