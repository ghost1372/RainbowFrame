﻿using RainbowFrame.ViewModels;

namespace RainbowFrame.Views;

public sealed partial class AppUpdateSettingPage : Page
{
    public AppUpdateSettingViewModel ViewModel { get; }

    public AppUpdateSettingPage()
    {
        ViewModel = App.GetService<AppUpdateSettingViewModel>();
        this.InitializeComponent();
    }
}
