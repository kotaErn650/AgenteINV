using AgenteINV.DataAcces;
using AgenteINV.ViewModels;
using AgenteINV.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AgenteINV;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddDbContextFactory<InventarioContext>(options =>
            options.UseSqlServer(InventarioContext.ConnectionString));
        builder.Services.AddSingleton<DatabaseInitializer>();

        builder.Services.AddSingleton<ProductosViewModel>();
        builder.Services.AddTransient<ProductoDetailViewModel>();
        builder.Services.AddSingleton<CategoriasViewModel>();
        builder.Services.AddSingleton<ProveedoresViewModel>();

        builder.Services.AddSingleton<ProductosPage>();
        builder.Services.AddTransient<ProductoDetailPage>();
        builder.Services.AddSingleton<CategoriasPage>();
        builder.Services.AddSingleton<ProveedoresPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
