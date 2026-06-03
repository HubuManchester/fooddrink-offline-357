using FoodDrinkApp.Services;

namespace FoodDrinkApp;

public partial class MainPage : ContentPage
{
    // 用于动态更新 CollectionView 内部按钮的绑定属性
    public string DetailsButtonText { get; set; } = "Details";

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this; // 设置绑定上下文供详情按钮获取多语言文本
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
        UpdateLanguageTexts();
        await LoadFoodItemsAsync(SearchFoodBar.Text);
    }

    private void UpdateLanguageTexts()
    {
        // 如果您的 LanguageService 字典里没有这些 key，Get 方法会自动返回英文原文，属于安全回退设计
        Title = LanguageService.Get("MainTitle");
        PageTitleLabel.Text = LanguageService.Get("MainTitle");
        PageDescLabel.Text = LanguageService.Get("SettingsDesc"); // 可以复用描述或补充新的key

        NutritionLabel.Text = LanguageService.CurrentLanguage == "zh" ? "营养" : "Nutrition";
        PhotosLabel.Text = LanguageService.CurrentLanguage == "zh" ? "影像" : "Photos";
        VoiceLabel.Text = LanguageService.CurrentLanguage == "zh" ? "语音" : "Voice";

        SearchFoodBar.Placeholder = LanguageService.Get("SearchPlaceholder");
        AddRecordButton.Text = LanguageService.Get("AddRecordBtn");

        FoodCollection.EmptyView = LanguageService.CurrentLanguage == "zh"
            ? "未找到匹配的食品或饮品记录。"
            : "No matching food or drink records.";

        DetailsButtonText = LanguageService.Get("DetailsBtn");
        OnPropertyChanged(nameof(DetailsButtonText));
    }

    private async Task LoadFoodItemsAsync(string? query = null)
    {
        FoodCollection.ItemsSource = await FoodCatalogService.SearchAsync(query);
    }

    private async void OnAddClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddItemPage));
    }

    private async void OnDetailsClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string id)
        {
            await Shell.Current.GoToAsync($"{nameof(FoodDetailPage)}?id={Uri.EscapeDataString(id)}");
        }
    }

    private async void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        await LoadFoodItemsAsync(e.NewTextValue);
    }

    private async void OnSearchButtonPressed(object? sender, EventArgs e)
    {
        await LoadFoodItemsAsync(SearchFoodBar.Text);
    }

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await LoadFoodItemsAsync(SearchFoodBar.Text);
        FoodRefreshView.IsRefreshing = false;

        bool isZh = LanguageService.CurrentLanguage == "zh";
        var source = FoodCatalogService.LastLoadUsedMockApi ? "mockapi.io" : (isZh ? "本地备用数据" : "local fallback data");
        SemanticScreenReader.Announce(isZh ? $"列表已刷新，数据源: {source}" : $"List refreshed. Current source: {source}.");
    }
}