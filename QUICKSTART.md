# Quick Start Guide for Windows Development

This guide helps you continue the EyeProtect WPF to WinUI3 migration on a Windows machine.

## Prerequisites Setup

1. **Install Visual Studio 2022** (17.5 or later)
   - Download from: https://visualstudio.microsoft.com/
   - Select workloads:
     - ✅ .NET Desktop Development
     - ✅ Universal Windows Platform development
   - Individual components:
     - ✅ Windows App SDK C# Templates
     - ✅ .NET 9.0 Runtime

2. **Verify Installation**
   ```powershell
   dotnet --version  # Should show 9.0.x
   ```

## First Build

1. **Clone and Open**
   ```powershell
   cd path\to\EyeProtect
   start EyeProtect.sln
   ```

2. **Restore Packages**
   - Visual Studio will automatically restore NuGet packages
   - Or manually: `Tools → NuGet Package Manager → Restore NuGet Packages`

3. **Set Startup Project**
   - Right-click `EyeProtect` project → Set as Startup Project

4. **Select Platform**
   - Choose `x64` in the platform dropdown (toolbar)

5. **Build**
   - Press `Ctrl+Shift+B`
   - Expected: Build will fail with errors (services not adapted yet)

## Fixing Initial Build Errors

### Error 1: Dispatcher References

**Files to fix:** Services in `EyeProtect.Core/Services/EyeProtection/`

**WPF code:**
```csharp
using System.Windows.Threading;
Dispatcher.Invoke(() => { /* code */ });
```

**Fix to:**
```csharp
using Microsoft.UI.Dispatching;
DispatcherQueue.TryEnqueue(() => { /* code */ });
```

**Helper method to add:**
```csharp
private DispatcherQueue? _dispatcherQueue;
private DispatcherQueue DispatcherQueue => 
    _dispatcherQueue ??= DispatcherQueue.GetForCurrentThread();
```

### Error 2: Window References

**Files to fix:** TrayService, MainService

**WPF code:**
```csharp
using System.Windows;
Window window = new SomeWindow();
window.Show();
```

**Fix to:**
```csharp
using Microsoft.UI.Xaml;
var window = new Window();
window.Content = new SomePage();
window.Activate();
```

### Error 3: ResourceDictionary

**Files to fix:** ThemeService

**WPF code:**
```csharp
Application.Current.Resources.MergedDictionaries.Add(dict);
```

**Fix to:**
```csharp
// Use template's ThemeSelectorService instead
var themeService = App.GetService<IThemeSelectorService>();
themeService.SetTheme(ElementTheme.Dark);
```

## Priority Tasks

### Task 1: Adapt MainService (30 min)

**File:** `src/EyeProtect.Core/Services/EyeProtection/MainService.cs`

1. Replace Dispatcher with DispatcherQueue
2. Update timer implementations if needed
3. Remove WPF window dependencies

**Steps:**
```csharp
// Find and replace:
// Dispatcher.Invoke -> DispatcherQueue.TryEnqueue
// System.Windows.Threading -> Microsoft.UI.Dispatching

// Add constructor injection:
private readonly DispatcherQueue _dispatcherQueue;

public MainService()
{
    _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
}

// Update all Dispatcher calls:
_dispatcherQueue.TryEnqueue(() => 
{
    // UI update code
});
```

### Task 2: Update TrayService (20 min)

**File:** `src/EyeProtect.Core/Services/EyeProtection/TrayService.cs`

The template already has H.NotifyIcon setup. Update TrayService to use it:

```csharp
// See template's TrayMenuControl.xaml for reference
// Update menu creation to use WinUI3 MenuFlyout
```

### Task 3: Register Services (10 min)

**File:** `src/EyeProtect/App.xaml.cs`

In the `ConfigureServices` method, add:

