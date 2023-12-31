using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RainbowFrame.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [ObservableProperty]
    public ObservableCollection<WindowInfo> windows = new();

    [ObservableProperty]
    public object selectedItem;

    [ObservableProperty]
    public bool isUIElementEnabled;

    [ObservableProperty]
    public bool isAllWindowToggled;

    [ObservableProperty]
    public int rainbowEffectSpeed = 4;

    public Dictionary<string, WinUICommunity.RainbowFrame> rainbowKeys = new();

    private List<WindowInfo> GetOpenWindows()
    {
        List<WindowInfo> _windows = new List<WindowInfo>();

        EnumWindows((hWnd, lParam) =>
        {
            if (IsWindowVisible(hWnd))
            {
                int textLength = GetWindowTextLength(hWnd);

                StringBuilder titleBuilder = new StringBuilder(textLength + 1);
                GetWindowText(hWnd, titleBuilder, titleBuilder.Capacity);

                string title = titleBuilder.ToString();
                if (IsAllWindowToggled)
                {
                    _windows.Add(new WindowInfo { HWnd = hWnd, Title = title });
                }
                else
                {
                    if (!string.IsNullOrEmpty(title))
                    {
                        _windows.Add(new WindowInfo { HWnd = hWnd, Title = title });
                    }
                }
            }

            return true; // Continue enumeration
        }, IntPtr.Zero);

        return _windows;
    }

    [RelayCommand]
    private void OnAllWindowToggled()
    {
        OnRefresh();
    }

    [RelayCommand]
    private void OnStartRainbow()
    {
        WindowInfo selectedItem = SelectedItem as WindowInfo;

        WinUICommunity.RainbowFrame rainbowFrame = rainbowKeys.Where(x => x.Key.Equals(selectedItem.Title)).FirstOrDefault().Value;

        if (selectedItem != null)
        {
            if (rainbowFrame == null)
            {
                rainbowFrame = new WinUICommunity.RainbowFrame();
                rainbowFrame.Initialize(selectedItem.HWnd);
                rainbowKeys.AddIfNotExists(selectedItem.Title, rainbowFrame);
            }
            rainbowFrame?.UpdateEffectSpeed(RainbowEffectSpeed);
            rainbowFrame?.StopRainbowFrame();
            rainbowFrame?.StartRainbowFrame();
        }
    }

    [RelayCommand]
    private void OnStopRainbow()
    {
        WindowInfo selectedItem = SelectedItem as WindowInfo;
        if (selectedItem != null)
        {
            var rainbow = rainbowKeys.Where(x => x.Key.Equals(selectedItem.Title)).FirstOrDefault();
            rainbow.Value?.StopRainbowFrame();
        }
    }

    [RelayCommand]
    private void OnReset()
    {
        WindowInfo selectedItem = SelectedItem as WindowInfo;
        if (selectedItem != null)
        {
            var rainbow = rainbowKeys.Where(x => x.Key.Equals(selectedItem.Title)).FirstOrDefault();
            rainbow.Value?.ResetFrameColorToDefault();
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
        WindowInfo selectedItem = SelectedItem as WindowInfo;
        WinUICommunity.RainbowFrame rainbowFrame = rainbowKeys.Where(x => x.Key.Equals(selectedItem.Title)).FirstOrDefault().Value;

        colorPicker.ColorChanged += (s, e) =>
        {
            try
            {
                if (selectedItem != null)
                {
                    if (rainbowFrame == null)
                    {
                        rainbowFrame = new WinUICommunity.RainbowFrame();
                        rainbowFrame.Initialize(selectedItem.HWnd);
                        rainbowKeys.AddIfNotExists(selectedItem.Title, rainbowFrame);
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
        contentDialog.XamlRoot = App.currentWindow.Content.XamlRoot;
        contentDialog.Loaded += (s, e) =>
        {
            contentDialog.Content = scrollViewer;
            contentDialog.RequestedTheme = App.Current.ThemeService.GetCurrentTheme();
        };
        contentDialog.Title = "Choose Color";
        contentDialog.PrimaryButtonText = "Ok";
        contentDialog.SecondaryButtonText = "Cancel";
        contentDialog.SecondaryButtonClick += (s, e) =>
        {
            rainbowFrame?.ResetFrameColorToDefault();
        };
        contentDialog.PrimaryButtonStyle = (Style)Application.Current.Resources["AccentButtonStyle"];
        await contentDialog.ShowAsyncQueue();
    }

    public void OnEffectSpeedValueChanged()
    {
        WindowInfo selectedItem = SelectedItem as WindowInfo;
        if (selectedItem != null)
        {
            var rainbow = rainbowKeys.Where(x => x.Key.Equals(selectedItem.Title)).FirstOrDefault();
            rainbow.Value?.UpdateEffectSpeed(RainbowEffectSpeed);
        }
    }

    [RelayCommand]
    private void OnRefresh()
    {
        Windows = new ObservableCollection<WindowInfo>(GetOpenWindows());
    }

    public void ResetAll()
    {
        foreach (var rainbow in rainbowKeys)
        {
            rainbow.Value?.ResetFrameColorToDefault();
        }
    }
}
