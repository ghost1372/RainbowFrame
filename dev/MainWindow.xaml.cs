using RainbowFrame.Common;
using RainbowFrame.Views;

namespace RainbowFrame;

public sealed partial class MainWindow : Window
{
    internal static MainWindow Instance { get; private set; }
    public IDelegateCommand ShowHideWindowCommand { get; }
    public IDelegateCommand ExitCommand { get; }
    public MainWindow()
    {
        this.InitializeComponent();
        Instance = this;

        ShowHideWindowCommand = DelegateCommand.Create(OnShowHideWindow);
        ExitCommand = DelegateCommand.Create(OnExit);

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;

        var NavService = App.GetService<IJsonNavigationService>() as JsonNavigationService;
        if (NavService != null)
        {
            NavService.Initialize(NavView, NavFrame, NavigationPageMappings.PageDictionary)
                .ConfigureDefaultPage(typeof(MainPage))
                .ConfigureSettingsPage(typeof(SettingsPage))
                .ConfigureJsonFile("Assets/NavViewMenu/AppData.json", OrderItemsType.AscendingBoth)
                .ConfigureTitleBar(AppTitleBar)
                .ConfigureBreadcrumbBar(BreadCrumbNav, BreadcrumbPageMappings.PageDictionary);
        }
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
            await contentDialog.ShowAsyncQueue();
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

                if (Settings.ResetWhenClosed)
                {
                    MainPage.Instance?.ViewModel?.ResetAll();
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
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            string query = sender.Text;

            var filteredItems = MainPage.Instance?.ViewModel.Windows.Where(item =>
                (item.Text != null && item.Text.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                item.Handle.ToString().Contains(query)).ToList();

            MainPage.Instance?.SetListViewItemsSource(filteredItems);
        }
    }

    private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {

    }

    private void KeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        HeaderAutoSuggestBox.Focus(FocusState.Programmatic);
    }

    public void OnShowHideWindow()
    {
        if (Visible)
        {
            AppWindow.Hide();
        }
        else
        {
            AppWindow.Show();
        }
    }

    public void OnExit()
    {
        if (Settings.ResetWhenClosed)
        {
            MainPage.Instance?.ViewModel?.ResetAll();
        }
        Environment.Exit(0);
    }
}

