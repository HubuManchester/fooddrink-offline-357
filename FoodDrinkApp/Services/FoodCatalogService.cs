using FoodDrinkApp.Models;

namespace FoodDrinkApp.Services;

public static class FoodCatalogService
{
    public static bool LastLoadUsedMockApi { get; private set; } = false;

    // 根据当前语言动态生成模拟数据
    private static List<FoodItem> GetMockData()
    {
        bool isZh = LanguageService.CurrentLanguage == "zh";

        return new List<FoodItem>
        {
            new FoodItem
            {
                Name = isZh ? "浆果酸奶碗" : "Berry Yogurt Bowl",
                Category = isZh ? "早餐" : "Breakfast",
                Description = isZh ? "希腊酸奶配以新鲜浆果、坚果和少许蜂蜜。" : "Greek yogurt topped with fresh berries, nuts, and a drizzle of honey.",
                Calories = 320, Protein = 15, Carbs = 35, Fat = 12,
                AllergyNote = isZh ? "含乳制品和坚果。" : "Contains dairy and tree nuts.",
                ImageUrl = "berry_yogurt_bowl.jpg" // 请确保项目中图片已重命名为此全小写格式
            },
            new FoodItem
            {
                Name = isZh ? "鸡肉糙米饭盒" : "Chicken Brown Rice Box",
                Category = isZh ? "午餐" : "Lunch",
                Description = isZh ? "烤鸡胸肉搭配健康糙米、樱桃番茄和青豆。" : "Grilled chicken breast paired with healthy brown rice, cherry tomatoes, and peas.",
                Calories = 450, Protein = 35, Carbs = 45, Fat = 10,
                AllergyNote = isZh ? "无特殊过敏原。" : "No common allergens.",
                ImageUrl = "chicken_brown_rice_box.jpg"
            },
            new FoodItem
            {
                Name = isZh ? "冰抹茶拿铁" : "Iced Matcha Latte",
                Category = isZh ? "饮品" : "Drink",
                Description = isZh ? "优质抹茶粉搭配冰块与丝滑牛奶。" : "Premium matcha powder mixed with ice and smooth milk.",
                Calories = 180, Protein = 6, Carbs = 22, Fat = 7,
                AllergyNote = isZh ? "含乳制品。" : "Contains dairy.",
                ImageUrl = "iced_matcha_latte.jpg"
            },
            new FoodItem
            {
                Name = isZh ? "番茄全麦意面" : "Tomato Wholegrain Pasta",
                Category = isZh ? "晚餐" : "Dinner",
                Description = isZh ? "全麦意大利面拌以浓郁的番茄罗勒肉酱。" : "Whole wheat pasta tossed in a rich tomato basil meat sauce.",
                Calories = 520, Protein = 22, Carbs = 68, Fat = 14,
                AllergyNote = isZh ? "含麸质。" : "Contains gluten.",
                ImageUrl = "tomato_wholegrain_pasta.jpg"
            }
        };
    }

    public static async Task<IEnumerable<FoodItem>> SearchAsync(string? query = null)
    {
        // 模拟网络延迟
        await Task.Delay(300);

        LastLoadUsedMockApi = false;
        var data = GetMockData();

        if (string.IsNullOrWhiteSpace(query))
            return data;

        var lowerQuery = query.ToLowerInvariant();
        return data.Where(f =>
            f.Name.ToLowerInvariant().Contains(lowerQuery) ||
            f.Category.ToLowerInvariant().Contains(lowerQuery) ||
            f.Description.ToLowerInvariant().Contains(lowerQuery) ||
            f.Tags.ToLowerInvariant().Contains(lowerQuery));
    }

    public static async Task AddAsync(FoodItem item)
    {
        // 模拟添加操作延迟
        await Task.Delay(200);
    }
}