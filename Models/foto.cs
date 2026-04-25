using SQLite;

namespace RCA.BitacoraCamiones.Models;

public class Foto
{
    [PrimaryKey, AutoIncrement]
    public int FOT_Id { get; set; }

    public int FOT_IngresoId { get; set; }

    public string? FOT_Path { get; set; }
}