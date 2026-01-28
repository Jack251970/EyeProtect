using System;
using System.Collections.Generic;
using ProjectEye.Models;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace ProjectEye.Core
{
    public class Win32APIHelper
    {
        /// <summary>
        /// 窗口信息结构
        /// </summary>
        public struct WindowInfo
        {
            /// <summary>
            /// 窗口宽度
            /// </summary>
            public int Width;
            /// <summary>
            /// 窗口高度
            /// </summary>
            public int Height;
            /// <summary>
            /// 窗口标题
            /// </summary>
            public string Title;
            /// <summary>
            /// 窗口类名
            /// </summary>
            public string ClassName;
            /// <summary>
            /// 是否全屏
            /// </summary>
            public bool IsFullScreen;
            /// <summary>
            /// 是否最大化
            /// </summary>
            public bool IsZoomed;
            /// <summary>
            /// 窗口所属进程ID
            /// </summary>
            public uint ProcessId;
        }

        /// <summary>
        /// 获取当前焦点窗口信息
        /// </summary>
        /// <returns></returns>
        public static unsafe WindowInfo GetFocusWindowInfo()
        {
            var result = new WindowInfo();
            //获取当前焦点窗口句柄
            var hwnd = PInvoke.GetForegroundWindow();
            //获取窗口大小
            PInvoke.GetWindowRect(hwnd, out var rect);
            result.IsZoomed = PInvoke.IsZoomed(hwnd);
            result.Width = rect.Width;
            result.Height = rect.Height;
            //获取窗口标题
            result.Title = GetWindowTitle(hwnd);
            //获取窗口类名
            result.ClassName = GetClassName(hwnd);
            //判断全屏
            result.IsFullScreen = IsWindowFullscreen(hwnd);
            //获取进程ID
            uint processId = 0;
            PInvoke.GetWindowThreadProcessId(hwnd, &processId);
            result.ProcessId = processId;
            return result;
        }

        /// <summary>
        /// 获取所有顶层可见窗口信息
        /// </summary>
        /// <returns></returns>
        public static List<WindowInfo> GetTopVisibleWindowsInfo()
        {
            var windows = new List<WindowInfo>();
            var visibleWindows = new List<HWND>();

            // 枚举所有顶层窗口
            unsafe
            {
                PInvoke.EnumWindows((hwnd, lParam) =>
                {
                    // 只处理可见的窗口
                    if (PInvoke.IsWindowVisible(hwnd))
                    {
                        visibleWindows.Add(hwnd);
                    }
                    return true;
                }, 0);
            }

            // 对于每个可见窗口，检查它是否应该被包含
            foreach (var hwnd in visibleWindows)
            {
                if (IsValidTopWindow(hwnd))
                {
                    var info = GetWindowInfoFromHandle(hwnd);
                    windows.Add(info);
                }
            }

            return windows;
        }

        /// <summary>
        /// 从窗口句柄获取窗口信息
        /// </summary>
        private static unsafe WindowInfo GetWindowInfoFromHandle(HWND hwnd)
        {
            var result = new WindowInfo();
            PInvoke.GetWindowRect(hwnd, out var rect);
            result.IsZoomed = PInvoke.IsZoomed(hwnd);
            result.Width = rect.Width;
            result.Height = rect.Height;
            result.Title = GetWindowTitle(hwnd);
            result.ClassName = GetClassName(hwnd);
            result.IsFullScreen = IsWindowFullscreen(hwnd);
            
            // Get process ID
            uint processId = 0;
            PInvoke.GetWindowThreadProcessId(hwnd, &processId);
            result.ProcessId = processId;
            
            return result;
        }

        /// <summary>
        /// 检查窗口是否为有效的顶层窗口
        /// Check if window is a valid top window
        /// </summary>
        private static unsafe bool IsValidTopWindow(HWND hwnd)
        {
            // 跳过桌面和Shell窗口
            // Skip desktop and shell windows
            if (hwnd.Equals(HWND_DESKTOP) || hwnd.Equals(HWND_SHELL))
            {
                return false;
            }

            // 获取窗口类名
            // Get window class name
            string windowClass = GetClassName(hwnd);

            // 跳过某些特殊窗口类
            // Skip certain special window classes
            if (windowClass is WINDOW_CLASS_PROGMAN or WINDOW_CLASS_WORKERW or WINDOW_CLASS_WINTAB)
            {
                return false;
            }

            // 跳过UWP应用内部窗口（但保留ApplicationFrameWindow容器）
            // Skip UWP app internal windows (but keep ApplicationFrameWindow container)
            if (windowClass is "Windows.UI.Core.CoreWindow")
            {
                return false;
            }

            // 跳过特定的已知覆盖层和系统窗口
            // Skip specific known overlay and system windows
            if (windowClass is "CEF-OSC-WIDGET" or "Windows Input Experience")
            {
                return false;
            }

            // 跳过HwndWrapper窗口（通常是WPF托管的背景窗口）
            // Skip HwndWrapper windows (usually WPF hosted background windows)
            if (windowClass.StartsWith("HwndWrapper", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // 检查窗口是否被DWM隐藏（Windows 10+的窗口隐藏功能）
            // Check if window is cloaked by DWM (Windows 10+ window hiding feature)
            int cloaked = 0;
            var result = PInvoke.DwmGetWindowAttribute(hwnd, Windows.Win32.Graphics.Dwm.DWMWINDOWATTRIBUTE.DWMWA_CLOAKED, &cloaked, sizeof(int));
            if (result == 0 && cloaked != 0)
            {
                return false;
            }

            // 检查窗口扩展样式
            // Check window extended styles
            var exStyle = (WINDOW_EX_STYLE)PInvoke.GetWindowLongPtr(hwnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
            
            // 跳过工具窗口
            // Skip tool windows
            if ((exStyle & WINDOW_EX_STYLE.WS_EX_TOOLWINDOW) != 0)
            {
                return false;
            }

            // 跳过无应用窗口样式的窗口（这些通常不显示在任务栏）
            // Skip windows without app window style (these usually don't show in taskbar)
            if ((exStyle & WINDOW_EX_STYLE.WS_EX_APPWINDOW) == 0)
            {
                // 检查窗口是否有所有者，如果有则通常不是顶层应用窗口
                // Check if window has an owner, if so it's usually not a top-level app window
                var owner = PInvoke.GetWindow(hwnd, GET_WINDOW_CMD.GW_OWNER);
                if (owner != HWND.Null)
                {
                    return false;
                }
            }

            // 获取窗口矩形
            // Get window rectangle
            PInvoke.GetWindowRect(hwnd, out var rect);

            // 跳过太小的窗口（例如任务栏图标等）
            // Skip windows that are too small (e.g., taskbar icons)
            if (rect.Width < MIN_WINDOW_WIDTH || rect.Height < MIN_WINDOW_HEIGHT)
            {
                return false;
            }

            return true;
        }

        private static unsafe string GetWindowTitle(HWND hwnd)
        {
            var capacity = PInvoke.GetWindowTextLength(hwnd) + 1;
            int length;
            Span<char> buffer = capacity < 1024 ? stackalloc char[capacity] : new char[capacity];
            fixed (char* pBuffer = buffer)
            {
                // If the window has no title bar or text, if the title bar is empty,
                // or if the window or control handle is invalid, the return value is zero.
                length = PInvoke.GetWindowText(hwnd, pBuffer, capacity);
            }

            return buffer[..length].ToString();
        }

        private static unsafe string GetClassName(HWND hwnd)
        {
            fixed (char* buf = new char[256])
            {
                return PInvoke.GetClassName(hwnd, buf, 256) switch
                {
                    0 => string.Empty,
                    _ => new string(buf),
                };
            }
        }

        private const string WINDOW_CLASS_CONSOLE = "ConsoleWindowClass";
        private const string WINDOW_CLASS_WINTAB = "Flip3D";
        private const string WINDOW_CLASS_PROGMAN = "Progman";
        private const string WINDOW_CLASS_WORKERW = "WorkerW";
        private const int MIN_WINDOW_WIDTH = 100;
        private const int MIN_WINDOW_HEIGHT = 100;

        private static HWND _hwnd_shell;
        private static HWND HWND_SHELL =>
            _hwnd_shell != HWND.Null ? _hwnd_shell : _hwnd_shell = PInvoke.GetShellWindow();

        private static HWND _hwnd_desktop;
        private static HWND HWND_DESKTOP =>
            _hwnd_desktop != HWND.Null ? _hwnd_desktop : _hwnd_desktop = PInvoke.GetDesktopWindow();

        private static unsafe bool IsWindowFullscreen(HWND hWnd)
        {
            // If current active window is desktop or shell, exit early
            if (hWnd.Equals(HWND_DESKTOP) || hWnd.Equals(HWND_SHELL))
            {
                return false;
            }

            string windowClass;
            const int capacity = 256;
            Span<char> buffer = stackalloc char[capacity];
            int validLength;
            fixed (char* pBuffer = buffer)
            {
                validLength = PInvoke.GetClassName(hWnd, pBuffer, capacity);
            }

            windowClass = buffer[..validLength].ToString();

            // For Win+Tab (Flip3D)
            if (windowClass == WINDOW_CLASS_WINTAB)
            {
                return false;
            }

            PInvoke.GetWindowRect(hWnd, out var appBounds);

            // For console (ConsoleWindowClass), we have to check for negative dimensions
            if (windowClass == WINDOW_CLASS_CONSOLE)
            {
                return appBounds.top < 0 && appBounds.bottom < 0;
            }

            // For desktop (Progman or WorkerW, depends on the system), we have to check
            if (windowClass is WINDOW_CLASS_PROGMAN or WINDOW_CLASS_WORKERW)
            {
                var hWndDesktop = PInvoke.FindWindowEx(hWnd, HWND.Null, "SHELLDLL_DefView", null);
                hWndDesktop = PInvoke.FindWindowEx(hWndDesktop, HWND.Null, "SysListView32", "FolderView");
                if (hWndDesktop != HWND.Null)
                {
                    return false;
                }
            }

            var monitorInfo = MonitorInfo.GetNearestDisplayMonitor(hWnd);
            return (appBounds.bottom - appBounds.top) == monitorInfo.Bounds.Height &&
                   (appBounds.right - appBounds.left) == monitorInfo.Bounds.Width;
        }
    }
}
