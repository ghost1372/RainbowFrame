using RainbowFrame.ViewModels;

namespace RainbowFrame.Views;
public sealed partial class GeneralSettingPage : Page
{
    public GeneralSettingViewModel ViewModel { get; }
    public GeneralSettingPage()
    {
        ViewModel = App.GetService<GeneralSettingViewModel>();
        this.InitializeComponent();
    }

    private void ActiveWindow_Toggled(object sender, RoutedEventArgs e)
    {
        if (TGActiveWindow.IsOn)
        {
            MainWindow.Instance.ViewModel.StartTracking();
        }
    }
}
