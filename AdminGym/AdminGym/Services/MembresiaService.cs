namespace AdminGym.Services
{
    public class MembresiaService
    {
        private readonly List<Membresia> _membresias =
        [
            new Membresia
            {
                id = 1,
                Tipo = "1 Mes",
                Inscripcion = new DateTime(2026, 7, 1),
                Vencimiento = new DateTime(2026, 8, 1),
                id_miembro = 1
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
            return _membresias.FirstOrDefault(m => m.id_miembro == miembroId);
        }
    }
}
