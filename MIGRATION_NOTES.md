# Migration to iNKORE.UI.WPF.Modern UI Framework

## Overview

This project has been migrated to use the [iNKORE.UI.WPF.Modern](https://www.nuget.org/packages/iNKORE.UI.WPF.Modern) UI framework, which provides modern Fluent 2 styles and controls for WPF applications.

## Changes Made

### 1. Added NuGet Packages

- **iNKORE.UI.WPF.Modern** (v0.10.2.1) has been added to both:
  - `src/Project1.UI/Project1.UI.csproj`
  - `src/ProjectEye/ProjectEye.csproj`

### 2. Updated Application Resources

- **src/ProjectEye/App.xaml**: Updated to include iNKORE theme resources
  - Added `xmlns:ui="http://schemas.inkore.net/lib/ui/wpf/modern"` namespace
  - Merged `ui:ThemeResources` and `ui:XamlControlsResources` into Application.Resources

- **src/Project1.UI/Themes/Generic.xaml**: Updated to include iNKORE theme resources
  - Added `xmlns:ui="http://schemas.inkore.net/lib/ui/wpf/modern"` namespace
  - Merged `ui:ThemeResources` and `ui:XamlControlsResources` at the beginning of MergedDictionaries

## Benefits

1. **Modern UI Design**: Fluent 2 design language with modern controls
2. **Theme Support**: Built-in light and dark themes with easy customization
3. **Enhanced Controls**: Additional modern controls from Windows UI Library
4. **Backward Compatibility**: Works with existing custom controls in Project1.UI
5. **Cross-Windows Support**: Runs on Windows 7 or higher (Windows 10+ recommended)

## Compatibility

- The migration maintains backward compatibility with existing custom controls
- All existing Project1.UI custom controls continue to work alongside iNKORE controls
- The iNKORE framework provides styling for standard WPF controls that will be automatically applied

## Resources

- [iNKORE.UI.WPF.Modern Documentation](https://docs.inkore.net/ui-wpf-modern/introduction)
- [GitHub Repository](https://github.com/iNKORE-NET/UI.WPF.Modern/)
- [NuGet Package](https://www.nuget.org/packages/iNKORE.UI.WPF.Modern)

## Future Improvements

Consider gradually replacing custom Project1.UI controls with iNKORE equivalents where appropriate to take full advantage of the modern UI framework's features and maintain a consistent look and feel across the application.
