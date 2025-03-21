﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Windows.System;

namespace RainbowFrame.ViewModels;
public partial class AppUpdateSettingViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string CurrentVersion { get; set; }

    [ObservableProperty]
    public partial string LastUpdateCheck { get; set; }

    [ObservableProperty]
    public partial bool IsUpdateAvailable { get; set; }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial bool IsCheckButtonEnabled { get; set; } = true;

    [ObservableProperty]
    public partial string LoadingStatus { get; set; } = "Status";

    private string ChangeLog = string.Empty;

    public AppUpdateSettingViewModel()
    {
        CurrentVersion = $"Current Version {ProcessInfoHelper.VersionWithPrefix}";
        LastUpdateCheck = Settings.LastUpdateCheck;
    }

    [RelayCommand]
    private async Task CheckForUpdateAsync()
    {
        IsLoading = true;
        IsUpdateAvailable = false;
        IsCheckButtonEnabled = false;
        LoadingStatus = "Checking for new version";
        if (NetworkHelper.IsNetworkAvailable())
        {
            try
            {
                string username = "Ghost1372";
                string repo = "RainbowFrame";
                LastUpdateCheck = DateTime.Now.ToShortDateString();
                Settings.LastUpdateCheck = DateTime.Now.ToShortDateString();
                var update = await UpdateHelper.CheckUpdateAsync(username, repo, new Version(ProcessInfoHelper.Version));
                if (update.StableRelease.IsExistNewVersion)
                {
                    IsUpdateAvailable = true;
                    ChangeLog = update.StableRelease.Changelog;
                    LoadingStatus = $"We found a new version {update.StableRelease.TagName} Created at {update.StableRelease.CreatedAt} and Published at {update.StableRelease.PublishedAt}";
                }
                else if (update.PreRelease.IsExistNewVersion)
                {
                    IsUpdateAvailable = true;
                    ChangeLog = update.PreRelease.Changelog;
                    LoadingStatus = $"We found a new PreRelease Version {update.PreRelease.TagName} Created at {update.PreRelease.CreatedAt} and Published at {update.PreRelease.PublishedAt}";
                }
                else
                {
                    LoadingStatus = "You are using latest version";
                }
            }
            catch (Exception ex)
            {
                LoadingStatus = ex.Message;
                IsLoading = false;
                IsCheckButtonEnabled = true;
            }
        }
        else
        {
            LoadingStatus = "Error Connection";
        }
        IsLoading = false;
        IsCheckButtonEnabled = true;
    }

    [RelayCommand]
    private async Task GoToUpdateAsync()
    {
        await Launcher.LaunchUriAsync(new Uri("https://github.com/Ghost1372/RainbowFrame/releases"));
    }

    [RelayCommand]
    private async Task GetReleaseNotesAsync()
    {
        ContentDialog dialog = new ContentDialog()
        {
            Title = "Release Note",
            CloseButtonText = "Close",
            Content = new ScrollViewer
            {
                Content = new TextBlock
                {
                    Text = ChangeLog,
                    Margin = new Thickness(10)
                },
                Margin = new Thickness(10)
            },
            Margin = new Thickness(10),
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = App.MainWindow.Content.XamlRoot
        };

        await dialog.ShowAsync();
    }
}
