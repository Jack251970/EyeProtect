using Microsoft.UI.Xaml;

namespace EyeProtect.Contracts.Services;

public interface IAppSettingsService
{
    void Initialize();

    string Language { get; }

    Task SetLanguageAsync(string language);

    ElementTheme Theme { get; }

    Task SetThemeAsync(ElementTheme theme);

    BackdropType BackdropType { get; }

    Task SetBackdropAsync(BackdropType type);
}
