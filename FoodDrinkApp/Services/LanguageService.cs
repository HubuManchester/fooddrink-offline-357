namespace FoodDrinkApp.Services;

public static class LanguageService
{
    // 全局当前语言状态，默认为英文 "en"，可选中文 "zh"
    public static string CurrentLanguage { get; set; } = "en";

    // 本地化多语言字典集合
    private static readonly Dictionary<string, Dictionary<string, string>> LocalizedTexts = new()
    {
        // 在 LanguageService.cs 的 LocalizedTexts 中确保包含以下键值对：
{
    "en", new Dictionary<string, string>
    {
        { "SettingsTitle", "Accessibility settings" },
        { "SettingsDesc", "Large text takes effect immediately and remains active when switching pages." },
        { "LanguageSection", "Language Preferences" },
        { "LanguagePickerTitle", "Select Language" },
        { "ThemeSection", "Theme mode" },
        { "ThemePickerTitle", "Choose app theme" },
        { "FontSection", "Large text mode" },
        { "FontDesc", "Increase text size for accessibility demonstration." },
        { "PreviewTitle", "Large text preview" },
        { "PreviewBody", "When enabled, this text and the rest of the page become visibly larger." }
    }
},
{
    "zh", new Dictionary<string, string>
    {
        { "SettingsTitle", "无障碍设置" },
        { "SettingsDesc", "大字体模式会立即生效，并在切换页面时保持激活状态。" },
        { "LanguageSection", "语言偏好" },
        { "LanguagePickerTitle", "选择语言" },
        { "ThemeSection", "主题模式" },
        { "ThemePickerTitle", "选择应用主题" },
        { "FontSection", "大字体模式" },
        { "FontDesc", "增大字体尺寸以展示无障碍功能。" },
        { "PreviewTitle", "大字体预览" },
        { "PreviewBody", "启用后，此文本以及页面上的其余文字将明显变大。" }
    }
}
    };

    /// <summary>
    /// 根据传入的键值获取对应当前语言的文本
    /// </summary>
    public static string Get(string key)
    {
        if (LocalizedTexts.TryGetValue(CurrentLanguage, out var dict) && dict.TryGetValue(key, out var text))
        {
            return text;
        }
        return key;
    }
}