using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AgenteINV.DataAcces;

public class DatabaseInitializer
{
    private readonly IDbContextFactory<InventarioContext> _contextFactory;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(IDbContextFactory<InventarioContext> contextFactory, ILogger<DatabaseInitializer> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            await context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No fue posible inicializar la base de datos de inventario.");
        }
    }
}
