using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ProjectEye.Core;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Imaging;
using Windows.Management.Deployment;
using Windows.Storage.Streams;

namespace ProjectEye.Models.AppInfo;

public sealed class UwpAppInfo : AppInfo, IEquatable<UwpAppInfo>
{
    [JsonPropertyName("app_user_model_id")]
    public string AppUserModelId { get; set; }

    [JsonPropertyName("package_family_name")]
    public string PackageFullName { get; set; }

    // Parameterless constructor for XML serialization
    public UwpAppInfo() { }

    [JsonIgnore]
    internal Package Package { get; set; }

    public override bool Equals(object obj)
    {
        return (obj is UwpAppInfo other && Equals(other)) && base.Equals(obj);
    }

    public bool Equals(UwpAppInfo other)
    {
        return base.Equals(other) && PackageFullName == other?.PackageFullName && AppUserModelId == other?.AppUserModelId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), PackageFullName, AppUserModelId);
    }

    public override void OnDeserialized()
    {
        if (Package is null)
        {
            var packageManager = new PackageManager();
            Package
                = packageManager.FindPackageForUser(
                    userSecurityId: null,
                    packageFullName: PackageFullName);
            if (Package is null)
            {
                IEnumerable<Package> packages = packageManager.FindPackagesForUser("");
                foreach (Package package in packages)
                {
                    foreach (AppListEntry appListEntry in package.GetAppListEntries())
                    {
                        if (appListEntry.AppUserModelId == AppUserModelId)
                        {
                            Package = package;
                            break;
                        }
                    }

                    if (Package is not null)
                    {
                        break;
                    }
                }
            }
        }

        if (Package is not null && string.IsNullOrEmpty(OverrideAppIconPath))
        {
            AppIcon = new TaskCompletionNotifier<ImageSource>(GetUwpAppIconAsync, runTaskImmediately: false);
        }
        else
        {
            base.OnDeserialized();
        }
    }

    public override AppInfo Clone()
    {
        var newAppInfo = new UwpAppInfo
        {
            DefaultDisplayName = this.DefaultDisplayName,
            DisplayName = this.DisplayName,
            OverrideAppIconPath = this.OverrideAppIconPath,
            AppUserModelId = this.AppUserModelId,
            PackageFullName = this.PackageFullName,
            Package = this.Package,
        };
        newAppInfo.OnDeserialized();
        return newAppInfo;
    }

    /// <summary>
    /// Retrieve the icon of a UWP app
    /// </summary>
    /// <param name="package">The UWP package</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task<ImageSource> GetUwpAppIconAsync()
    {
        return null;/*await Application.Current.Dispatcher.InvokeAsync(
            async () =>
            {
                try
                {
                    RandomAccessStreamReference appIcon
                        = Package.GetLogoAsRandomAccessStreamReference(
                            new Windows.Foundation.Size(
                                IconHelper.DefaultIconSize * 2,
                                IconHelper.DefaultIconSize * 2));

                    using IRandomAccessStreamWithContentType appIconStream = await appIcon.OpenReadAsync();

                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(appIconStream);

                    using SoftwareBitmap softwareBitmap
                        = await decoder.GetSoftwareBitmapAsync(
                            BitmapPixelFormat.Bgra8,
                            BitmapAlphaMode.Premultiplied);

                    // Create WriteableBitmap instead of BitmapImage
                    var writeableBitmap = new Microsoft.UI.Xaml.Media.Imaging.WriteableBitmap(
                        softwareBitmap.PixelWidth,
                        softwareBitmap.PixelHeight);

                    // Copy pixel data from SoftwareBitmap to WriteableBitmap
                    softwareBitmap.CopyToBuffer(writeableBitmap.PixelBuffer);

                    return writeableBitmap;
                }
                catch (Exception)
                {
                    // Failed to extract UWP app icon for: {Path}", PackageFullName
                    return null;
                }
            });*/
    }
}
