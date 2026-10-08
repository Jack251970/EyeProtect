# Eye Protect

Eye Protect is a Windows break reminder based on the **20-20-20 rule**. It runs from the system tray, reminds you to look away from your screen, and can track your daily work and break time.

## Get started

1. Download the latest release from [GitHub Releases](https://github.com/Jack251970/EyeProtect/releases).
2. Run `EyeProtect.exe`. No installer is required. The app appears as a sunglasses icon in the system tray; right-click it to open the menu.
3. Open **Settings** to adjust the reminder interval and break duration. The defaults are **20 minutes** and **20 seconds**.

Eye Protect supports **Windows 10 and 11 (x64)** and requires the [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0).

## How it works

The timer starts when Eye Protect launches. When the reminder interval ends, a full-screen prompt appears on each connected monitor. You can **Break** to start the countdown or **Skip** to dismiss the prompt and restart the timer. At the end of a break, Eye Protect plays a sound if the break-end sound is enabled.

The 20-20-20 rule suggests looking at something at least **20 feet (about 6 meters)** away for **20 seconds** every **20 minutes**. See the [Optometrists' 20-20-20 guide](https://opto.ca/health-library/the-20-20-20-rule).

## Features

- Adjustable reminder interval and break duration, with optional daily statistics.
- Do-not-disturb options for full-screen apps and selected applications.
- Away detection that pauses reminders while you are away from the computer.
- Optional automatic media pause during breaks and optional face detection.
- Tray controls for viewing the next break, starting a break, or suspending reminders.
- English, Simplified Chinese, and Traditional Chinese interfaces.

Some features are off by default; enable them in **Settings** if needed.

## Screenshots

![Eye Protect screenshot 1](images/screenshot%201.png)
![Eye Protect screenshot 2](images/screenshot%202.png)
![Eye Protect screenshot 3](images/screenshot%203.png)

## Build from source

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) and run this command from the repository root on Windows:

```powershell
dotnet publish src/EyeProtect/EyeProtect.csproj -p:PublishProfile=Net10.0-Win64.pubxml
```

The publish profile creates a framework-dependent, single-file Windows x64 build in `src/EyeProtect/bin/Publish/`. The .NET 10 Desktop Runtime is still required on the target computer.

## Help and support

- [Help documentation](https://github.com/Jack251970/EyeProtect/wiki)
- [Report an issue](https://github.com/Jack251970/EyeProtect/issues)
- [Support the project on Ko-fi](https://ko-fi.com/jackye)
