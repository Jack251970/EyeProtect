using System;
using System.IO;
using System.Text.Json.Serialization;
using System.Windows.Media;
using ProjectEye.Core;

namespace ProjectEye.Models.AppInfo;

public sealed partial class ExeAppInfo : AppInfo, IJsonOnDeserialized, IEquatable<ExeAppInfo>
{
    [JsonPropertyName("exe_file_path")]
    public string ExeFilePath { get; set; }

    // Parameterless constructor for XML serialization
    public ExeAppInfo() { }

    public override bool Equals(object obj)
    {
        return (obj is ExeAppInfo other && Equals(other)) && base.Equals(obj);
    }

    public bool Equals(ExeAppInfo other)
    {
        return base.Equals(other) && ExeFilePath == other?.ExeFilePath;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), ExeFilePath);
    }

    public override void OnDeserialized()
    {
        if (string.IsNullOrEmpty(OverrideAppIconPath) && !string.IsNullOrWhiteSpace(ExeFilePath) && File.Exists(ExeFilePath))
        {
            AppIcon = new TaskCompletionNotifier<ImageSource>(() => IconHelper.GetIconFromFileOrFolderAsync(ExeFilePath), runTaskImmediately: false);
        }
        else
        {
            base.OnDeserialized();
        }
    }

    public override AppInfo Clone()
    {
        var newAppInfo = new ExeAppInfo
        {
            DefaultDisplayName = this.DefaultDisplayName,
            DisplayName = this.DisplayName,
            OverrideAppIconPath = this.OverrideAppIconPath,
            ExeFilePath = this.ExeFilePath,
        };
        newAppInfo.OnDeserialized();
        return newAppInfo;
    }
}
