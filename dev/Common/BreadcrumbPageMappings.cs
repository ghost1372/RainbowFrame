namespace RainbowFrame.Common;
public partial class BreadcrumbPageMappings
{
    public static Dictionary<Type, BreadcrumbPageConfig> PageDictionary = new()
    {
        {typeof(RainbowFrame.Views.SettingsPage), new BreadcrumbPageConfig { PageTitle = null, IsHeaderVisible = true, ClearNavigation = false}},
        {typeof(RainbowFrame.Views.AboutUsSettingPage), new BreadcrumbPageConfig { PageTitle = null, IsHeaderVisible = true, ClearNavigation = false}},
        {typeof(RainbowFrame.Views.AppUpdateSettingPage), new BreadcrumbPageConfig { PageTitle = null, IsHeaderVisible = true, ClearNavigation = false}},
        {typeof(RainbowFrame.Views.GeneralSettingPage), new BreadcrumbPageConfig { PageTitle = null, IsHeaderVisible = true, ClearNavigation = false}},
        {typeof(RainbowFrame.Views.ThemeSettingPage), new BreadcrumbPageConfig { PageTitle = null, IsHeaderVisible = true, ClearNavigation = false}},
    };
}
