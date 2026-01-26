using System;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Media;
using ProjectEye.Core;

namespace ProjectEye.Models.AppInfo;

[JsonDerivedType(typeof(ExeAppInfo), typeDiscriminator: "exe")]
[JsonDerivedType(typeof(ShortcutAppInfo), typeDiscriminator: "shortcut")]
[JsonDerivedType(typeof(UwpAppInfo), typeDiscriminator: "uwp")]
[DebuggerDisplay("{DisplayName}")]
public abstract partial class AppInfo : IJsonOnDeserialized, IEquatable<AppInfo>
{
    [JsonPropertyName("default_display_name")]
    public required string DefaultDisplayName { get; init; }

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; }

    [JsonPropertyName("override_app_icon_path")]
    public string OverrideAppIconPath { get; set; }

    [JsonIgnore]
    internal TaskCompletionNotifier<ImageSource> AppIcon { get; set; }
        = new(() => Task.FromResult<ImageSource>(null), runTaskImmediately: false);

    public virtual void OnDeserialized()
    {
        AppIcon = new TaskCompletionNotifier<ImageSource>(() => IconHelper.GetIconFromFileOrFolderAsync(OverrideAppIconPath), runTaskImmediately: false);
    }

    public abstract AppInfo Clone();

    public override bool Equals(object obj)
    {
        return obj is AppInfo other && Equals(other);
    }

    public bool Equals(AppInfo other)
    {
        return DefaultDisplayName == other?.DefaultDisplayName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(DefaultDisplayName);
    }
}
