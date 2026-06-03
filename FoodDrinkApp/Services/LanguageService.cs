namespace FoodDrinkApp.Services;

public static class LanguageService
{
    public static string CurrentLanguage { get; set; } = "en";

    private static readonly Dictionary<string, Dictionary<string, string>> LocalizedTexts = new()
    {
        {
            "en", new Dictionary<string, string>
            {
                // MainPage (首页)
                { "MainTitle", "Dietary Nutrient Assistant" },
                { "SearchPlaceholder", "Search food, drink, category, or tags..." },
                { "AddRecordBtn", "Add Record" },
                { "DetailsBtn", "Details" },

                // AddItemPage (添加页)
                { "AddTitle", "Add food or drink" },
                { "AddDesc", "Record the full details of a meal worth remembering." },
                { "BasicInfoSection", "Basic information" },
                { "NamePlaceholder", "Name" },
                { "CategoryTitle", "Category" },
                { "DescPlaceholder", "Description, ingredients, flavour, or context" },
                { "NutritionSection", "Nutrition data" },
                { "CaloriesPlaceholder", "Calories" },
                { "ProteinPlaceholder", "Protein g" },
                { "CarbsPlaceholder", "Carbs g" },
                { "FatPlaceholder", "Fat g" },
                { "AllergyPlaceholder", "Allergy note or dietary restriction" },
                { "SaveRecordBtn", "Save record" },

                // HardwarePage (硬件页)
                { "HardwareTitle", "Mobile hardware" },
                { "HardwareDesc", "Demonstrate camera, location, speech, vibration, and haptic feedback." },
                { "FoodPhotoTitle", "Food photo" },
                { "FoodPhotoDesc", "Capture the colour and condition of the meal." },
                { "PhotoBtn", "Photo" },
                { "MealLocationTitle", "Meal location" },
                { "MealLocationDesc", "Show country, city, region, and coordinates." },
                { "LocateBtn", "Locate" },
                { "LocationDefault", "Location has not been captured yet." },
                { "CoordinateDefault", "Coordinates will appear here." },
                { "ReadHelpBtn", "Read help" },
                { "StopSpeechBtn", "Stop speech" },
                { "HapticBtn", "Haptic feedback" },

                // SettingsPage (设置页)
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
                // MainPage (首页)
                { "MainTitle", "食光营养助手" },
                { "SearchPlaceholder", "搜索食品、饮品、分类或标签..." },
                { "AddRecordBtn", "添加记录" },
                { "DetailsBtn", "详情" },

                // AddItemPage (添加页)
                { "AddTitle", "添加食品或饮品" },
                { "AddDesc", "记录一顿值得铭记的餐食的完整细节。" },
                { "BasicInfoSection", "基本信息" },
                { "NamePlaceholder", "名称" },
                { "CategoryTitle", "分类" },
                { "DescPlaceholder", "描述、配料、口味或上下文" },
                { "NutritionSection", "营养数据" },
                { "CaloriesPlaceholder", "热量 (千卡)" },
                { "ProteinPlaceholder", "蛋白质 (克)" },
                { "CarbsPlaceholder", "碳水 (克)" },
                { "FatPlaceholder", "脂肪 (克)" },
                { "AllergyPlaceholder", "过敏提示或饮食限制" },
                { "SaveRecordBtn", "保存记录" },

                // HardwarePage (硬件页)
                { "HardwareTitle", "移动设备硬件" },
                { "HardwareDesc", "演示相机、定位、语音、震动与触觉反馈能力。" },
                { "FoodPhotoTitle", "食品照片" },
                { "FoodPhotoDesc", "捕捉餐食的色彩与真实状态。" },
                { "PhotoBtn", "拍照" },
                { "MealLocationTitle", "用餐位置" },
                { "MealLocationDesc", "显示国家、城市、区域及精准经纬度。" },
                { "LocateBtn", "定位" },
                { "LocationDefault", "尚未获取用餐位置信息。" },
                { "CoordinateDefault", "经纬度坐标将在此处显示。" },
                { "ReadHelpBtn", "朗读帮助" },
                { "StopSpeechBtn", "停止朗读" },
                { "HapticBtn", "触觉反馈" },

                // SettingsPage (设置页)
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

    public static string Get(string key)
    {
        if (LocalizedTexts.TryGetValue(CurrentLanguage, out var dict) && dict.TryGetValue(key, out var text))
        {
            return text;
        }
        return key;
    }
}