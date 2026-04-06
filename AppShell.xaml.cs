using AgenteINV.Views;

namespace AgenteINV;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(ProductoDetailPage), typeof(ProductoDetailPage));
    }
}
