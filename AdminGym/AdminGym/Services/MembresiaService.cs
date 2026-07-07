namespace AdminGym.Services
{
    public class MembresiaService
    {
        private readonly List<Membresia> _membresias =
        [
            new Membresia
            {
                id = 1,
                TipoMembresia = "1 Mes",
                MembresiaInicio = new DateTime(2026, 7, 1),
                MembresiaVencimiento = new DateTime(2026, 8, 1),
                MiembroId = 1
            }
        ];

        public List<Membresia> FindAll()
        {
            return _membresias;
        }

        public bool Create(Membresia membresia)
        {
            _membresias.Add(membresia);
            return true;
        }

        public bool Delete(int id)
        {
            Membresia? membresia = _membresias.FirstOrDefault(m => m.id == id);

            if (membresia == null)
            {
                return false;
            }

            _membresias.Remove(membresia);
            return true;
        }

        public Membresia? FindById(int id)
        {
            return _membresias.FirstOrDefault(m => m.id == id);
        }

        public Membresia? FindByMiembroId(int miembroId)
        {
            return _membresias.FirstOrDefault(m => m.MiembroId == miembroId);
        }
    }
}
