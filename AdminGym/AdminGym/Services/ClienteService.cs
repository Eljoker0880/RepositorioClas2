
namespace AdminGym.Services
{
    public class ClienteService
    {
        private readonly List<Cliente> _clientes =
        [
            new Cliente
            {
                id = 1,
                Nombre = "Demo",
                Apellido = "Demo",
                Telefono = "123456789",
                FechaNacimiento = new DateTime(1990, 1, 1),
                TipoMembresia = "1 Mes",
                MembresiaInicio = new DateTime(2026, 7, 1),
                MembresiaVencimiento = new DateTime(2026, 8, 1),
            }
        ];

        public List<Cliente> FindAll()
        {
            return _clientes;
        }

        public bool Create(Cliente cliente)
        {
            _clientes.Add(cliente);
            return true;
        }

        public bool Delete(int id)
        {
            Cliente? cliente = _clientes.FirstOrDefault(c => c.id == id);

            if (cliente == null)
            {
                return false;
            }


            _clientes.Remove(cliente);
            return true;
        }
        public Cliente? FindById(int id)
        {
            return _clientes.FirstOrDefault(c => c.id == id);
        }
    }
}
















