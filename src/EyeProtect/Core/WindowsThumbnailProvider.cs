using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.Shell;

namespace EyeProtect.Core;

/// <summary>
/// Thumbnail options for controlling extraction behavior
/// </summary>
[Flags]
public enum ThumbnailOptions
{
    None = 0x00,
    BiggerSizeOk = 0x01,
    InMemoryOnly = 0x02,
    IconOnly = 0x04,
    ThumbnailOnly = 0x08,
    InCacheOnly = 0x10,
}

/// <summary>
/// Provider for extracting thumbnails and icons from files using Windows Shell APIs
/// </summary>
public class WindowsThumbnailProvider
{
    // Based on https://stackoverflow.com/questions/21751747/extract-thumbnail-for-any-file-in-windows

    private static readonly Guid GUID_IShellItem = typeof(IShellItem).GUID;

    private static readonly HRESULT S_EXTRACTIONFAILED = (HRESULT)0x8004B200;

    private static readonly HRESULT S_PATHNOTFOUND = (HRESULT)0x8004B205;

    private const string UrlExtension = ".url";

    /// <summary>
    /// Gets a thumbnail for the specified file
    /// </summary>
    /// <param name="fileName">Path to the file (can be a regular file or a ".url" shortcut).</param>
    /// <param name="width">Requested thumbnail width in pixels.</param>
    /// <param name="height">Requested thumbnail height in pixels.</param>
    /// <param name="options">Thumbnail extraction options (flags) controlling fallback and caching behavior.</param>
    /// <returns>BitmapSource containing the thumbnail</returns>
    public static BitmapSource GetThumbnail(string fileName, int width, int height, ThumbnailOptions options)
    {
        HBITMAP hBitmap;

        var extension = Path.GetExtension(fileName);
        if (string.Equals(extension, UrlExtension, StringComparison.OrdinalIgnoreCase))
        {
            hBitmap = GetHBitmapForUrlFile(fileName, width, height, options);
        }
        else
        {
            hBitmap = GetHBitmap(Path.GetFullPath(fileName), width, height, options);
        }

        try
        {
            return Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
        finally
        {
            // delete HBitmap to avoid memory leaks
            PInvoke.DeleteObject(hBitmap);
        }
    }

    /// <summary>
    /// Gets an HBITMAP for a file
    /// </summary>
    /// <param name="fileName">Path to the file to thumbnail.</param>
    /// <param name="width">Requested thumbnail width in pixels.</param>
    /// <param name="height">Requested thumbnail height in pixels.</param>
    /// <param name="options">Thumbnail request flags that control behavior (e.g., ThumbnailOnly, IconOnly).</param>
    /// <returns>HBITMAP handle</returns>
    /// <exception cref="InvalidOperationException">Thrown when thumbnail extraction fails</exception>
    private static unsafe HBITMAP GetHBitmap(string fileName, int width, int height, ThumbnailOptions options)
    {
        var retCode = PInvoke.SHCreateItemFromParsingName(
            fileName,
            null,
            GUID_IShellItem,
            out var nativeShellItem);

        if (retCode.Failed)
            throw Marshal.GetExceptionForHR(retCode);

        if (nativeShellItem is not IShellItemImageFactory imageFactory)
        {
            Marshal.ReleaseComObject(nativeShellItem);
            nativeShellItem = null;
            throw new InvalidOperationException("Failed to get IShellItemImageFactory");
        }

        SIZE size = new SIZE
        {
            cx = width,
            cy = height
        };

        HBITMAP hBitmap = default;
        try
        {
            try
            {
                imageFactory.GetImage(size, (SIIGBF)options, &hBitmap);
            }
            catch (COMException ex) when (options == ThumbnailOptions.ThumbnailOnly &&
                (ex.HResult == S_EXTRACTIONFAILED || ex.HResult == S_PATHNOTFOUND))
            {
                // Fallback to IconOnly if extraction fails or files cannot be found
                imageFactory.GetImage(size, (SIIGBF)ThumbnailOptions.IconOnly, &hBitmap);
            }
            catch (FileNotFoundException) when (options == ThumbnailOptions.ThumbnailOnly)
            {
                // Fallback to IconOnly if files cannot be found
                imageFactory.GetImage(size, (SIIGBF)ThumbnailOptions.IconOnly, &hBitmap);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                throw new InvalidOperationException("Failed to get thumbnail", ex);
            }
        }
        finally
        {
            if (nativeShellItem != null)
            {
                Marshal.ReleaseComObject(nativeShellItem);
            }
        }

        return hBitmap;
    }

    /// <summary>
    /// Gets an HBITMAP for a .url file
    /// </summary>
    /// <param name="fileName">Path to the .url shortcut file.</param>
    /// <param name="width">Requested thumbnail width (pixels).</param>
    /// <param name="height">Requested thumbnail height (pixels).</param>
    /// <param name="options">ThumbnailOptions flags controlling extraction behavior.</param>
    /// <returns>HBITMAP handle</returns>
    private static unsafe HBITMAP GetHBitmapForUrlFile(string fileName, int width, int height, ThumbnailOptions options)
    {
        HBITMAP hBitmap;

        try
        {
            // Try to read the .url file to get the icon path
            var lines = File.ReadAllLines(fileName);
            string iconPath = null;

            foreach (var line in lines)
            {
                if (line.StartsWith("IconFile=", StringComparison.OrdinalIgnoreCase))
                {
                    iconPath = line["IconFile=".Length..].Trim();
                    break;
                }
            }

            if (!string.IsNullOrEmpty(iconPath) && File.Exists(iconPath))
            {
                hBitmap = GetHBitmap(Path.GetFullPath(iconPath), width, height, options);
            }
            else
            {
                // If the IconFile is missing, fallback to the default icon
                throw new FileNotFoundException("Icon file not specified in Internet shortcut (.url) file.");
            }
        }
        catch
        {
            // Fallback to extracting icon from the .url file itself
            hBitmap = GetHBitmap(Path.GetFullPath(fileName), width, height, options);
        }

        return hBitmap;
    }
}
