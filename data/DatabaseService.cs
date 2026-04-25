using RCA.BitacoraCamiones.Models;
using SQLite;

namespace RCA.BitacoraCamiones.Data;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;

    public async Task Init()
    {
        if (_database != null)
            return;

        try
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "trucklog.db"
            );

            _database = new SQLiteAsyncConnection(dbPath);

            await _database.CreateTableAsync<Ingreso>();
            await _database.CreateTableAsync<Foto>();
        }
        catch (Exception ex)
        {
            throw new Exception("Error inicializando DB: " + ex.Message);
        }
    }

    public SQLiteAsyncConnection GetConnection()
    {
        if (_database == null)
            throw new Exception("DB no inicializada");

        return _database;
    }
}