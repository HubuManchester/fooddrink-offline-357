using FoodDrinkApp.Services;

namespace FoodDrinkApp;

public partial class HardwarePage : ContentPage
{
    private int feedbackTestCount;
    private bool IsZh => LanguageService.CurrentLanguage == "zh";

    public HardwarePage()
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
        Title = LanguageService.Get("HardwareTitle");
        HardwareTitleLabel.Text = LanguageService.Get("HardwareTitle");
        HardwareDescLabel.Text = LanguageService.Get("HardwareDesc");

        FoodPhotoTitleLabel.Text = LanguageService.Get("FoodPhotoTitle");
        FoodPhotoDescLabel.Text = LanguageService.Get("FoodPhotoDesc");
        TakePhotoButton.Text = LanguageService.Get("PhotoBtn");

        MealLocationTitleLabel.Text = LanguageService.Get("MealLocationTitle");
        MealLocationDescLabel.Text = LanguageService.Get("MealLocationDesc");
        LocateButton.Text = LanguageService.Get("LocateBtn");

        if (LocationLabel.Text.Contains("captured") || LocationLabel.Text.Contains("尚未"))
            LocationLabel.Text = LanguageService.Get("LocationDefault");

        if (CoordinateLabel.Text.Contains("appear") || CoordinateLabel.Text.Contains("坐标将"))
            CoordinateLabel.Text = LanguageService.Get("CoordinateDefault");

        ReadHelpButton.Text = LanguageService.Get("ReadHelpBtn");
        StopSpeechButton.Text = LanguageService.Get("StopSpeechBtn");
        HapticButton.Text = LanguageService.Get("HapticBtn");

        HardwareStatusLabel.Text = IsZh ? "准备就绪。" : "Ready.";
        FeedbackCountLabel.Text = IsZh ? $"触觉反馈测试次数：{feedbackTestCount}" : $"Haptic feedback tests: {feedbackTestCount}";
    }

    protected override void OnDisappearing()
    {
        SpeechService.Stop();
        base.OnDisappearing();
    }

    private async void OnTakePhotoClicked(object? sender, EventArgs e)
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                SetStatus(IsZh ? "该设备不支持相机功能。" : "This device does not support camera capture.");
                return;
            }

            // 【核心修复】：显式检查并请求相机权限
            // 这样系统会先等待您点击“允许”，确认拿到权限后，再继续执行下面的真实拍照代码，就不会被打断了。
            var cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (cameraStatus != PermissionStatus.Granted)
            {
                cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
                if (cameraStatus != PermissionStatus.Granted)
                {
                    SetStatus(IsZh ? "相机权限被拒绝，无法拍照。" : "Camera permission was denied.");
                    return;
                }
            }

            SetStatus(IsZh ? "正在唤起相机..." : "Opening camera...");

            // 权限验证通过后，真实唤起原生相机应用
            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo is null)
            {
                SetStatus(IsZh ? "已取消拍摄。" : "Photo capture cancelled.");
                return;
            }

            await using var stream = await photo.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();

            // 将真实拍摄的照片渲染到界面上
            FoodPhoto.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));

            SetStatus(IsZh ? "食品照片拍摄成功。" : "Food photo captured successfully.");
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch (Exception ex)
        {
            SetStatus(IsZh ? $"相机错误: {ex.Message}" : $"Camera error: {ex.Message}");
        }
    }

    private async void OnGetLocationClicked(object? sender, EventArgs e)
    {
        try
        {
            SetStatus(IsZh ? "正在获取定位..." : "Getting location...");
            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var location = await Geolocation.Default.GetLocationAsync(request);

            if (location is null)
            {
                SetStatus(IsZh ? "无法获取当前位置。" : "Current location could not be found.");
                return;
            }

            CoordinateLabel.Text = IsZh ? $"纬度 {location.Latitude:F5}, 经度 {location.Longitude:F5}" : $"Latitude {location.Latitude:F5}, longitude {location.Longitude:F5}";
            LocationLabel.Text = await BuildAddressTextAsync(location);
            SetStatus(IsZh ? "国家、城市和坐标已加载。" : "Country, city, and coordinates have been loaded.");
        }
        catch (PermissionException)
        {
            SetStatus(IsZh ? "定位权限被拒绝。" : "Location permission was denied.");
        }
        catch (Exception ex)
        {
            SetStatus(IsZh ? $"定位错误: {ex.Message}" : $"Location error: {ex.Message}");
        }
    }

    private static async Task<string> BuildAddressTextAsync(Location location)
    {
        try
        {
            var placemarks = await Geocoding.Default.GetPlacemarksAsync(location);
            var placemark = placemarks?.FirstOrDefault();
            if (placemark != null)
                return $"{placemark.CountryName} / {placemark.AdminArea} / {placemark.Locality}";
        }
        catch { }
        return LanguageService.CurrentLanguage == "zh" ? "已找到坐标，但地址解析失败。" : "Coordinates found, but address resolution failed.";
    }

    private async void OnReadHelpClicked(object? sender, EventArgs e)
    {
        try
        {
            string helpText = IsZh
                ? "食光营养助手可记录餐饮、展示营养细节，并利用相机、定位、语音与触觉反馈提升记录体验。"
                : "NutriBite records foods and drinks, shows nutrition details, and uses camera, location, speech, and haptic feedback to make meal tracking more practical.";

            await SpeechService.SpeakAsync(helpText);
            SetStatus(IsZh ? "正在朗读帮助内容。" : "Reading help content aloud.");
        }
        catch (Exception ex)
        {
            SetStatus(IsZh ? $"语音错误: {ex.Message}" : $"Text to speech error: {ex.Message}");
        }
    }

    private void OnStopSpeechClicked(object? sender, EventArgs e)
    {
        SpeechService.Stop();
        SetStatus(IsZh ? "已停止朗读。" : "Reading stopped.");
    }

    private void OnFeedbackClicked(object? sender, EventArgs e)
    {
        try
        {
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(500));
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);

            feedbackTestCount++;
            FeedbackCountLabel.Text = IsZh ? $"触觉反馈测试次数：{feedbackTestCount}" : $"Haptic feedback tests: {feedbackTestCount}";
            SetStatus(IsZh ? "触发震动与触觉反馈。" : "Half-second vibration and haptic feedback triggered.");
        }
        catch (Exception ex)
        {
            SetStatus(IsZh ? $"反馈错误: {ex.Message}" : $"Feedback error: {ex.Message}");
        }
    }

    private void SetStatus(string message)
    {
        HardwareStatusLabel.Text = message;
        SemanticScreenReader.Announce(message);
    }
}