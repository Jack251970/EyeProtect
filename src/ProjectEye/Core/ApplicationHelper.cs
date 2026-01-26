using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace ProjectEye.Core
{
    /// <summary>
    /// Helper class for working with installed applications
    /// </summary>
    public class ApplicationHelper
    {
        /// <summary>
        /// Gets a list of installed applications with their process names
        /// </summary>
        /// <returns>List of ApplicationInfo objects</returns>
        public static List<ApplicationInfo> GetInstalledApplications()
        {
            var apps = new HashSet<ApplicationInfo>(new ApplicationInfoComparer());

            // Get Win32 applications from registry
            GetWin32Applications(apps);

            // Get currently running processes
            GetRunningProcesses(apps);

            return apps.OrderBy(a => a.DisplayName).ToList();
        }

        private static void GetWin32Applications(HashSet<ApplicationInfo> apps)
        {
            // Check both 32-bit and 64-bit registry locations
            var uninstallKeys = new[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
            };

            foreach (var keyPath in uninstallKeys)
            {
                try
                {
                    using (var key = Registry.LocalMachine.OpenSubKey(keyPath))
                    {
                        if (key != null)
                        {
                            foreach (var subKeyName in key.GetSubKeyNames())
                            {
                                try
                                {
                                    using (var subKey = key.OpenSubKey(subKeyName))
                                    {
                                        if (subKey != null)
                                        {
                                            var displayName = subKey.GetValue("DisplayName")?.ToString();
                                            var displayIcon = subKey.GetValue("DisplayIcon")?.ToString();
                                            
                                            if (!string.IsNullOrWhiteSpace(displayName))
                                            {
                                                // Try to get process name from install location or display icon
                                                var processName = GetProcessNameFromRegistry(subKey, displayIcon);
                                                
                                                if (!string.IsNullOrWhiteSpace(processName))
                                                {
                                                    apps.Add(new ApplicationInfo
                                                    {
                                                        DisplayName = displayName,
                                                        ProcessName = processName,
                                                        IconPath = displayIcon
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                    // Skip apps that can't be read
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // Skip if registry key can't be accessed
                }
            }

            // Also check current user registry
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
                {
                    if (key != null)
                    {
                        foreach (var subKeyName in key.GetSubKeyNames())
                        {
                            try
                            {
                                using (var subKey = key.OpenSubKey(subKeyName))
                                {
                                    if (subKey != null)
                                    {
                                        var displayName = subKey.GetValue("DisplayName")?.ToString();
                                        var displayIcon = subKey.GetValue("DisplayIcon")?.ToString();
                                        
                                        if (!string.IsNullOrWhiteSpace(displayName))
                                        {
                                            var processName = GetProcessNameFromRegistry(subKey, displayIcon);
                                            
                                            if (!string.IsNullOrWhiteSpace(processName))
                                            {
                                                apps.Add(new ApplicationInfo
                                                {
                                                    DisplayName = displayName,
                                                    ProcessName = processName,
                                                    IconPath = displayIcon
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                // Skip apps that can't be read
                            }
                        }
                    }
                }
            }
            catch
            {
                // Skip if registry key can't be accessed
            }
        }

        private static string GetProcessNameFromRegistry(RegistryKey subKey, string displayIcon)
        {
            // Try to get from DisplayIcon first
            if (!string.IsNullOrWhiteSpace(displayIcon))
            {
                var exePath = ExtractExePath(displayIcon);
                if (!string.IsNullOrWhiteSpace(exePath))
                {
                    return Path.GetFileNameWithoutExtension(exePath);
                }
            }

            // Try InstallLocation
            var installLocation = subKey.GetValue("InstallLocation")?.ToString();
            if (!string.IsNullOrWhiteSpace(installLocation))
            {
                var exeFiles = Directory.Exists(installLocation) 
                    ? Directory.GetFiles(installLocation, "*.exe", SearchOption.TopDirectoryOnly)
                    : Array.Empty<string>();
                
                if (exeFiles.Length > 0)
                {
                    return Path.GetFileNameWithoutExtension(exeFiles[0]);
                }
            }

            return null;
        }

        private static string ExtractExePath(string displayIcon)
        {
            if (string.IsNullOrWhiteSpace(displayIcon))
                return null;

            // Remove quotes
            displayIcon = displayIcon.Trim('"');

            // Remove icon index (e.g., ",0")
            var commaIndex = displayIcon.LastIndexOf(',');
            if (commaIndex > 0)
            {
                displayIcon = displayIcon.Substring(0, commaIndex);
            }

            // Check if file exists
            if (File.Exists(displayIcon) && displayIcon.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                return displayIcon;
            }

            return null;
        }

        private static void GetRunningProcesses(HashSet<ApplicationInfo> apps)
        {
            try
            {
                var processes = Process.GetProcesses();
                foreach (var process in processes)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(process.MainWindowTitle) && 
                            process.MainWindowHandle != IntPtr.Zero)
                        {
                            var displayName = process.MainWindowTitle;
                            var processName = process.ProcessName;

                            // Try to get file description as a better name
                            if (!string.IsNullOrWhiteSpace(process.MainModule?.FileName))
                            {
                                var versionInfo = FileVersionInfo.GetVersionInfo(process.MainModule.FileName);
                                if (!string.IsNullOrWhiteSpace(versionInfo.FileDescription))
                                {
                                    displayName = versionInfo.FileDescription;
                                }
                            }

                            apps.Add(new ApplicationInfo
                            {
                                DisplayName = displayName,
                                ProcessName = processName,
                                IconPath = process.MainModule?.FileName
                            });
                        }
                    }
                    catch
                    {
                        // Skip processes that can't be accessed
                    }
                }
            }
            catch
            {
                // Skip if processes can't be enumerated
            }
        }
    }

    /// <summary>
    /// Information about an installed application
    /// </summary>
    public class ApplicationInfo
    {
        public string DisplayName { get; set; }
        public string ProcessName { get; set; }
        public string IconPath { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    /// <summary>
    /// Comparer for ApplicationInfo to avoid duplicates
    /// </summary>
    public class ApplicationInfoComparer : IEqualityComparer<ApplicationInfo>
    {
        public bool Equals(ApplicationInfo x, ApplicationInfo y)
        {
            if (x == null || y == null)
                return false;

            return string.Equals(x.ProcessName, y.ProcessName, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(ApplicationInfo obj)
        {
            if (obj?.ProcessName == null)
                return 0;

            return obj.ProcessName.ToLowerInvariant().GetHashCode();
        }
    }
}
