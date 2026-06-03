using FoodDrinkApp.Models;
using FoodDrinkApp.Services;

namespace FoodDrinkApp;

[QueryProperty(nameof(ItemId), "id")]
public partial class FoodDetailPage : ContentPage
{
    private FoodItem? currentItem;

    // 动态获取当前语言状态
    private bool IsZh => LanguageService.CurrentLanguage == "zh";

    public FoodDetailPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
        UpdateLanguageTexts();
    }

    protected override void OnDisappearing()
    {
        SpeechService.Stop();
        base.OnDisappearing();
    }

    public string ItemId
    {
        set => _ = LoadItemAsync(value);
    }

    private async Task LoadItemAsync(string id)
    {
        currentItem = await FoodCatalogService.GetByIdAsync(id);
        BindingContext = currentItem;
        RenderItem();
    }

    private void UpdateLanguageTexts()
    {
        // 动态更新页面标题
        Title = IsZh ? "详情" : "Details";

        // 如果数据已加载，刷新页面渲染以应用新的语言
        if (currentItem != null)
        {
            RenderItem();
        }
    }

    private void RenderItem()
    {
        if (currentItem is null)
        {
            NameLabel.Text = IsZh ? "未找到记录" : "Record not found";
            DescriptionLabel.Text = IsZh ? "无法加载所选的食品或饮品。" : "The selected food or drink could not be loaded.";
            return;
        }

        NameLabel.Text = currentItem.Name;
        CategoryLabel.Text = currentItem.Category;
        CaloriesLabel.Text = currentItem.CaloriesLabel;
        MacroLabel.Text = currentItem.MacroSummary;
        DescriptionLabel.Text = currentItem.Description;
        AllergyLabel.Text = currentItem.AllergyNote;

        SemanticProperties.SetDescription(NameLabel, currentItem.AccessibleSummary);
    }

    private async void OnSpeakClicked(object? sender, EventArgs e)
    {
        if (currentItem is null)
        {
            await DisplayAlert(
                IsZh ? "记录缺失" : "Missing record",
                IsZh ? "没有可供朗读的营养摘要。" : "There is no nutrition summary to read.",
                IsZh ? "确定" : "OK");
            return;
        }

        try
        {
            await SpeechService.SpeakAsync(currentItem.AccessibleSummary);
            SemanticScreenReader.Announce(IsZh ? "正在朗读。" : "Reading started.");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                IsZh ? "语音不可用" : "Text to speech unavailable",
                ex.Message,
                IsZh ? "确定" : "OK");
        }
    }

    private void OnStopSpeechClicked(object? sender, EventArgs e)
    {
        SpeechService.Stop();
        SemanticScreenReader.Announce(IsZh ? "已停止朗读。" : "Reading stopped.");
    }

    private async void OnVibrateClicked(object? sender, EventArgs e)
    {
        try
        {
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(500));
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);

            await DisplayAlert(
                IsZh ? "提示" : "Reminder",
                IsZh ? "已触发震动与触觉反馈。" : "Vibration feedback has been triggered.",
                IsZh ? "确定" : "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                IsZh ? "震动不可用" : "Vibration unavailable",
                ex.Message,
                IsZh ? "确定" : "OK");
        }
    }
}