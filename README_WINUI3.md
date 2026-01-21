# EyeProtect - WinUI3

A modern 20-20-20 eye protection reminder tool built with WinUI3, migrated from the original WPF application.

## 📖 About

EyeProtect is an eye health application that follows the 20-20-20 rule: every 20 minutes, take a 20-second break and look at something 20 feet away. This helps reduce eye strain from prolonged computer use.

This version has been completely rebuilt using WinUI3 and the Windows App SDK, providing a modern Windows 11-style interface with Fluent Design.

## ✨ Features

### Core Features
- ⏱️ **20-20-20 Timer** - Automated reminders following the 20-20-20 rule
- 🔔 **Notifications** - Visual and audio alerts for break times
- 📊 **Eye Tests** - Built-in vision testing tools
- 🎨 **Themes** - Light, Dark, and System theme support
- 🌍 **Multi-language** - English, Simplified Chinese, Traditional Chinese
- 🖥️ **Multi-monitor** - Support for multiple displays
- ⌨️ **Keyboard Shortcuts** - Global hotkey support
- 🔊 **Sound Alerts** - Configurable audio notifications
- ⚙️ **Customizable** - Adjustable timer intervals and behaviors

### WinUI3 Template Features
- 🎭 **Backdrop Effects** - Acrylic, Mica, Mica Alt backgrounds
- 🚀 **Splash Screen** - Smooth loading experience
- 📍 **System Tray** - Minimize to tray functionality
- 🔒 **Single Instance** - Prevents multiple app instances
- 📦 **Modern Packaging** - MSIX packaging support
- 🪵 **Logging** - Structured logging with Serilog
- 💾 **Settings** - JSON-based configuration storage

## 🚀 Getting Started

### Prerequisites
- Windows 10 version 1809 (Build 17763) or later
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (version 17.5 or later) with:
  - .NET Desktop Development workload
  - Universal Windows Platform development workload
  - Windows App SDK C# Templates

### Building from Source

1. **Clone the repository**
   ```bash
   git clone https://github.com/Jack251970/EyeProtect.git
   cd EyeProtect
   ```

2. **Open the solution**
   ```bash
   # Open in Visual Studio 2022
   start EyeProtect.sln
   ```
   
3. **Restore packages**
   - Visual Studio will automatically restore NuGet packages
   - Or manually: `dotnet restore EyeProtect.sln`

4. **Build the solution**
   - Press `Ctrl+Shift+B` in Visual Studio
   - Or: `dotnet build EyeProtect.sln`

5. **Run the application**
   - Press `F5` in Visual Studio
   - Or: Set `EyeProtect` as startup project and run

### Project Structure

```
EyeProtect/
├── src/
│   ├── EyeProtect/                    # Main WinUI3 application
│   │   ├── Views/                     # XAML pages and windows
│   │   ├── ViewModels/                # MVVM view models
│   │   ├── Services/                  # Application services
│   │   ├── Assets/                    # Images, icons, resources
│   │   └── App.xaml.cs                # Application entry point
│   │
│   ├── EyeProtect.Core/               # Core business logic
│   │   ├── Services/EyeProtection/    # Eye tracking services
│   │   ├── Models/EyeProtection/      # Data models
│   │   ├── Helpers/EyeProtection/     # Utility helpers
│   │   └── ...                        # Template core features
│   │
│   ├── EyeProtect.Infrastructure/     # Infrastructure layer
│   │   ├── Services/                  # File, startup services
│   │   ├── Helpers/                   # JSON, exception helpers
│   │   └── Net/                       # Network operations
│   │
│   └── ProjectEye/                    # Original WPF project (reference)
│
├── EyeProtect.sln                     # Solution file
└── MIGRATION_GUIDE.md                 # Migration documentation
```

## 📋 Migration Status

This is an ongoing migration from WPF to WinUI3. See [MIGRATION_GUIDE.md](MIGRATION_GUIDE.md) for details.

### ✅ Completed
- Project structure setup
- Template integration
- Business logic migration
- Resources migration
- Namespace updates

### 🔄 In Progress
- Service adaptation (WPF → WinUI3)
- UI component conversion
- Service registration
- Testing and debugging

### ⏳ Planned
- UI polish with Fluent Design
- Performance optimization
- Enhanced features
- Comprehensive testing

## 🎯 Usage

### Basic Operation
1. Launch EyeProtect
2. The timer starts automatically
3. Work for 20 minutes
4. Take a 20-second break when notified
5. Look at something 20 feet away

### System Tray
- Click the tray icon to access quick options
- Right-click for context menu
- Temporarily disable reminders (1hr, 2hr, Forever)

### Settings
- Click ⚙️ Settings in the navigation menu
- Configure timer intervals
- Adjust themes and appearance
- Set keyboard shortcuts
- Configure startup behavior

### Keyboard Shortcuts
- Global hotkeys can be configured in Settings
- Default shortcuts (to be configured):
  - Pause/Resume timer
  - Force break
  - Skip break

## 🛠️ Technologies

- **Framework:** WinUI3 / Windows App SDK 1.8
- **Language:** C# 12 / .NET 9.0
- **UI Pattern:** MVVM (Model-View-ViewModel)
- **DI Container:** Microsoft.Extensions.DependencyInjection
- **MVVM Toolkit:** CommunityToolkit.Mvvm
- **Logging:** Serilog
- **Tray Icon:** H.NotifyIcon.WinUI
- **JSON:** Newtonsoft.Json
- **Task Scheduling:** TaskScheduler

## 📦 NuGet Packages

### Main Application
- Microsoft.WindowsAppSDK
- CommunityToolkit.WinUI.Controls.SettingsControls
- H.NotifyIcon.WinUI
- WinUIEx

### Core Library
- CommunityToolkit.Mvvm
- CommunityToolkit.WinUI.Extensions
- SuGarToolkit.Controls.Dialogs
- Newtonsoft.Json
- System.Runtime.Caching

### Infrastructure
- Serilog (with Console, File, Debug sinks)
- TaskScheduler

## 🤝 Contributing

Contributions are welcome! This project is in active migration from WPF to WinUI3.

### How to Contribute
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Priority Areas
- WPF to WinUI3 service adaptation
- UI component conversion
- Testing and bug fixes
- Documentation improvements

## 📜 License

This project maintains the license of the original ProjectEye application.

## 🙏 Acknowledgments

- **Original ProjectEye**: Based on the [ProjectEye](https://github.com/Jack251970/EyeProtect) WPF application
- **WinUI3 Template**: Built using [WinUI3-Template](https://github.com/Jack251970/WinUI3-Template)
- **Microsoft**: For WinUI3 and Windows App SDK
- **Community Toolkit**: For MVVM and WinUI extensions

## 📸 Screenshots

*Screenshots will be added after UI migration is complete*

## 🔗 Links

- [Original ProjectEye Repository](https://github.com/Jack251970/EyeProtect)
- [WinUI3 Template](https://github.com/Jack251970/WinUI3-Template)
- [Migration Guide](MIGRATION_GUIDE.md)
- [WinUI3 Documentation](https://docs.microsoft.com/windows/apps/winui/winui3/)

## 📧 Contact

For issues, questions, or suggestions, please open an issue on GitHub.

---

**Note:** This is an early version migrated from WPF to WinUI3. Some features may not be fully functional yet. See MIGRATION_GUIDE.md for current status and remaining work.
