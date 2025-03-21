using Nucs.JsonSettings.Examples;
using Nucs.JsonSettings.Modulation;

namespace RainbowFrame.Common;

[GenerateAutoSaveOnChange]
public partial class AppConfig : NotifiyingJsonSettings, IVersionable
{
    [EnforcedVersion("1.2.1.0")]
    public Version Version { get; set; } = new Version(1, 1, 0, 0);

    private string fileName { get; set; } = Constants.AppConfigPath;
    private bool useTrayIcon { get; set; } = true;
    private bool isFirstTrayIcon { get; set; } = true;
    private bool resetWhenClosed { get; set; }
    private bool activeWindow { get; set; }
    private string lastUpdateCheck { get; set; }
}
