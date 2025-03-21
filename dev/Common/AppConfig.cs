using Nucs.JsonSettings.Examples;
using Nucs.JsonSettings.Modulation;

namespace RainbowFrame.Common;

[GenerateAutoSaveOnChange]
public partial class AppConfig : NotifiyingJsonSettings, IVersionable
{
    [EnforcedVersion("1.2.1.0")]
    public Version Version { get; set; } = new Version(1, 1, 0, 0);

    public string fileName { get; set; } = Constants.AppConfigPath;
    public bool useTrayIcon { get; set; } = true;
    public bool isFirstTrayIcon { get; set; } = true;
}
