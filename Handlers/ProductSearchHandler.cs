using AgenteINV.Models;
using System.Windows.Input;

namespace AgenteINV.Handlers;

public class ProductSearchHandler : SearchHandler
{
    public static readonly BindableProperty SourceProductsProperty = BindableProperty.Create(
        nameof(SourceProducts),
        typeof(IEnumerable<Producto>),
        typeof(ProductSearchHandler),
        default(IEnumerable<Producto>));

    public static readonly BindableProperty SearchCommandProperty = BindableProperty.Create(
        nameof(SearchCommand),
        typeof(ICommand),
        typeof(ProductSearchHandler));

    public IEnumerable<Producto>? SourceProducts
    {
        get => (IEnumerable<Producto>?)GetValue(SourceProductsProperty);
        set => SetValue(SourceProductsProperty, value);
    }

    public ICommand? SearchCommand
    {
        get => (ICommand?)GetValue(SearchCommandProperty);
        set => SetValue(SearchCommandProperty, value);
    }

    protected override void OnQueryChanged(string? oldValue, string? newValue)
    {
        var filtered = string.IsNullOrWhiteSpace(newValue)
            ? SourceProducts?.ToList()
            : SourceProducts?
                .Where(p => p.Nombre.Contains(newValue, StringComparison.OrdinalIgnoreCase))
                .ToList();

        ItemsSource = filtered;
        SearchCommand?.Execute(newValue ?? string.Empty);
        base.OnQueryChanged(oldValue, newValue);
    }

    protected override async void OnItemSelected(object? item)
    {
        base.OnItemSelected(item);

        if (item is Producto producto)
        {
            await Shell.Current.GoToAsync($"{nameof(Views.ProductoDetailPage)}?productoId={producto.Id}");
        }
    }
}
