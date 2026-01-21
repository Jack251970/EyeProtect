# EyeProtect WPF to WinUI3 Migration Guide

## Overview
This document provides a comprehensive guide for completing the migration of EyeProtect from WPF to WinUI3 using the template structure from [WinUI3-Template](https://github.com/Jack251970/WinUI3-Template).

## Migration Status

### ✅ Completed
1. **Project Structure Created**
   - `src/EyeProtect/` - Main WinUI3 UI project
   - `src/EyeProtect.Core/` - Business logic and UI utilities
   - `src/EyeProtect.Infrastructure/` - System services and file operations

2. **Template Integration**
   - All template files copied and adapted
   - Namespaces updated from `WinUI3Template` to `EyeProtect`
   - Project references configured correctly
   - Build configuration files in place

3. **Business Logic Migration**
   - All services copied to `EyeProtect.Core/Services/EyeProtection/`
   - All models copied to `EyeProtect.Core/Models/EyeProtection/`
   - All helpers copied to `EyeProtect.Core/Helpers/EyeProtection/`
   - Network classes copied to `EyeProtect.Infrastructure/Net/`

4. **Resources Migration**
   - Icons, images, sounds copied to `src/EyeProtect/Assets/Resources/`
   - Language files (en, zh-cn, zh-tw) copied
   - Fonts copied
   - Theme resources copied

### 🔄 Remaining Work

#### 1. Service Adaptation (WPF → WinUI3)
The following services need to be adapted from WPF to WinUI3 APIs:

**TrayService.cs**
- Replace WPF tray icon with H.NotifyIcon (already in template)
- Update context menu creation to use WinUI3 controls
- Adapt icon change logic

**ThemeService.cs**
- Remove WPF ResourceDictionary usage
- Use template's ThemeSelectorService instead
- Update theme application logic

**MainService.cs**
- Replace WPF Dispatcher with DispatcherQueue
- Update window management using WinUIEx
- Adapt timer implementations if needed

**RestService.cs**
- Replace WPF Dispatcher with DispatcherQueue
- Update UI update logic

**KeyboardShortcutsService.cs**
- Verify Win32 API calls work with WinUI3
- May need to use template's Win32 helpers

**ScreenService.cs**
- Use template's DisplayMonitor helpers
- Update multi-monitor detection

**NotificationService.cs**
- Replace with template's AppNotificationService
- Use Windows.UI.Notifications

**ConfigService.cs**
- Consider using LocalSettingsService from template
- Or keep XML-based config but update paths

#### 2. UI Components Migration

**Windows to Convert:**
- `OptionsWindow.xaml` → Settings page (integrate with template's SettingsPage)
- `EyesTestWindow.xaml` → New page in EyeProtect
- `TipWindow.xaml` → Dialog using DialogService
- `ContributorsWindow.xaml` → New page or dialog
- `UpdateWindow.xaml` → Dialog or page

**Key Changes:**
```xaml
<!-- WPF -->
<Window x:Class="ProjectEye.OptionsWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">

<!-- WinUI3 -->
<Page x:Class="EyeProtect.Views.Pages.OptionsPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
```

**Control Mapping:**
- WPF `System.Windows.Controls.Button` → WinUI3 `Microsoft.UI.Xaml.Controls.Button`
- WPF `TextBox` → WinUI3 `TextBox`
- WPF `CheckBox` → WinUI3 `CheckBox`
- WPF `ComboBox` → WinUI3 `ComboBox`
- WPF `Slider` → WinUI3 `Slider`
- WPF `TabControl` → WinUI3 `NavigationView` or `TabView`

#### 3. Service Registration

Update `App.xaml.cs` to register eye protection services:

```csharp
// In ConfigureServices method, add:
services.AddSingleton<MainService>();
services.AddSingleton<RestService>();
services.AddSingleton<ConfigService>();
services.AddSingleton<SoundService>();
services.AddSingleton<ScreenService>();
services.AddSingleton<PreAlertService>();
services.AddSingleton<EyesTestService>();
services.AddSingleton<KeyboardShortcutsService>();
services.AddSingleton<CacheService>();
services.AddSingleton<BackgroundWorkerService>();
services.AddSingleton<SystemResourcesService>();
```

#### 4. Dispatcher Migration

**WPF:**
```csharp
Dispatcher.Invoke(() => { /* UI code */ });
```

**WinUI3:**
```csharp
DispatcherQueue.TryEnqueue(() => { /* UI code */ });
```

#### 5. Window Management

**WPF:**
```csharp
var window = new SomeWindow();
window.Show();
```

**WinUI3:**
```csharp
var window = new Window();
window.Content = new SomePage();
window.Activate();
```

#### 6. Resource Dictionary Updates

Convert WPF theme XAML to WinUI3:
- Update `Assets/Resources/Language/*.xaml` to use WinUI3 syntax
- Convert theme files in `Assets/Resources/Themes/` to WinUI3 styles
- Use WinUI3 Fluent Design resources

#### 7. Configuration

Update file paths:
- WPF used `AppDomain.CurrentDirectory`
- WinUI3 uses `ApplicationData.Current.LocalFolder` (packaged) or `Environment.GetFolderPath` (unpackaged)
- Template's LocalSettingsHelper already handles this

#### 8. Icon and Asset Configuration

Update `.csproj` to include assets:
```xml
<ItemGroup>
  <Content Include="Assets\Resources\**\*.*">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

## Build Instructions

**Prerequisites:**
- Windows 10 version 1809 (Build 17763) or later
- Visual Studio 2022 version 17.5 or later
- Windows App SDK 1.8 or later
- .NET 9.0 SDK

**Steps:**
1. Open `EyeProtect.sln` in Visual Studio 2022
2. Restore NuGet packages
3. Set `EyeProtect` as startup project
4. Select platform (x64, x86, or ARM64)
5. Build solution (Ctrl+Shift+B)
6. Run (F5)

**Note:** The project cannot be built on Linux/macOS due to WinUI3 dependencies.

## Testing Checklist

After completing the migration, test:

- [ ] Application launches successfully
- [ ] System tray icon appears and functions
- [ ] Timer starts and counts correctly
- [ ] Rest notifications appear at correct intervals
- [ ] Pre-alerts work (1-minute warnings)
- [ ] Rest window displays correctly
- [ ] Settings can be saved and loaded
- [ ] Theme switching works
- [ ] Multi-monitor support works
- [ ] Keyboard shortcuts function
- [ ] Sound notifications play
- [ ] Eye test functionality works
- [ ] Multi-language support works
- [ ] Startup on login works
- [ ] Window state persistence works
- [ ] Crash recovery works

## Key Differences: WPF vs WinUI3

| Feature | WPF | WinUI3 |
|---------|-----|--------|
| Namespace | System.Windows | Microsoft.UI.Xaml |
| Threading | Dispatcher | DispatcherQueue |
| Window | System.Windows.Window | Microsoft.UI.Xaml.Window |
| Page Navigation | Frame.Navigate | Frame.Navigate (similar) |
| Resource Dictionary | MergedDictionaries | ResourceDictionary.MergedDictionaries |
| Styling | BasedOn | BasedOn (similar) |
| Binding | {Binding Path=...} | {x:Bind ViewModel...} (compiled) |
| Icon Tray | WPF native or library | H.NotifyIcon.WinUI |
| File Access | System.IO | Windows.Storage (packaged) |

## Template Features Available

The template provides these features that can be leveraged:

1. **Theme System** - Light/Dark/System theme support
2. **Backdrop System** - Acrylic, Mica, Mica Alt backdrops
3. **Navigation System** - Page navigation with NavigationView
4. **Settings Service** - JSON-based local settings
5. **Dialog Service** - Easy dialog creation
6. **Tray Icon** - System tray with context menu
7. **Splash Screen** - Loading screen support
8. **Single Instance** - Prevents multiple app instances
9. **Startup Task** - Run on Windows startup
10. **Logging** - Serilog integration
11. **Multi-language** - Localization support

## Recommended Integration Strategy

1. **Phase 1: Core Services (Current)**
   - ✅ Copy all existing code
   - ✅ Update namespaces
   - ✅ Add NuGet packages

2. **Phase 2: Service Adaptation**
   - Adapt services to use WinUI3 APIs
   - Replace Dispatcher with DispatcherQueue
   - Update TrayService to use H.NotifyIcon

3. **Phase 3: UI Migration**
   - Convert XAML pages to WinUI3
   - Update control bindings
   - Adapt styles and themes

4. **Phase 4: Integration**
   - Register services in App.xaml.cs
   - Wire up navigation
   - Connect tray menu to pages

5. **Phase 5: Testing**
   - Build on Windows
   - Test all functionality
   - Fix issues

6. **Phase 6: Polish**
   - Update icons and assets
   - Improve UI with Fluent Design
   - Add animations

## Known Issues & Solutions

### Issue: MakePri.exe cannot run on Linux
**Solution:** Build must be done on Windows. This is a WinUI3 limitation.

### Issue: Dispatcher.Invoke not found
**Solution:** Replace with `DispatcherQueue.TryEnqueue()`

### Issue: Window.Show() not found
**Solution:** Use `Window.Activate()` instead

### Issue: ResourceDictionary not loading
**Solution:** Check paths and ensure XAML is marked as "Content" in .csproj

## Additional Resources

- [WinUI3 Documentation](https://docs.microsoft.com/windows/apps/winui/winui3/)
- [WinUI3 Template Repository](https://github.com/Jack251970/WinUI3-Template)
- [WinUI3 Migration Guide](https://docs.microsoft.com/windows/apps/windows-app-sdk/migrate-to-windows-app-sdk/)
- [XAML Controls Gallery](https://github.com/microsoft/Xaml-Controls-Gallery)

## Contact

For questions or issues with the migration, refer to the original ProjectEye repository or the WinUI3 template documentation.

---

**Last Updated:** 2025-01-21
**Status:** Phase 1 Complete, Phase 2 In Progress
