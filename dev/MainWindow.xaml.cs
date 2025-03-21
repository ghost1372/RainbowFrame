using RainbowFrame.ViewModels;
using Windows.System;

namespace RainbowFrame;

public sealed partial class MainWindow : Window
{
    public MainViewModel ViewModel { get; }
    internal static MainWindow Instance { get; private set; }
    public MainWindow()
    {
        ViewModel = App.GetService<MainViewModel>();
        this.InitializeComponent();
        Instance = this;

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;
        Closed += MainWindow_Closed;
    }

    private async void MainWindow_Closed(object sender, WindowEventArgs args)
    {
        args.Handled = true;
        if (Settings.IsFirstTrayIcon)
        {
            ContentDialog contentDialog = new ContentDialog();
            contentDialog.XamlRoot = App.MainWindow.Content.XamlRoot;
            contentDialog.Loaded += (s, e) =>
            {
                contentDialog.Content = "The app will continue running in the background and can be accessed from the system tray. You can enable or disable this notification in the app settings.";
                contentDialog.RequestedTheme = App.Current.ThemeService.GetElementTheme();
            };
            contentDialog.Title = "App Hidden to System Tray";
            contentDialog.PrimaryButtonText = "Ok";
            contentDialog.SecondaryButtonText = "Cancel";
            contentDialog.CloseButtonText = "Exit";
            contentDialog.PrimaryButtonClick += (s, e) =>
            {
                Settings.IsFirstTrayIcon = false;
                App.MainWindow.AppWindow.Hide();
            };
            contentDialog.CloseButtonClick += (s, e) =>
            {
                args.Handled = false;
                Close();
                Environment.Exit(0);
            };
            contentDialog.PrimaryButtonStyle = (Style)Application.Current.Resources["AccentButtonStyle"];
            await contentDialog.ShowAsync();
        }
        else
        {
            if (Settings.UseTrayIcon)
            {
                App.MainWindow.AppWindow.Hide();
            }
            else
            {
                args.Handled = false;

                if (TGResetAll.IsOn)
                {
                    ViewModel?.ResetAll();
                }
            }
        }
    }

    private void ThemeButton_Click(object sender, RoutedEventArgs e)
    {
        ThemeService.ChangeThemeWithoutSave(App.MainWindow);
    }

    private void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
    }

    private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
    }

    private void KeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        HeaderAutoSuggestBox.Focus(FocusState.Programmatic);
    }

    private void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.RefreshCommand.Execute(null);
    }
    private async void btnGotoSource_Click(object sender, RoutedEventArgs e)
    {
        await Launcher.LaunchUriAsync(new Uri("https://github.com/ghost1372/RainbowFrame/"));
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
    private void NumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        ViewModel?.OnEffectSpeedValueChanged();
    }

    private void OnRainbowEffectForWindow_Toggled(object sender, RoutedEventArgs e)
    {
        if (TGRainbowEffectForWindow.IsOn)
        {
            ViewModel.StartRainbowCommand.Execute(null);
        }
        else
        {
            ViewModel.StopRainbowCommand.Execute(null);
        }
    }

    private void OnRainbowEffectForAllWindow_Toggled(object sender, RoutedEventArgs e)
    {
        if (TGRainbowEffectForAllWindow.IsOn)
        {
            ViewModel.StartRainbowForAllCommand.Execute(null);
        }
        else
        {
            ViewModel.StopRainbowForAllCommand.Execute(null);
        }
    }
}

