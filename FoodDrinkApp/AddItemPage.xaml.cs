using FoodDrinkApp.Models;
using FoodDrinkApp.Services;

namespace FoodDrinkApp;

public partial class AddItemPage : ContentPage
{
    private bool IsZh => LanguageService.CurrentLanguage == "zh";

    public AddItemPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
        UpdateLanguageTexts();
    }

    private void UpdateLanguageTexts()
    {
        Title = LanguageService.Get("AddTitle");
        AddTitleLabel.Text = LanguageService.Get("AddTitle");
        AddDescLabel.Text = LanguageService.Get("AddDesc");

        BasicSectionLabel.Text = LanguageService.Get("BasicInfoSection");
        NameEntry.Placeholder = LanguageService.Get("NamePlaceholder");
        CategoryPicker.Title = LanguageService.Get("CategoryTitle");
        DescriptionEditor.Placeholder = LanguageService.Get("DescPlaceholder");

        NutritionSectionLabel.Text = LanguageService.Get("NutritionSection");
        CaloriesEntry.Placeholder = LanguageService.Get("CaloriesPlaceholder");
        ProteinEntry.Placeholder = LanguageService.Get("ProteinPlaceholder");
        CarbsEntry.Placeholder = LanguageService.Get("CarbsPlaceholder");
        FatEntry.Placeholder = LanguageService.Get("FatPlaceholder");
        AllergyEntry.Placeholder = LanguageService.Get("AllergyPlaceholder");

        SaveRecordButton.Text = LanguageService.Get("SaveRecordBtn");

        // 动态清空并载入当前语言的分类选项
        var selectedIndex = CategoryPicker.SelectedIndex;
        CategoryPicker.Items.Clear();
        var categories = IsZh
            ? new[] { "早餐", "午餐", "晚餐", "小吃", "饮品" }
            : new[] { "Breakfast", "Lunch", "Dinner", "Snack", "Drink" };

        foreach (var c in categories) CategoryPicker.Items.Add(c);

        if (selectedIndex >= 0 && selectedIndex < categories.Length)
        {
            CategoryPicker.SelectedIndex = selectedIndex;
        }
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        try
        {
            var validationMessage = ValidateForm(out var calories, out var protein, out var carbs, out var fat);
            if (validationMessage is not null)
            {
                ShowValidation(validationMessage);
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(250));
                return;
            }

            var item = new FoodItem
            {
                Name = NameEntry.Text!.Trim(),
                Category = CategoryPicker.SelectedItem?.ToString() ?? (IsZh ? "小吃" : "Snack"),
                Description = DescriptionEditor.Text!.Trim(),
                Calories = calories,
                Protein = protein,
                Carbs = carbs,
                Fat = fat,
                AllergyNote = string.IsNullOrWhiteSpace(AllergyEntry.Text)
                    ? (IsZh ? "未提供过敏提示。" : "No allergy note provided.")
                    : AllergyEntry.Text.Trim(),
                Tags = $"{NameEntry.Text} {CategoryPicker.SelectedItem} {DescriptionEditor.Text}"
            };

            await FoodCatalogService.AddAsync(item);
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);

            SemanticScreenReader.Announce(IsZh ? "记录已保存。" : "Food record saved.");

            string alertTitle = IsZh ? "已保存" : "Saved";
            string alertBtn = IsZh ? "确定" : "OK";
            string apiMsg = MockApiConfig.IsConfigured
                ? (IsZh ? "记录已同步至 mockapi.io 云端。" : "The record has been saved to mockapi.io.")
                : (IsZh ? "记录已保存至本地设备。" : "The record has been saved to local fallback data.");

            await DisplayAlert(alertTitle, apiMsg, alertBtn);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ShowValidation(IsZh ? $"保存记录失败: {ex.Message}" : $"The record could not be saved: {ex.Message}");
        }
    }

    private string? ValidateForm(out int calories, out int protein, out int carbs, out int fat)
    {
        calories = protein = carbs = fat = 0;

        if (string.IsNullOrWhiteSpace(NameEntry.Text))
            return IsZh ? "请输入食品或饮品名称。" : "Please enter a food or drink name.";

        if (CategoryPicker.SelectedIndex < 0)
            return IsZh ? "请选择一个所属分类。" : "Please choose a category.";

        if (string.IsNullOrWhiteSpace(DescriptionEditor.Text))
            return IsZh ? "请添加简要描述或配料。" : "Please add a short description.";

        return TryReadNumber(CaloriesEntry.Text, IsZh ? "热量" : "calories", out calories)
            ?? TryReadNumber(ProteinEntry.Text, IsZh ? "蛋白质" : "protein", out protein)
            ?? TryReadNumber(CarbsEntry.Text, IsZh ? "碳水" : "carbs", out carbs)
            ?? TryReadNumber(FatEntry.Text, IsZh ? "脂肪" : "fat", out fat);
    }

    private string? TryReadNumber(string? value, string fieldName, out int number)
    {
        if (int.TryParse(value, out number) && number >= 0)
            return null;

        return IsZh ? $"请输入有效的非负数值：{fieldName}。" : $"Please enter a valid non-negative number for {fieldName}.";
    }

    private void ShowValidation(string message)
    {
        ValidationLabel.Text = message;
        ValidationPanel.IsVisible = true;
        SemanticScreenReader.Announce(message);
    }
}