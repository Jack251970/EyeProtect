using System;
using System.Linq;
using Microsoft.Win32;

namespace ProjectEye.Core;

/// <summary>
/// Helper for startup register and unregister.
/// For MSIX package, you need to add extension: uap5:StartupTask.
/// Codes are edited from: <see href="https://github.com/microsoft/terminal"> and <see href="https://github.com/seerge/g-helper">.
/// </summary>
public class StartupHelper
{
    public const string NonMsixStartupTag = "/startup";

    private const string NonMsixRegistryKey = "ProjectEye";

    private const string RegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    private const string ApprovalPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";

    private static readonly byte[] ApprovalValue1 = [0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
    private static readonly byte[] ApprovalValue2 = [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];

    /// <summary>
    /// Set application startup or not.
    /// </summary>
    public static bool SetStartup(bool startup, bool currentUser = true)
    {
        var state = GetStartup(currentUser);
        if (!state && startup)
        {
            return SetStartupRegistryKey(startup, currentUser);
        }
        else if (state && !startup)
        {
            return SetStartupRegistryKey(startup, currentUser);
        }
        return true;
    }

    /// <summary>
    /// Get application startup or not by checking register keys.
    /// </summary>
    public static bool GetStartup(bool currentUser = true)
    {
        return CheckAndGetStartupRegistryKey(currentUser);
    }

    /// <summary>
    /// Check and fix the startup.
    /// </summary>
    public static bool CheckStartup(bool currentUser = true)
    {
        return CheckAndGetStartupRegistryKey(currentUser);
    }

    /// <summary>
    /// Check and get the startup register key.
    /// </summary>
    private static bool CheckAndGetStartupRegistryKey(bool currentUser = true)
    {
        if (Environment.ProcessPath is not string appPath)
        {
            return false;
        }

        var root = currentUser ? Registry.CurrentUser : Registry.LocalMachine;
        try
        {
            var startup = false;
            var path = root.OpenSubKey(RegistryPath, true);
            if (path == null)
            {
                using var key2 = root.CreateSubKey("SOFTWARE");
                using var key3 = key2.CreateSubKey("Microsoft");
                using var key4 = key3.CreateSubKey("Windows");
                using var key5 = key4.CreateSubKey("CurrentVersion");
                using var key6 = key5.CreateSubKey("Run");
                path = key6;
            }
            var keyNames = path.GetValueNames();
            // check if the startup register key exists
            foreach (var keyName in keyNames)
            {
                if (keyName.Equals(NonMsixRegistryKey, StringComparison.CurrentCultureIgnoreCase))
                {
                    startup = true;
                    // check if the startup register value is valid and fix it
                    if (startup)
                    {
                        var value = path.GetValue(keyName)!.ToString()!;
                        if (!value.Contains(@appPath, StringComparison.CurrentCultureIgnoreCase))
                        {
                            path.SetValue(NonMsixRegistryKey, $@"""{@appPath}"" {NonMsixStartupTag}");
                            path.Close();
                            path = root.OpenSubKey(ApprovalPath, true);
                            if (path != null)
                            {
                                path.SetValue(NonMsixRegistryKey, ApprovalValue1);
                                path.Close();
                            }
                        }
                    }
                    break;
                }
            }
            // check if the startup register key is approved
            if (startup)
            {
                path?.Close();
                path = root.OpenSubKey(ApprovalPath, false);
                if (path != null)
                {
                    keyNames = path.GetValueNames();
                    foreach (var keyName in keyNames)
                    {
                        if (keyName.Equals(NonMsixRegistryKey, StringComparison.CurrentCultureIgnoreCase))
                        {
                            var value = (byte[])path.GetValue(keyName)!;
                            if (!(value.SequenceEqual(ApprovalValue1) || value.SequenceEqual(ApprovalValue2)))
                            {
                                startup = false;
                            }
                            break;
                        }
                    }
                }
            }
            path?.Close();
            return startup;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Add or delete the startup register key.
    /// </summary>
    private static bool SetStartupRegistryKey(bool startup, bool currentUser = true)
    {
        if (Environment.ProcessPath is not string appPath)
        {
            return false;
        }

        var root = currentUser ? Registry.CurrentUser : Registry.LocalMachine;
        var value = $@"""{@appPath}"" {NonMsixStartupTag}";
        try
        {
            var path = root.OpenSubKey(RegistryPath, true);
            if (path == null)
            {
                var key2 = root.CreateSubKey("SOFTWARE");
                var key3 = key2.CreateSubKey("Microsoft");
                var key4 = key3.CreateSubKey("Windows");
                var key5 = key4.CreateSubKey("CurrentVersion");
                var key6 = key5.CreateSubKey("Run");
                path = key6;
            }
            // add the startup register key
            if (startup)
            {
                path.SetValue(NonMsixRegistryKey, value);
                path.Close();
                // set the startup approval key to approval status
                path = root.OpenSubKey(ApprovalPath, true);
                if (path != null)
                {
                    path.SetValue(NonMsixRegistryKey, ApprovalValue1);
                    path.Close();
                }
            }
            else
            // delete the startup register key
            {
                var keyNames = path.GetValueNames();
                foreach (var keyName in keyNames)
                {
                    if (keyName.Equals(NonMsixRegistryKey, StringComparison.CurrentCultureIgnoreCase))
                    {
                        path.DeleteValue(NonMsixRegistryKey);
                        path.Close();
                        break;
                    }
                }
                // delete the startup approval key
                path = root.OpenSubKey(ApprovalPath, true);
                if (path != null)
                {
                    path.DeleteValue(NonMsixRegistryKey);
                    path.Close();
                }
            }
            path?.Close();
        }
        catch
        {
            return false;
        }
        return true;
    }
}
