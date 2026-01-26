using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ProjectEye.Models.AppInfo;

namespace ProjectEye.Core;

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

                // Extract icon from executable
                Icon icon = Icon.ExtractAssociatedIcon(filePath);
                if (icon == null)
                {
                    return null;
                }

                // Convert Icon to BitmapSource
                using (icon)
                {
                    Bitmap bitmap = icon.ToBitmap();
                    IntPtr hBitmap = bitmap.GetHbitmap();

                    try
                    {
                        ImageSource imageSource = Imaging.CreateBitmapSourceFromHBitmap(
                            hBitmap,
                            IntPtr.Zero,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());

                        // Freeze for cross-thread access
                        if (imageSource.CanFreeze)
                        {
                            imageSource.Freeze();
                        }

                        return imageSource;
                    }
                    finally
                    {
                        DeleteObject(hBitmap);
                        bitmap.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading icon from {filePath}: {ex.Message}");
                return null;
            }
        });
    }

    [DllImport("gdi32.dll", SetLastError = true)]
    private static extern bool DeleteObject(IntPtr hObject);

    public static async Task<ImageSource> CreateGridIconAsync(
        IReadOnlyList<AppInfo> selectedItems,
        int selectedSize)
    {
        // Grid icon creation not implemented yet
        return null;
    }
}
