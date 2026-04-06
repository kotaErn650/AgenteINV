using AgenteINV.DataAcces;

namespace AgenteINV;

public partial class App : Application
{
    public App(DatabaseInitializer databaseInitializer)
    {
        InitializeComponent();
        _ = databaseInitializer.InitializeAsync();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}
