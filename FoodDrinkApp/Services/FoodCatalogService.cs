using System.Net.Http.Json;
using System.Text.Json;
using FoodDrinkApp.Models;

namespace FoodDrinkApp.Services;

public static class FoodCatalogService
{
    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(12)
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static string _lastLanguage = "en";

    private static List<FoodItem> GetLocalizedFallbackItems()
    {
        bool isZh = LanguageService.CurrentLanguage == "zh";
        return new List<FoodItem>
        {
            new FoodItem
            {
                Id = "1",
                Name = isZh ? "浆果酸奶碗" : "Berry Yogurt Bowl",
                Category = isZh ? "早餐" : "Breakfast",
                Description = isZh ? "希腊酸奶配以新鲜浆果、坚果和少许蜂蜜。" : "Greek yogurt topped with fresh berries, nuts, and a drizzle of honey.",
                Calories = 320, Protein = 15, Carbs = 35, Fat = 12,
                AllergyNote = isZh ? "含乳制品和坚果。" : "Contains dairy and tree nuts.",
                ImageUrl = "berry_yogurt_bowl.jpg"
            },
            new FoodItem
            {
                Id = "2",
                Name = isZh ? "鸡肉糙米饭盒" : "Chicken Brown Rice Box",
                Category = isZh ? "午餐" : "Lunch",
                Description = isZh ? "烤鸡胸肉搭配健康糙米、樱桃番茄和青豆。" : "Grilled chicken breast paired with healthy brown rice, cherry tomatoes, and peas.",
                Calories = 450, Protein = 35, Carbs = 45, Fat = 10,
                AllergyNote = isZh ? "无特殊过敏原。" : "No common allergens.",
                ImageUrl = "chicken_brown_rice_box.jpg"
            },
            new FoodItem
            {
                Id = "3",
                Name = isZh ? "冰抹茶拿铁" : "Iced Matcha Latte",
                Category = isZh ? "饮品" : "Drink",
                Description = isZh ? "优质抹茶粉搭配冰块与丝滑牛奶。" : "Premium matcha powder mixed with ice and smooth milk.",
                Calories = 180, Protein = 6, Carbs = 22, Fat = 7,
                AllergyNote = isZh ? "含乳制品。" : "Contains dairy.",
                ImageUrl = "iced_matcha_latte.jpg"
            },
            new FoodItem
            {
                Id = "4",
                Name = isZh ? "番茄全麦意面" : "Tomato Wholegrain Pasta",
                Category = isZh ? "晚餐" : "Dinner",
                Description = isZh ? "全麦意大利面拌以浓郁的番茄罗勒肉酱。" : "Whole wheat pasta tossed in a rich tomato basil meat sauce.",
                Calories = 520, Protein = 22, Carbs = 68, Fat = 14,
                AllergyNote = isZh ? "含麸质。" : "Contains gluten.",
                ImageUrl = "tomato_wholegrain_pasta.jpg"
            }
        };
    }

    private static List<FoodItem> cachedItems = GetLocalizedFallbackItems();

    public static bool LastLoadUsedMockApi { get; private set; }

    public static async Task<IReadOnlyList<FoodItem>> SearchAsync(string? query)
    {
        var items = await GetAllAsync();

        if (string.IsNullOrWhiteSpace(query))
        {
            return items.OrderBy(item => item.Name).ToList();
        }

        var normalised = query.Trim().ToLowerInvariant();
        return items
            .Where(item =>
                item.Name.ToLowerInvariant().Contains(normalised) ||
                item.Category.ToLowerInvariant().Contains(normalised) ||
                item.Description.ToLowerInvariant().Contains(normalised) ||
                item.Tags.ToLowerInvariant().Contains(normalised))
            .OrderBy(item => item.Name)
            .ToList();
    }

    public static async Task<FoodItem?> GetByIdAsync(string id)
    {
        if (MockApiConfig.IsConfigured)
        {
            try
            {
                var item = await HttpClient.GetFromJsonAsync<FoodItem>(
                    $"{MockApiConfig.EndpointUrl.TrimEnd('/')}/{Uri.EscapeDataString(id)}",
                    JsonOptions);

                if (item is not null)
                {
                    return item;
                }
            }
            catch
            {
                // Fallback to local
            }
        }

        await GetAllAsync();
        return cachedItems.FirstOrDefault(item => item.Id == id);
    }

    public static async Task<FoodItem> AddAsync(FoodItem item)
    {
        if (MockApiConfig.IsConfigured)
        {
            var response = await HttpClient.PostAsJsonAsync(MockApiConfig.EndpointUrl, item, JsonOptions);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<FoodItem>(JsonOptions);
            if (created is not null)
            {
                cachedItems.Add(created);
                return created;
            }
        }

        cachedItems.Add(item);
        return item;
    }

    private static async Task<IReadOnlyList<FoodItem>> GetAllAsync()
    {
        if (!LastLoadUsedMockApi && _lastLanguage != LanguageService.CurrentLanguage)
        {
            _lastLanguage = LanguageService.CurrentLanguage;

            var userAddedItems = cachedItems.Where(i => i.Id != "1" && i.Id != "2" && i.Id != "3" && i.Id != "4").ToList();

            cachedItems = GetLocalizedFallbackItems();
            cachedItems.AddRange(userAddedItems);
        }

        if (!MockApiConfig.IsConfigured)
        {
            LastLoadUsedMockApi = false;
            return cachedItems;
        }

        try
        {
            var items = await HttpClient.GetFromJsonAsync<List<FoodItem>>(MockApiConfig.EndpointUrl, JsonOptions);
            if (items is { Count: > 0 })
            {
                cachedItems = items;
                LastLoadUsedMockApi = true;
                return cachedItems;
            }
        }
        catch
        {
            // Network fallback
        }

        LastLoadUsedMockApi = false;
        return cachedItems;
    }
}