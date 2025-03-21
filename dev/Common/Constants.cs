﻿namespace RainbowFrame.Common;

public static class Constants
{
    public static readonly string RootDirectoryPath = Path.Combine(PathHelper.GetAppDataFolderPath(), ProcessInfoHelper.ProductNameAndVersion);
    public static readonly string AppConfigPath = Path.Combine(RootDirectoryPath, "AppConfig.json");
}
