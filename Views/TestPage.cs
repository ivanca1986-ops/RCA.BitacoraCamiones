using RCA.BitacoraCamiones.Data;
using RCA.BitacoraCamiones.Models;
using Microsoft.Maui.Storage;

namespace RCA.BitacoraCamiones;

public partial class TestPage : ContentPage
{
    private readonly DatabaseService _db;
    private IngresoRepository? _repo;
    private FotoRepository? _fotoRepo;

    public TestPage(DatabaseService db)
    {
        _db = db;

        InitializeAsync();

        Content = new VerticalStackLayout
        {
            Padding = 40,
            Children =
            {
                new Label { Text = "BITÁCORA CAMIONES", FontSize = 30 },

                // =========================
                // INSERTAR INGRESO
                // =========================
                new Button
                {
                    Text = "Insertar",
                    Command = new Command(async () =>
                    {
                        if (_repo == null)
                        {
                            await DisplayAlertAsync("Error", "DB no lista", "OK");
                            return;
                        }

                        var ingreso = new Ingreso
                        {
                            ING_Placa = "ABC123",
                            ING_Conductor = "Prueba",
                            ING_Fecha = DateTime.Now,
                            ING_SyncStatus = "Pending"
                        };

                        await _repo.Save(ingreso);

                        await DisplayAlertAsync("OK", "Ingreso guardado", "OK");
                    })
                },

                // =========================
                // VER INGRESOS
                // =========================
                new Button
                {
                    Text = "Ver",
                    Command = new Command(async () =>
                    {
                        if (_repo == null)
                        {
                            await DisplayAlertAsync("Error", "DB no lista", "OK");
                            return;
                        }

                        var lista = await _repo.GetAll();

                        string resultado = string.Join("\n",
                            lista.Select(x => $"{x.ING_Id} - {x.ING_Placa} - {x.ING_SyncStatus}")
                        );

                        await DisplayAlertAsync("Datos", resultado, "OK");
                    })
                },

                // =========================
                // TOMAR FOTO (FIX COMPLETO)
                // =========================
                new Button
                {
                    Text = "Tomar Foto",
                    Command = new Command(async () =>
                    {
                        try
                        {
                            if (_fotoRepo == null)
                            {
                                await DisplayAlertAsync("Error", "DB no lista", "OK");
                                return;
                            }

                            var foto = await MediaPicker.CapturePhotoAsync();

                            if (foto == null)
                                return;

                            // 🔥 Nombre único
                            var fileName = $"{DateTime.Now:yyyyMMddHHmmss}.jpg";

                            // 🔥 Ruta segura
                            var newFile = Path.Combine(
                                FileSystem.AppDataDirectory,
                                fileName
                            );

                            // 🔥 Guardado correcto
                            using (var stream = await foto.OpenReadAsync())
                            using (var newStream = File.Create(newFile))
                            {
                                await stream.CopyToAsync(newStream);
                            }

                            // 🔍 VALIDACIÓN
                            bool existe = File.Exists(newFile);

                            await DisplayAlertAsync("Ruta", newFile, "OK");
                            await DisplayAlertAsync("Archivo existe", existe.ToString(), "OK");

                            // 🔥 Guardar en DB
                            await _fotoRepo.Save(new Foto
                            {
                                FOT_IngresoId = 1, // temporal
                                FOT_Path = newFile
                            });

                            await DisplayAlertAsync("OK", "Foto guardada en DB", "OK");
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlertAsync("ERROR", ex.ToString(), "OK");
                        }
                    })
                }
            }
        };
    }

    // =========================
    // INIT DB
    // =========================
    private async void InitializeAsync()
    {
        try
        {
            await _db.Init();

            _repo = new IngresoRepository(_db);
            _fotoRepo = new FotoRepository(_db);

            await DisplayAlertAsync("OK", "DB lista", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error DB", ex.ToString(), "OK");
        }
    }
}