```csharp
private void ConfigureServices(IServiceCollection services)
{
    // Existing template services...
    
    // Add eye protection services:
    services.AddSingleton<BackgroundWorkerService>();
    services.AddSingleton<SystemResourcesService>();
    services.AddSingleton<CacheService>();
    services.AddSingleton<ConfigService>();
    services.AddSingleton<ScreenService>();
    services.AddSingleton<MainService>();
    services.AddSingleton<TrayService>();
    services.AddSingleton<RestService>();
    services.AddSingleton<SoundService>();
    services.AddSingleton<KeyboardShortcutsService>();
    services.AddSingleton<PreAlertService>();
    services.AddSingleton<EyesTestService>();
}
```

### Task 4: Convert Settings Page (45 min)

**Reference:** `src/ProjectEye/Views/OptionsWindow.xaml`

**Target:** Update `src/EyeProtect/Views/Pages/SettingsPage.xaml`

1. Copy settings from OptionsWindow
2. Convert WPF controls to WinUI3
3. Use SettingsCard from CommunityToolkit
4. Update ViewModel

**Example conversion:**
```xaml
<!-- WPF -->
<StackPanel>
    <TextBlock Text="General Settings" />
    <CheckBox Content="Auto start" IsChecked="{Binding AutoStart}" />
</StackPanel>

<!-- WinUI3 -->
<StackPanel Spacing="4">
    <TextBlock Text="General Settings" Style="{StaticResource SubtitleTextBlockStyle}" />
    <labs:SettingsCard Header="Auto start" Description="Start with Windows">
        <ToggleSwitch IsOn="{x:Bind ViewModel.AutoStart, Mode=TwoWay}" />
    </labs:SettingsCard>
</StackPanel>
```

## Testing

After each fix:

1. **Build** (`Ctrl+Shift+B`)
2. **Run** (`F5`)
3. **Check console** for errors
4. **Test feature** works correctly

## Common Issues & Solutions

### Issue: "Type or namespace could not be found"
**Solution:** Add using directive or check project references

### Issue: "Dispatcher not found"
**Solution:** Replace with DispatcherQueue

### Issue: "Window.Show() doesn't exist"
**Solution:** Use Window.Activate()

### Issue: "Assets not found"
**Solution:** Check `.csproj` includes assets with CopyToOutputDirectory

## Development Workflow

1. **Pick a service** from EyeProtection folder
2. **Identify WPF-specific code** (Dispatcher, Window, etc.)
3. **Replace with WinUI3 equivalents**
4. **Build and test**
5. **Commit changes**
6. **Move to next service**

## Debugging Tips

1. **Enable detailed logging:**
   - Check `appsettings.json`
   - Logs are in: `%AppData%\EyeProtect\ApplicationData\Logs\`

2. **Use breakpoints:**
   - Set breakpoints in services
   - Check if code is being called

3. **Check Dispatcher context:**
   - Ensure UI updates are on UI thread
   - Use DispatcherQueue for UI updates

## Estimated Time

- **Basic adaptation:** 4-6 hours
  - Services: 3-4 hours
  - UI pages: 2-3 hours
  - Testing: 1 hour

- **Full migration:** 2-3 days
  - All services adapted
  - All UI converted
  - Theme system updated
  - Full testing

## Help & Resources

- **Template examples:** See existing services in `EyeProtect/Services/`
- **WinUI3 docs:** https://docs.microsoft.com/windows/apps/winui/winui3/
- **XAML Gallery:** https://github.com/microsoft/Xaml-Controls-Gallery
- **Migration guide:** See `MIGRATION_GUIDE.md`

## Next Steps After Initial Build

1. ✅ Build succeeds
2. ✅ App launches
3. ✅ Main window appears
4. Implement eye tracking timer
5. Add rest notification window
6. Test all features
7. Polish UI
8. Final testing

## Getting Help

If you encounter issues:

1. Check `MIGRATION_GUIDE.md` for detailed information
2. Review template examples in the project
3. Check WinUI3 documentation
4. Open an issue on GitHub

---

**Ready to start?** Open Visual Studio 2022 and begin with Task 1!

Good luck! 🚀
