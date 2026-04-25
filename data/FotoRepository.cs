using SQLite;
using RCA.BitacoraCamiones.Models;

namespace RCA.BitacoraCamiones.Data;

public class FotoRepository
{
    private readonly SQLiteAsyncConnection _db;

    public FotoRepository(DatabaseService dbService)
    {
        _db = dbService.GetConnection();
    }

    public async Task Save(Foto foto)
    {
        await _db.InsertAsync(foto);
    }

    public async Task<List<Foto>> GetByIngreso(int ingresoId)
    {
        return await _db.Table<Foto>()
                        .Where(f => f.FOT_IngresoId == ingresoId)
                        .ToListAsync();
    }
}