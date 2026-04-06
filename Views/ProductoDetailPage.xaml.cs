using AgenteINV.ViewModels;

namespace AgenteINV.Views;

public partial class ProductoDetailPage : ContentPage, IQueryAttributable
{
    private readonly ProductoDetailViewModel _viewModel;

    public ProductoDetailPage(ProductoDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_initializedFromQuery)
        {
            await _viewModel.InitializeAsync();
        }
    }

    private bool _initializedFromQuery;

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _initializedFromQuery = false;

        if (query.TryGetValue("productoId", out var rawValue) &&
            int.TryParse(rawValue?.ToString(), out var productoId) &&
            productoId > 0)
        {
            _initializedFromQuery = true;
            await _viewModel.InitializeAsync(productoId);
            return;
        }

        _viewModel.ResetForNew();
    }
}
