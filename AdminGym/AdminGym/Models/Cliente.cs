namespace AdminGym.Models;

public class Cliente
{
    public int id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Telefono { get; set; }
    public DateTime FechaNacimiento { get; set; }

    public string TipoMembresia { get; set; }

    public DateTime MembresiaInicio { get; set; }
    public DateTime MembresiaVencimiento { get; set; }
}

