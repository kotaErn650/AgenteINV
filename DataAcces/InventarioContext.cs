using AgenteINV.Models;
using Microsoft.EntityFrameworkCore;

namespace AgenteINV.DataAcces;

public class InventarioContext : DbContext
{
    public const string ConnectionString = "Server=localhost;Database=AgenteINVDb;Trusted_Connection=True;TrustServerCertificate=True;";

    public InventarioContext()
    {
    }

    public InventarioContext(DbContextOptions<InventarioContext> options)
        : base(options)
    {
    }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nombre).HasMaxLength(100).IsRequired();

            entity.HasData(
                new Categoria { Id = 1, Nombre = "Laptops" },
                new Categoria { Id = 2, Nombre = "Smartphones" },
                new Categoria { Id = 3, Nombre = "Accesorios" });
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Nombre).HasMaxLength(150).IsRequired();
            entity.Property(p => p.Contacto).HasMaxLength(120).IsRequired();
            entity.Property(p => p.Direccion).HasMaxLength(200).IsRequired();

            entity.HasData(
                new Proveedor { Id = 1, Nombre = "Tech Source", Contacto = "ventas@techsource.com", Direccion = "Av. Innovación 120, Madrid" },
                new Proveedor { Id = 2, Nombre = "Mobile Hub", Contacto = "compras@mobilehub.com", Direccion = "Calle Digital 45, Bogotá" },
                new Proveedor { Id = 3, Nombre = "Accesorios 360", Contacto = "info@accesorios360.com", Direccion = "Blvd. Central 88, Ciudad de México" });
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Nombre).HasMaxLength(150).IsRequired();
            entity.Property(p => p.Descripcion).HasMaxLength(500);
            entity.Property(p => p.Precio).HasColumnType("decimal(18,2)");
            entity.Property(p => p.ImagenUrl).HasMaxLength(500);

            entity.HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.Proveedor)
                .WithMany(pr => pr.Productos)
                .HasForeignKey(p => p.ProveedorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasData(
                new Producto
                {
                    Id = 1,
                    Nombre = "Laptop Pro 14",
                    Descripcion = "Portátil de alto rendimiento con 16 GB RAM y SSD de 512 GB.",
                    Precio = 1499.99m,
                    Stock = 8,
                    ImagenUrl = "dotnet_bot.png",
                    CategoriaId = 1,
                    ProveedorId = 1
                },
                new Producto
                {
                    Id = 2,
                    Nombre = "Smartphone X12",
                    Descripcion = "Teléfono 5G con cámara de 108 MP y batería de larga duración.",
                    Precio = 999.50m,
                    Stock = 15,
                    ImagenUrl = "dotnet_bot.png",
                    CategoriaId = 2,
                    ProveedorId = 2
                },
                new Producto
                {
                    Id = 3,
                    Nombre = "Auriculares Quantum",
                    Descripcion = "Auriculares inalámbricos con cancelación de ruido.",
                    Precio = 249.90m,
                    Stock = 0,
                    ImagenUrl = "dotnet_bot.png",
                    CategoriaId = 3,
                    ProveedorId = 3
                });
        });
    }
}
