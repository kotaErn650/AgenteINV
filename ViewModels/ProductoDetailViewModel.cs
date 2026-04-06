using AgenteINV.DataAcces;
using AgenteINV.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AgenteINV.ViewModels;

public class ProductoDetailViewModel : BaseViewModel
{
    private readonly IDbContextFactory<InventarioContext> _contextFactory;
    private int _productoId;
    private Categoria? _selectedCategoria;
    private Proveedor? _selectedProveedor;
    private Producto _currentProduct = new();

    public ProductoDetailViewModel(IDbContextFactory<InventarioContext> contextFactory)
    {
        _contextFactory = contextFactory;
        Categorias = [];
        Proveedores = [];
        SaveCommand = new Command(async () => await SaveAsync(), () => !IsBusy);
        DeleteCommand = new Command(async () => await DeleteAsync(), () => !IsBusy && IsEditMode);
        CancelCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        ResetForNew();
    }

    public ObservableCollection<Categoria> Categorias { get; }

    public ObservableCollection<Proveedor> Proveedores { get; }

    public Producto CurrentProduct
    {
        get => _currentProduct;
        private set
        {
            if (SetProperty(ref _currentProduct, value))
            {
                OnPropertyChanged(nameof(PageTitle));
                OnPropertyChanged(nameof(IsEditMode));
            }
        }
    }

    public Categoria? SelectedCategoria
    {
        get => _selectedCategoria;
        set
        {
            if (SetProperty(ref _selectedCategoria, value) && value is not null)
            {
                CurrentProduct.CategoriaId = value.Id;
            }
        }
    }

    public Proveedor? SelectedProveedor
    {
        get => _selectedProveedor;
        set
        {
            if (SetProperty(ref _selectedProveedor, value) && value is not null)
            {
                CurrentProduct.ProveedorId = value.Id;
            }
        }
    }

    public ICommand SaveCommand { get; }

    public ICommand DeleteCommand { get; }

    public ICommand CancelCommand { get; }

    public bool IsEditMode => _productoId > 0;

    public string PageTitle => IsEditMode ? "Editar producto" : "Nuevo producto";

    public async Task InitializeAsync(int? productoId = null)
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            await LoadCatalogsAsync();

            if (productoId.GetValueOrDefault() > 0)
            {
                _productoId = productoId!.Value;
                await using var context = await _contextFactory.CreateDbContextAsync();
                var producto = await context.Productos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == _productoId);
                CurrentProduct = producto is null
                    ? new Producto { ImagenUrl = "dotnet_bot.png" }
                    : new Producto
                    {
                        Id = producto.Id,
                        Nombre = producto.Nombre,
                        Descripcion = producto.Descripcion,
                        Precio = producto.Precio,
                        Stock = producto.Stock,
                        ImagenUrl = producto.ImagenUrl,
                        CategoriaId = producto.CategoriaId,
                        ProveedorId = producto.ProveedorId
                    };
            }
            else
            {
                ResetForNew();
            }

            SelectedCategoria = Categorias.FirstOrDefault(c => c.Id == CurrentProduct.CategoriaId) ?? Categorias.FirstOrDefault();
            SelectedProveedor = Proveedores.FirstOrDefault(p => p.Id == CurrentProduct.ProveedorId) ?? Proveedores.FirstOrDefault();
            OnPropertyChanged(nameof(PageTitle));
            OnPropertyChanged(nameof(IsEditMode));
            ((Command)DeleteCommand).ChangeCanExecute();
        }
        finally
        {
            IsBusy = false;
            ((Command)SaveCommand).ChangeCanExecute();
        }
    }

    public void ResetForNew()
    {
        _productoId = 0;
        CurrentProduct = new Producto
        {
            ImagenUrl = "dotnet_bot.png",
            Precio = 0m,
            Stock = 0
        };
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(IsEditMode));
        ((Command)DeleteCommand).ChangeCanExecute();
    }

    private async Task LoadCatalogsAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var categorias = await context.Categorias.AsNoTracking().OrderBy(c => c.Nombre).ToListAsync();
        var proveedores = await context.Proveedores.AsNoTracking().OrderBy(p => p.Nombre).ToListAsync();

        SyncCollection(Categorias, categorias);
        SyncCollection(Proveedores, proveedores);
    }

    private async Task SaveAsync()
    {
        if (IsBusy || string.IsNullOrWhiteSpace(CurrentProduct.Nombre) || SelectedCategoria is null || SelectedProveedor is null)
        {
            return;
        }

        try
        {
            IsBusy = true;
            await using var context = await _contextFactory.CreateDbContextAsync();

            Producto entity;
            if (IsEditMode)
            {
                entity = await context.Productos.FirstAsync(p => p.Id == _productoId);
            }
            else
            {
                entity = new Producto();
                await context.Productos.AddAsync(entity);
            }

            entity.Nombre = CurrentProduct.Nombre.Trim();
            entity.Descripcion = CurrentProduct.Descripcion?.Trim() ?? string.Empty;
            entity.Precio = CurrentProduct.Precio;
            entity.Stock = CurrentProduct.Stock;
            entity.ImagenUrl = string.IsNullOrWhiteSpace(CurrentProduct.ImagenUrl) ? "dotnet_bot.png" : CurrentProduct.ImagenUrl.Trim();
            entity.CategoriaId = SelectedCategoria.Id;
            entity.ProveedorId = SelectedProveedor.Id;

            await context.SaveChangesAsync();
            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsBusy = false;
            ((Command)SaveCommand).ChangeCanExecute();
        }
    }

    private async Task DeleteAsync()
    {
        if (!IsEditMode || IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            await using var context = await _contextFactory.CreateDbContextAsync();
            var entity = await context.Productos.FirstOrDefaultAsync(p => p.Id == _productoId);
            if (entity is not null)
            {
                context.Productos.Remove(entity);
                await context.SaveChangesAsync();
            }

            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsBusy = false;
            ((Command)DeleteCommand).ChangeCanExecute();
        }
    }

    private static void SyncCollection<T>(ObservableCollection<T> target, IEnumerable<T> source)
    {
        target.Clear();
        foreach (var item in source)
        {
            target.Add(item);
        }
    }
}
