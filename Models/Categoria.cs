namespace AgenteINV.Models;

public class Categoria : ObservableObjectBase
{
    private int _id;
    private string _nombre = string.Empty;

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

    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
