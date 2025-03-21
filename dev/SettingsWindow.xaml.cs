namespace RainbowFrame;

public sealed partial class SettingsWindow : Window
{
    internal static SettingsWindow Instance { get; private set; }
    public SettingsWindow(Type type)
    {
        this.InitializeComponent();
        Instance = this;

        Title = AppWindow.Title = "Settings";
        AppWindow.SetIcon("Assets/icon.ico");
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;

        NavFrame.Navigate(type);
    }
}
