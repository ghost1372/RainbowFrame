using RainbowFrame.ViewModels;
using Windows.System;

namespace RainbowFrame.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel { get; set; }
    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        this.InitializeComponent();
        appTitleBar.Window = App.currentWindow;
        App.currentWindow.Closed += CurrentWindow_Closed;
        Loaded += MainPage_Loaded;
    }

    private void CurrentWindow_Closed(object sender, WindowEventArgs args)
    {
        if (tgResetAll.IsOn)
        {
            ViewModel?.ResetAll();
        }
    }

    private void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.RefreshCommand.Execute(null);
    }

    private void btnToggleTheme_Click(object sender, RoutedEventArgs e)
    {
        var element = App.currentWindow.Content as FrameworkElement;

        if (element.ActualTheme == ElementTheme.Light)
        {
            element.RequestedTheme = ElementTheme.Dark;
        }
        else if (element.ActualTheme == ElementTheme.Dark)
        {
            element.RequestedTheme = ElementTheme.Light;
        }
    }

    private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (listView.SelectedIndex == -1)
        {
            ViewModel.IsUIElementEnabled = false;
        }
        else
        {
            ViewModel.IsUIElementEnabled = true;
        }
    }

    private async void btnGotoSource_Click(object sender, RoutedEventArgs e)
    {
        await Launcher.LaunchUriAsync(new Uri("https://github.com/ghost1372/RainbowFrame/"));
    }

    private void NumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        ViewModel?.OnEffectSpeedValueChanged();
    }
}

