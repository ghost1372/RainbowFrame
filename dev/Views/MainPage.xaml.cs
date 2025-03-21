using RainbowFrame.ViewModels;

namespace RainbowFrame.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel { get; }
    internal static MainPage Instance { get; private set; }
    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();

        this.InitializeComponent();
        Instance = this;
        Loaded += MainPage_Loaded;
    }

    private void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.RefreshCommand.Execute(null);
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
        MainFrame.Navigate(typeof(ControlCenterPage));
    }

    private void NumberBoxAll_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        ViewModel?.OnEffectSpeedAllValueChanged();
    }

    internal void SetListViewItemsSource(List<Win32Window> filteredItems)
    {
        listView.ItemsSource = filteredItems;
    }
}
