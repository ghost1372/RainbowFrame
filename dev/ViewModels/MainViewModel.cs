using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RainbowFrame.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [ObservableProperty]
    public partial ObservableCollection<Win32Window> Windows { get; set; } = new();

    [ObservableProperty]
    public partial object SelectedItem { get; set; }

    [ObservableProperty]
    public partial bool IsUIElementEnabled { get; set; }

    [ObservableProperty]
    public partial bool IsAllWindowToggled { get; set; }

    [ObservableProperty]
    public partial int RainbowEffectSpeed { get; set; } = 4;

    public Dictionary<nint, DevWinUI.RainbowFrame> rainbowKeys = new();

    private List<Win32Window> GetOpenWindows()
    {
        List<Win32Window> _windows = new List<Win32Window>();
        var topWindows = WindowHelper.GetTopLevelWindows();

        if (IsAllWindowToggled)
        {
            return topWindows.ToList();
        }
        else
        {
            return topWindows.Where(w => !string.IsNullOrEmpty(w.Text) && IsWindowVisible(w.Handle)).ToList();
        }
    }

    [RelayCommand]
    private void OnAllWindowToggled()
    {
        OnRefresh();
    }

    [RelayCommand]
    private void OnStartRainbowForAll()
    {
        foreach (var item in Windows)
        {
            rainbowKeys.TryGetValue(item.Handle, out var rainbowFrame);

            if (rainbowFrame == null)
            {
                rainbowFrame = new DevWinUI.RainbowFrame();
                rainbowFrame.Initialize(item.Handle);
                rainbowKeys.AddIfNotExists(item.Handle, rainbowFrame);
            }
            rainbowFrame?.UpdateEffectSpeed(RainbowEffectSpeed);
            rainbowFrame?.StopRainbowFrame();
            rainbowFrame?.StartRainbowFrame();
        }
    }

    [RelayCommand]
    private void OnStopRainbowForAll()
    {
        foreach (var item in Windows)
        {
            rainbowKeys.TryGetValue(item.Handle, out var rainbowFrame);
            rainbowFrame?.StopRainbowFrame();
        }
    }

    [RelayCommand]
    private void OnStartRainbow()
    {
        Win32Window selectedItem = SelectedItem as Win32Window;

        rainbowKeys.TryGetValue(selectedItem.Handle, out var rainbowFrame);
        if (selectedItem != null)
        {
            if (rainbowFrame == null)
            {
                rainbowFrame = new DevWinUI.RainbowFrame();
                rainbowFrame.Initialize(selectedItem.Handle);
                rainbowKeys.AddIfNotExists(selectedItem.Handle, rainbowFrame);
            }
            rainbowFrame?.UpdateEffectSpeed(RainbowEffectSpeed);
            rainbowFrame?.StopRainbowFrame();
            rainbowFrame?.StartRainbowFrame();
        }
    }

    [RelayCommand]
    private void OnStopRainbow()
    {
        Win32Window selectedItem = SelectedItem as Win32Window;
        if (selectedItem != null)
        {
            rainbowKeys.TryGetValue(selectedItem.Handle, out var rainbowFrame);
            rainbowFrame?.StopRainbowFrame();
        }
    }

    [RelayCommand]
    private void OnResetAll()
    {
        foreach (var item in Windows)
        {
            rainbowKeys.TryGetValue(item.Handle, out var rainbowFrame);
            rainbowFrame?.ResetFrameColorToDefault();
        }
    }

    [RelayCommand]
    private void OnReset()
    {
        Win32Window selectedItem = SelectedItem as Win32Window;
        if (selectedItem != null)
        {
            rainbowKeys.TryGetValue(selectedItem.Handle, out var rainbowFrame);
            rainbowFrame?.ResetFrameColorToDefault();
        }
    }

    [RelayCommand]
    private async Task OnChooseColor()
    {
        var scrollViewer = new ScrollViewer { Margin = new Thickness(10) };
        var colorPicker = new ColorPicker
        {
            ColorSpectrumShape = ColorSpectrumShape.Ring,
            IsMoreButtonVisible = false,
            IsColorSliderVisible = true,
            IsColorChannelTextInputVisible = true,
            IsHexInputVisible = true,
            IsAlphaEnabled = true,
            IsAlphaSliderVisible = true,
            IsAlphaTextInputVisible = true,
            Margin = new Thickness(10)
        };
        Win32Window selectedItem = SelectedItem as Win32Window;
        rainbowKeys.TryGetValue(selectedItem.Handle, out var rainbowFrame);
        colorPicker.ColorChanged += (s, e) =>
        {
            try
            {
                if (selectedItem != null)
                {
                    if (rainbowFrame == null)
                    {
                        rainbowFrame = new DevWinUI.RainbowFrame();
                        rainbowFrame.Initialize(selectedItem.Handle);
                        rainbowKeys.AddIfNotExists(selectedItem.Handle, rainbowFrame);
                    }
                    rainbowFrame?.StopRainbowFrame();
                    rainbowFrame?.ChangeFrameColor(e.NewColor);
                }
            }
            catch (Exception)
            {
            }
        };

        scrollViewer.Content = colorPicker;
        ContentDialog contentDialog = new ContentDialog();
        contentDialog.XamlRoot = App.MainWindow.Content.XamlRoot;
        contentDialog.Loaded += (s, e) =>
        {
            contentDialog.Content = scrollViewer;
            contentDialog.RequestedTheme = App.Current.ThemeService.GetElementTheme();
        };
        contentDialog.Title = "Choose Color";
        contentDialog.PrimaryButtonText = "Ok";
        contentDialog.SecondaryButtonText = "Cancel";
        contentDialog.SecondaryButtonClick += (s, e) =>
        {
            rainbowFrame?.ResetFrameColorToDefault();
        };
        contentDialog.PrimaryButtonStyle = (Style)Application.Current.Resources["AccentButtonStyle"];
        await contentDialog.ShowAsync();
    }

    public void OnEffectSpeedValueChanged()
    {
        Win32Window selectedItem = SelectedItem as Win32Window;
        if (selectedItem != null)
        {
            rainbowKeys.TryGetValue(selectedItem.Handle, out var rainbowFrame);
            rainbowFrame?.UpdateEffectSpeed(RainbowEffectSpeed);
        }
    }

    [RelayCommand]
    private void OnRefresh()
    {
        Windows = new ObservableCollection<Win32Window>(GetOpenWindows());
    }

    public void ResetAll()
    {
        foreach (var rainbow in rainbowKeys)
        {
            rainbow.Value?.ResetFrameColorToDefault();
        }
    }

    [RelayCommand]
    private void OnShowHideWindow()
    {
        if (App.MainWindow.Visible)
        {
            App.MainWindow.AppWindow.Hide();
        }
        else
        {
            App.MainWindow.AppWindow.Show();
        }
    }

    [RelayCommand]
    private void OnExit()
    {
        Environment.Exit(0);
    }
}
