using FoodDrinkApp.Services;

namespace FoodDrinkApp;

public partial class SettingsPage : ContentPage
{
    private bool _isInitializing;

    public SettingsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // 1. 保留原本的大字体无障碍应用
        AccessibilityService.ApplyFontScale(this);

        // 2. 初始化语言选择器状态，避免死循环触发
        _isInitializing = true;
        LanguagePicker.SelectedIndex = LanguageService.CurrentLanguage == "en" ? 0 : 1;

        // 3. 恢复主题选择器的默认状态 (如果之前有保存逻辑，请按需修改)
        if (ThemePicker.SelectedIndex < 0)
        {
            ThemePicker.SelectedIndex = 0;
        }
        _isInitializing = false;

        // 4. 更新当前语言文本
        UpdateLanguageTexts();
    }

    private void OnThemeChanged(object? sender, EventArgs e)
    {
        if (_isInitializing || ThemePicker.SelectedIndex < 0) return;

        // 您的原有主题切换逻辑
        Application.Current!.UserAppTheme = ThemePicker.SelectedIndex switch
        {
            1 => AppTheme.Light,
            2 => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };

        SettingsStatusLabel.Text = LanguageService.CurrentLanguage == "en" ? "Theme updated." : "主题已更新。";
        SemanticScreenReader.Announce(SettingsStatusLabel.Text);
    }

    private void OnLargeTextToggled(object? sender, ToggledEventArgs e)
    {
        // 您的原有大字体切换逻辑
        AccessibilityService.LargeTextEnabled = e.Value;
        AccessibilityService.ApplyFontScale(this);

        SettingsStatusLabel.Text = LanguageService.CurrentLanguage == "en" ? "Text size updated." : "字体大小已更新。";
        SemanticScreenReader.Announce(SettingsStatusLabel.Text);
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        if (_isInitializing || LanguagePicker.SelectedIndex < 0) return;

        LanguageService.CurrentLanguage = LanguagePicker.SelectedIndex == 0 ? "en" : "zh";
        UpdateLanguageTexts();

        SettingsStatusLabel.Text = LanguageService.CurrentLanguage == "en" ? "Language updated." : "语言已切换为中文。";
        SemanticScreenReader.Announce(SettingsStatusLabel.Text);
    }

    private void UpdateLanguageTexts()
    {
        // 使用 LanguageService 读取并更新所有 Label 的 Text 属性
        Title = LanguageService.Get("SettingsTitle");

        PageTitleLabel.Text = LanguageService.Get("SettingsTitle");
        PageDescLabel.Text = LanguageService.Get("SettingsDesc");

        LanguageSectionLabel.Text = LanguageService.Get("LanguageSection");
        LanguagePicker.Title = LanguageService.Get("LanguagePickerTitle");

        ThemeSectionLabel.Text = LanguageService.Get("ThemeSection");
        ThemePicker.Title = LanguageService.Get("ThemePickerTitle");

        FontSectionLabel.Text = LanguageService.Get("FontSection");
        FontDescLabel.Text = LanguageService.Get("FontDesc");

        LargeTextPreviewTitle.Text = LanguageService.Get("PreviewTitle");
        LargeTextPreviewBody.Text = LanguageService.Get("PreviewBody");
    }
}