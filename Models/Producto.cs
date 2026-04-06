namespace AgenteINV.Models;

public class Producto : ObservableObjectBase
{
    private int _id;
    private string _nombre = string.Empty;
    private string _descripcion = string.Empty;
    private decimal _precio;
    private int _stock;
    private string _imagenUrl = string.Empty;
    private int _categoriaId;
    private int _proveedorId;
    private Categoria? _categoria;
    private Proveedor? _proveedor;

    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string Nombre
    {
        get => _nombre;
        set => SetProperty(ref _nombre, value);
    }

    public string Descripcion
    {
        get => _descripcion;
        set => SetProperty(ref _descripcion, value);
    }

    public decimal Precio
    {
        get => _precio;
        set => SetProperty(ref _precio, value);
    }

    public int Stock
    {
        get => _stock;
        set => SetProperty(ref _stock, value);
    }

    public string ImagenUrl
    {
        get => _imagenUrl;
        set => SetProperty(ref _imagenUrl, value);
    }

    public int CategoriaId
    {
        get => _categoriaId;
        set => SetProperty(ref _categoriaId, value);
    }

    public int ProveedorId
    {
        get => _proveedorId;
        set => SetProperty(ref _proveedorId, value);
    }

    public Categoria? Categoria
    {
        get => _categoria;
        set => SetProperty(ref _categoria, value);
    }

    public Proveedor? Proveedor
    {
        get => _proveedor;
        set => SetProperty(ref _proveedor, value);
    }
}
