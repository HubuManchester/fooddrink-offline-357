using System.Text.Json.Serialization;
using FoodDrinkApp.Services; // 引入 LanguageService

namespace FoodDrinkApp.Models;

public sealed class FoodItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("calories")]
    public int Calories { get; set; }

    [JsonPropertyName("protein")]
    public int Protein { get; set; }

    [JsonPropertyName("carbs")]
    public int Carbs { get; set; }

    [JsonPropertyName("fat")]
    public int Fat { get; set; }

    [JsonPropertyName("allergyNote")]
    public string AllergyNote { get; set; } = string.Empty;

    [JsonPropertyName("tags")]
    public string Tags { get; set; } = string.Empty;

    // 新增：图片路径属性
    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;

    [JsonIgnore]
    public string CaloriesLabel => LanguageService.CurrentLanguage == "zh"
        ? $"{Calories} 千卡"
        : $"{Calories} kcal";

    // 更新：支持双语的营养素总结
    [JsonIgnore]
    public string MacroSummary => LanguageService.CurrentLanguage == "zh"
        ? $"蛋白质 {Protein}g, 碳水 {Carbs}g, 脂肪 {Fat}g"
        : $"Protein {Protein}g, carbs {Carbs}g, fat {Fat}g";

    [JsonIgnore]
    public string AccessibleSummary => $"{Name}. {Category}. {CaloriesLabel}. {MacroSummary}. {AllergyNote}";
}