using Microsoft.UI.Xaml.Navigation;
using RainbowFrame.ViewModels;

namespace RainbowFrame.Views;

public sealed partial class ControlCenterPage : Page
{
    public MainViewModel ViewModel { get; set; }
    public ControlCenterPage()
    {
        ViewModel = MainWindow.Instance.ViewModel;
        this.InitializeComponent();
    }
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        Win32Window selectedItem = ViewModel.SelectedItem as Win32Window;
        if (selectedItem != null)
        {
            var itemExist = Settings.RainbowWindows.Where(x => x.HWND == selectedItem.Handle).FirstOrDefault();
            if (itemExist != null)
            {
                TGRainbowEffectForWindow.IsOn = true;
                NBSpeed.Value = itemExist.Speed;
            }
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
}
