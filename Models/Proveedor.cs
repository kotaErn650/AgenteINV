namespace AgenteINV.Models;

public class Proveedor : ObservableObjectBase
{
    private int _id;
    private string _nombre = string.Empty;
    private string _contacto = string.Empty;
    private string _direccion = string.Empty;

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

    public string Contacto
    {
        get => _contacto;
        set => SetProperty(ref _contacto, value);
    }

    public string Direccion
    {
        get => _direccion;
        set => SetProperty(ref _direccion, value);
    }

    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
