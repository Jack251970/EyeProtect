using System.IO;
using Path = System.IO.Path;

namespace ProjectEye.Models.AppInfo;

internal static class ExeAppHelper
{
    internal static ExeAppInfo GetExeAppInfo(string exeFilePath)
    {
        if (!File.Exists(exeFilePath))
        {
            return null;
        }

        var exeAppInfo = new ExeAppInfo
        {
            DefaultDisplayName = Path.GetFileNameWithoutExtension(exeFilePath),
            DisplayName = Path.GetFileNameWithoutExtension(exeFilePath),
            ExeFilePath = exeFilePath,
        };
        exeAppInfo.OnDeserialized();
        return exeAppInfo;
    }
}
