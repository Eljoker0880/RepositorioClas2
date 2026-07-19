namespace AdminGym.Models;

public class Miembro
{
    public int id { get; set; }
    public required string Nombre { get; set; }
    public required string Apellido { get; set; }
    public required string Telefono { get; set; }
    public DateTime Fecha { get; set; }
}
