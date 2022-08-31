namespace MauiJass;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(JassPage), typeof(JassPage));
        Routing.RegisterRoute(nameof(GamePage), typeof(GamePage));

    }
}
