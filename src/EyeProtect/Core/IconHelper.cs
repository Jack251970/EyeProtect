using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EyeProtect.Models.AppInfo;

namespace EyeProtect.Core;

internal static class IconHelper
{
    internal const uint DefaultIconSize = 64;

    internal static async Task<ImageSource> GetIconFromFileOrFolderAsync(string filePath, uint size = DefaultIconSize)
    {
        return await Task.Run(() =>
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    return null;
                }

                // Use WindowsThumbnailProvider for better icon extraction
                BitmapSource thumbnail = WindowsThumbnailProvider.GetThumbnail(
                    filePath,
                    (int)size,
                    (int)size,
                    ThumbnailOptions.IconOnly | ThumbnailOptions.BiggerSizeOk);

                // Freeze for cross-thread access
                if (thumbnail != null && thumbnail.CanFreeze)
                {
                    thumbnail.Freeze();
                }

                return thumbnail;
            }
            catch (Exception ex)
            {
                LogHelper.Debug($"Error loading icon from {filePath}: {ex.Message}");
                return null;
            }
        });
    }

    public static async Task<ImageSource> CreateGridIconAsync(
        IReadOnlyList<AppInfo> selectedItems,
        int selectedSize)
    {
        // Grid icon creation not implemented yet
        return null;
    }
}
