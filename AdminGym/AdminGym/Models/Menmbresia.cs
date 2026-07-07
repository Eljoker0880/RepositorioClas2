namespace AdminGym.Models;

public class Membresia
{
    public int id { get; set; }

    public string TipoMembresia { get; set; }

    public DateTime MembresiaInicio { get; set; }

    public DateTime MembresiaVencimiento { get; set; }

    public int MiembroId { get; set; }
}