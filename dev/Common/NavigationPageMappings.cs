namespace RainbowFrame.Common;
public partial class NavigationPageMappings
{
    public static Dictionary<string, Type> PageDictionary { get; } = new Dictionary<string, Type>
    {
        {"RainbowFrame.Views.MainPage", typeof(RainbowFrame.Views.MainPage)},
    };
}
