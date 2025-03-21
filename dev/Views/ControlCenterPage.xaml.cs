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
