# Eye Protect

An eye rest reminder software (Windows) based on the `20-20-20` rule, helping you maintain a healthy work state and track your daily eye usage.

You can set the reminder interval (default 20 minutes) and break duration (default 20 seconds). When the program starts, the timer begins. Each time the reminder interval is reached, a full-screen window will pop up to remind you to take a break. By default, you can choose to `Skip` or `Start Break`. Selecting `Skip` will close the window and restart the timer. Selecting `Start Break` will start a countdown from the set break duration (seconds). During this time, you should look away from the screen and focus on something at least 6 meters away to relax your eyes. When the countdown ends, the program will play a notification sound.

## What is the 20-20-20 Rule

Every **20** minutes, focus your attention on something at least **20** feet (**6** meters) away for **20** seconds. Following this rule can effectively relieve eye strain and protect your vision health.

[Reference: https://opto.ca/health-library/the-20-20-20-rule](https://opto.ca/health-library/the-20-20-20-rule)

## Features

- Do Not Disturb mode for full-screen status (full-screen games, full-screen videos);
- Process whitelist setting to skip reminders when running specific programs;
- Support for multiple extended monitors;
- Away detection: pauses the timer when it detects the user has left the computer until they return.

*Some features need to be manually enabled in the options to take effect.*

## Local Build

```
dotnet publish src\EyeProtect\EyeProtect.csproj -p:PublishProfile=Net10.0-Win64.pubxml
```

## Download and Install

You can download the compiled EXE files for all releases here: [Releases](https://github.com/Jack251970/EyeProtect/releases). Just double-click EyeProtect.exe to run, no installation required.

After successful launch, you will see the 😎 icon in the system tray at the bottom right corner. Right-click to show the menu.

## Runtime Environment

OS: Windows 10/11

Runtime: [.NET 10.0](https://dotnet.microsoft.com/download/dotnet/10.0)

## Other

[Help Documentation](https://github.com/Jack251970/EyeProtect/wiki)
