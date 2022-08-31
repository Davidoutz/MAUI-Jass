using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm;
using MauiJass.Services;
using MauiJass.ViewModels;
using JassWeb.Shared;

namespace MauiJass;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
        builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("materialdesignicons-webfont.ttf", "MaterialIcons");
                fonts.AddFont("Chalk-Regular.ttf", "Chalk");
                fonts.AddFont("DiloWorld-mLJLv.ttf", "Dilo");
                fonts.AddFont("EraserDust.ttf", "EraserD");
                fonts.AddFont("EraserRegular.ttf", "EraserR");
                fonts.AddFont("sitka-small-815.ttf", "Sitka");
            });
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddTransient<JassPage>();
        builder.Services.AddTransient<JassViewModel>();
        builder.Services.AddTransient<GamePage>();
        builder.Services.AddTransient<GameViewModel>();
        builder.Services.AddSingleton<SignalRService>();
        builder.Services.AddSingleton<IPlayer, Player>();

        return builder.Build();
	}

}
