using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RainbowFrame.Views;
using static RainbowFrame.Common.NativeMethods;

namespace RainbowFrame.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    private IntPtr _previousWindow;
    private const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
    private const uint WINEVENT_OUTOFCONTEXT = 0;
    private WinEventDelegate _winEventDelegate;


    private Dictionary<string, Type> SettingsDictionary = new Dictionary<string, Type>()
    {
        { "AboutUsSettingPage", typeof(AboutUsSettingPage) },
        { "AppUpdateSettingPage", typeof(AppUpdateSettingPage) },
        { "GeneralSettingPage", typeof(GeneralSettingPage) },
        { "ThemeSettingPage", typeof(ThemeSettingPage) },
    };

    public Dictionary<nint, DevWinUI.RainbowFrame> rainbowKeys = new();

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
    public void OnEffectSpeedValueChanged()
    {
        Win32Window selectedItem = SelectedItem as Win32Window;
        OnEffectSpeedChangedBase(selectedItem, RainbowEffectSpeed);
    }

    [ObservableProperty]
    public partial int RainbowEffectSpeedAll { get; set; } = 4;
    public void OnEffectSpeedAllValueChanged()
    {
        foreach (var item in Windows)
        {
            OnEffectSpeedChangedBase(item, RainbowEffectSpeedAll);
        }
    }

    public MainViewModel()
    {
        if (Settings.ActiveWindow)
        {
            StartTracking();
        }
    }
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

    internal void StartRainbowBase(Win32Window window, int speed)
    {
        rainbowKeys.TryGetValue(window.Handle, out var rainbowFrame);
        if (window != null)
        {
            if (rainbowFrame == null)
            {
                rainbowFrame = new DevWinUI.RainbowFrame();
                rainbowFrame.Initialize(window.Handle);
                rainbowKeys.AddIfNotExists(window.Handle, rainbowFrame);
            }
            rainbowFrame?.UpdateEffectSpeed(speed);
            rainbowFrame?.StopRainbowFrame();
            rainbowFrame?.StartRainbowFrame();

            Settings.RainbowWindows.Add(new Common.RainbowWindow { HWND = window.Handle, Title = window.Text, Speed = speed });
            Settings.Save();
        }
    }
    internal void StopRainbowBase(Win32Window window)
    {
        if (window != null)
        {
            rainbowKeys.TryGetValue(window.Handle, out var rainbowFrame);
            rainbowFrame?.StopRainbowFrame();
        }
    }
    internal void ResetRainbowBase(Win32Window window)
    {
        if (window != null)
        {
            rainbowKeys.TryGetValue(window.Handle, out var rainbowFrame);
            rainbowFrame?.ResetFrameColorToDefault();
        }
    }
    internal void OnEffectSpeedChangedBase(Win32Window window, int speed)
    {
        if (window != null)
        {
            rainbowKeys.TryGetValue(window.Handle, out var rainbowFrame);
            rainbowFrame?.UpdateEffectSpeed(speed);
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
            StartRainbowBase(item, RainbowEffectSpeedAll);
        }
    }

    [RelayCommand]
    private void OnStopRainbowForAll()
    {
        foreach (var item in Windows)
        {
            StopRainbowBase(item);
        }
    }

    [RelayCommand]
    private void OnStartRainbow()
    {
        Win32Window selectedItem = SelectedItem as Win32Window;
        StartRainbowBase(selectedItem, RainbowEffectSpeed);
    }

    [RelayCommand]
    private void OnStopRainbow()
    {
        Win32Window selectedItem = SelectedItem as Win32Window;
        StopRainbowBase(selectedItem);
    }

    [RelayCommand]
    private void OnResetAll()
    {
        foreach (var item in Windows)
        {
            ResetRainbowBase(item);
        }

        Settings.RainbowWindows.Clear();
        Settings.Save();
    }

    [RelayCommand]
    private void OnReset()
    {
        Win32Window selectedItem = SelectedItem as Win32Window;
        ResetRainbowBase(selectedItem);

        var rbWindow = Settings.RainbowWindows.Where(x => x.HWND == selectedItem.Handle).FirstOrDefault();
        if (rbWindow != null)
        {
            Settings.RainbowWindows.Remove(rbWindow);
            Settings.Save();
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

    [RelayCommand]
    private void OnRefresh()
    {
        Windows = new ObservableCollection<Win32Window>(GetOpenWindows());
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
        if (Settings.ResetWhenClosed)
        {
            ResetAll();
        }
        Environment.Exit(0);
    }

    [RelayCommand]
    private void OnSettings(string page)
    {
        SettingsDictionary.TryGetValue(page, out var type);
        var settingWindow = new SettingsWindow(type);
        WindowHelper.TrackWindow(settingWindow);
        settingWindow.Activate();
    }

    public void ResetAll()
    {
        foreach (var rainbow in rainbowKeys.Values)
        {
            rainbow?.ResetFrameColorToDefault();
        }
    }

    public void StartTracking()
    {
        _winEventDelegate = new WinEventDelegate(WinEventProc);
        SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _winEventDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);
    }

    private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        IntPtr activeWindowHandle = GetForegroundWindow();
        if (!Windows.Where(x => x.Handle == activeWindowHandle).Any())
        {
            OnRefresh();
        }

        if (activeWindowHandle != _previousWindow)
        {
            // If the previously focused window is not null, reset the rainbow effect
            if (_previousWindow != IntPtr.Zero && Settings.ActiveWindow)
            {
                ResetRainbowBase(new Win32Window(_previousWindow));
            }

            // Update the tracked window
            _previousWindow = activeWindowHandle;

            // Start the rainbow effect on the new active window
            if (Settings.ActiveWindow)
            {
                StartRainbowBase(new Win32Window(activeWindowHandle), RainbowEffectSpeedAll);
            }
        }
    }
}
