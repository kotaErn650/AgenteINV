using AgenteINV.DataAcces;
using AgenteINV.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AgenteINV.ViewModels;

public class ProductosViewModel : BaseViewModel
{
    private readonly IDbContextFactory<InventarioContext> _contextFactory;
    private readonly List<Producto> _productosCache = [];
    private Producto? _selectedProducto;

    public ProductosViewModel(IDbContextFactory<InventarioContext> contextFactory)
    {
        _contextFactory = contextFactory;
        Title = "Productos";
        Productos = [];
        ProductosVisibles = [];
        LoadProductosCommand = new Command(async () => await LoadAsync(), () => !IsBusy);
        NewProductoCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(Views.ProductoDetailPage)));
        SearchProductosCommand = new Command<string>(ApplySearch);
    }

    public ObservableCollection<Producto> Productos { get; }

    public ObservableCollection<Producto> ProductosVisibles { get; }

    public ICommand LoadProductosCommand { get; }

    public ICommand NewProductoCommand { get; }

    public ICommand SearchProductosCommand { get; }

    public Producto? SelectedProducto
    {
        get => _selectedProducto;
        set
        {
            if (SetProperty(ref _selectedProducto, value) && value is not null)
            {
                _ = NavigateToDetailAsync(value.Id);
            }
        }
    }

    public int TotalProductos => Productos.Count;

    public int ProductosAgotados => Productos.Count(p => p.Stock == 0);

    public decimal ValorInventario => Productos.Sum(p => p.Precio * p.Stock);

    public async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            await using var context = await _contextFactory.CreateDbContextAsync();
            var productos = await context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .OrderBy(p => p.Nombre)
                .ToListAsync();

            _productosCache.Clear();
            _productosCache.AddRange(productos);

            SyncCollection(Productos, productos);
            SyncCollection(ProductosVisibles, productos);
            RaiseTotals();
        }
        finally
        {
            IsBusy = false;
            ((Command)LoadProductosCommand).ChangeCanExecute();
        }
    }

    public void ApplySearch(string? query)
    {
        var filtered = string.IsNullOrWhiteSpace(query)
            ? _productosCache
            : _productosCache
                .Where(p => p.Nombre.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();

        SyncCollection(ProductosVisibles, filtered);
    }

    private async Task NavigateToDetailAsync(int productoId)
    {
        SelectedProducto = null;
        await Shell.Current.GoToAsync($"{nameof(Views.ProductoDetailPage)}?productoId={productoId}");
    }

    private void SyncCollection(ObservableCollection<Producto> target, IEnumerable<Producto> source)
    {
        target.Clear();
        foreach (var item in source)
        {
            target.Add(item);
        }
    }

    private void RaiseTotals()
    {
        OnPropertyChanged(nameof(TotalProductos));
        OnPropertyChanged(nameof(ProductosAgotados));
        OnPropertyChanged(nameof(ValorInventario));
    }
}
