﻿using Microsoft.Extensions.DependencyInjection;
using RainbowFrame.ViewModels;

namespace RainbowFrame;
public partial class App : Application
{
    public static Window MainWindow = Window.Current;
    public new static App Current => (App)Application.Current;
    public IServiceProvider Services { get; }
    public IThemeService ThemeService => GetService<IThemeService>();
    public IJsonNavigationService NavService => GetService<IJsonNavigationService>();

    public static T GetService<T>() where T : class
    {
        if ((App.Current as App)!.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public App()
    {
        Services = ConfigureServices();
        this.InitializeComponent();
    }
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<IJsonNavigationService, JsonNavigationService>();

        services.AddTransient<MainViewModel>();
        services.AddTransient<AppUpdateSettingViewModel>();
        services.AddTransient<GeneralSettingViewModel>();

        return services.BuildServiceProvider();
    }
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new MainWindow();

        if (this.ThemeService != null)
        {
            this.ThemeService.AutoInitialize(MainWindow);
        }

        MainWindow.Title = MainWindow.AppWindow.Title = ProcessInfoHelper.ProductNameAndVersion;
        MainWindow.AppWindow.SetIcon("Assets/icon.ico");

        MainWindow.Activate();
    }
}

