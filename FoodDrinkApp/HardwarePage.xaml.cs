using FoodDrinkApp.Services;

namespace FoodDrinkApp;

public partial class HardwarePage : ContentPage
{
    private int feedbackTestCount;

    public HardwarePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
    }

    protected override void OnDisappearing()
    {
        SpeechService.Stop();
        base.OnDisappearing();
    }

    // 1. 硬件功能：相机 (Camera)
    private async void OnTakePhotoClicked(object? sender, EventArgs e)
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                SetStatus("This device does not support camera capture.");
                return;
            }

            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo is null) return;

            await using var stream = await photo.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();
            FoodPhoto.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
            SetStatus("Food photo captured successfully.");
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch (Exception ex)
        {
            SetStatus($"Camera error: {ex.Message}");
        }
    }

    // 2. 硬件功能：定位与地理编码 (Location & Geocoding)
    private async void OnGetLocationClicked(object? sender, EventArgs e)
    {
        try
        {
            SetStatus("Getting location...");
            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var location = await Geolocation.Default.GetLocationAsync(request);

            if (location is null) return;

            CoordinateLabel.Text = $"Latitude {location.Latitude:F5}, longitude {location.Longitude:F5}";
            LocationLabel.Text = await BuildAddressTextAsync(location);
            SetStatus("Country, city, and coordinates have been loaded.");
        }
        catch (Exception ex)
        {
            SetStatus($"Location error: {ex.Message}");
        }
    }

    private static async Task<string> BuildAddressTextAsync(Location location)
    {
        try
        {
            var placemarks = await Geocoding.Default.GetPlacemarksAsync(location);
            var placemark = placemarks?.FirstOrDefault();
            if (placemark != null)
            {
                return $"{placemark.CountryName} / {placemark.AdminArea} / {placemark.Locality}";
            }
        }
        catch { }
        return "Coordinates found, but address resolution failed.";
    }

    // 3. 硬件功能：文字转语音 (Text-to-Speech)
    private async void OnReadHelpClicked(object? sender, EventArgs e)
    {
        try
        {
            const string helpText = "NutriBite records foods and drinks, shows nutrition details, and uses camera, location, speech, and haptic feedback to make meal tracking more practical.";
            await SpeechService.SpeakAsync(helpText);
            SetStatus("Reading help content aloud.");
        }
        catch (Exception ex)
        {
            SetStatus($"Text to speech error: {ex.Message}");
        }
    }

    private void OnStopSpeechClicked(object? sender, EventArgs e)
    {
        SpeechService.Stop();
        SetStatus("Reading stopped.");
    }

    // 4. 硬件功能：震动与触觉反馈 (Vibration & Haptic Feedback)
    private void OnFeedbackClicked(object? sender, EventArgs e)
    {
        try
        {
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(450));
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
            feedbackTestCount++;
            FeedbackCountLabel.Text = $"Haptic feedback tests: {feedbackTestCount}";
            SetStatus("Vibration and haptic feedback triggered.");
        }
        catch (Exception ex)
        {
            SetStatus($"Feedback error: {ex.Message}");
        }
    }

    private void SetStatus(string message)
    {
        HardwareStatusLabel.Text = message;
        SemanticScreenReader.Announce(message);
    }
}