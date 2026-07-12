namespace AdminGym.Models;


public class Membresia
{
    public int id { get; set; }

    public required string Tipo { get; set; }

    public DateTime Inscripcion { get; set; }

    public DateTime Vencimiento { get; set; }

    public int id_miembro { get; set; }
}