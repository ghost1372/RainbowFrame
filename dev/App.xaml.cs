using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Media;
using RainbowFrame.ViewModels;

namespace RainbowFrame;
public partial class App : Application
{
    public static Window currentWindow = Window.Current;
    public IThemeService ThemeService { get; set; }
    public new static App Current => (App)Application.Current;
    public string AppVersion { get; set; } = AssemblyInfoHelper.GetAssemblyVersion();
    public string AppName { get; set; } = "RainbowFrame";
    public IServiceProvider Services { get; }
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
       
        services.AddTransient<MainViewModel>();

        return services.BuildServiceProvider();
    }
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        currentWindow = new Window();

        currentWindow.AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        currentWindow.AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;

        if (currentWindow.Content is not Frame rootFrame)
        {
            currentWindow.Content = rootFrame = new Frame();
        }

        ThemeService = new ThemeService();
        ThemeService.Initialize(currentWindow);
        ThemeService.ConfigBackdrop();
        ThemeService.ConfigElementTheme();
        ThemeService.ConfigBackdropFallBackColorForWindow10(Application.Current.Resources["ApplicationPageBackgroundThemeBrush"] as Brush);

        rootFrame.Navigate(typeof(MainPage));

        currentWindow.Title = currentWindow.AppWindow.Title = $"{AppName} v{AppVersion}";
        currentWindow.AppWindow.SetIcon("Assets/icon.ico");

        currentWindow.Activate();
    }
}

