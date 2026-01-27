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
        }

        /// <summary>
        /// 获取当前焦点窗口信息
        /// </summary>
        /// <returns></returns>
        public static WindowInfo GetFocusWindowInfo()
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
            return result;
        }

        /// <summary>
        /// 获取所有顶层可见窗口信息（未被其他窗口覆盖）
        /// Get all top-level visible window information (not covered by other windows)
        /// </summary>
        /// <returns></returns>
        public static List<WindowInfo> GetTopVisibleWindowsInfo()
        {
            var windows = new List<WindowInfo>();
            var visibleWindows = new List<HWND>();

            // 枚举所有顶层窗口
            // Enumerate all top-level windows
            unsafe
            {
                PInvoke.EnumWindows((hwnd, lParam) =>
                {
                    // 只处理可见的窗口
                    // Only process visible windows
                    if (PInvoke.IsWindowVisible(hwnd))
                    {
                        visibleWindows.Add(hwnd);
                    }
                    return true;
                }, 0);
            }

            // 对于每个可见窗口，检查它是否被其他窗口覆盖
            // For each visible window, check if it's covered by other windows
            foreach (var hwnd in visibleWindows)
            {
                if (IsTopMostVisibleWindow(hwnd, visibleWindows))
                {
                    var info = GetWindowInfoFromHandle(hwnd);
                    windows.Add(info);
                }
            }

            return windows;
        }

        /// <summary>
        /// 从窗口句柄获取窗口信息
        /// Get window information from handle
        /// </summary>
        private static WindowInfo GetWindowInfoFromHandle(HWND hwnd)
        {
            var result = new WindowInfo();
            PInvoke.GetWindowRect(hwnd, out var rect);
            result.IsZoomed = PInvoke.IsZoomed(hwnd);
            result.Width = rect.Width;
            result.Height = rect.Height;
            result.Title = GetWindowTitle(hwnd);
            result.ClassName = GetClassName(hwnd);
            result.IsFullScreen = IsWindowFullscreen(hwnd);
            return result;
        }

        /// <summary>
        /// 检查窗口是否为顶层可见窗口（未被其他窗口完全覆盖）
        /// Check if window is a top-most visible window (not completely covered by other windows)
        /// </summary>
        private static bool IsTopMostVisibleWindow(HWND hwnd, List<HWND> allVisibleWindows)
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

            // 获取窗口矩形
            // Get window rectangle
            PInvoke.GetWindowRect(hwnd, out var rect);

            // 跳过太小的窗口（例如任务栏图标等）
            // Skip windows that are too small (e.g., taskbar icons)
            if (rect.Width < 100 || rect.Height < 100)
            {
                return false;
            }

            // 检查是否有其他窗口完全覆盖此窗口
            // Check if there's another window completely covering this window
            foreach (var otherHwnd in allVisibleWindows)
            {
                if (otherHwnd.Equals(hwnd))
                    continue;

                // 检查Z-order：获取窗口的下一个窗口
                // Check Z-order: get the next window
                var currentWindow = PInvoke.GetWindow(otherHwnd, GET_WINDOW_CMD.GW_HWNDPREV);
                bool isOtherWindowAbove = false;

                // 遍历Z-order以查找hwnd是否在otherHwnd之下
                // Traverse Z-order to find if hwnd is below otherHwnd
                while (currentWindow != HWND.Null)
                {
                    if (currentWindow.Equals(hwnd))
                    {
                        isOtherWindowAbove = true;
                        break;
                    }
                    currentWindow = PInvoke.GetWindow(currentWindow, GET_WINDOW_CMD.GW_HWNDPREV);
                }

                if (!isOtherWindowAbove)
                    continue;

                // 检查otherHwnd是否完全覆盖hwnd
                // Check if otherHwnd completely covers hwnd
                PInvoke.GetWindowRect(otherHwnd, out var otherRect);

                if (otherRect.left <= rect.left &&
                    otherRect.top <= rect.top &&
                    otherRect.right >= rect.right &&
                    otherRect.bottom >= rect.bottom)
                {
                    return false; // 窗口被完全覆盖 / Window is completely covered
                }
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

        private static HWND _hwnd_shell;
        private static HWND HWND_SHELL =>
            _hwnd_shell != HWND.Null ? _hwnd_shell : _hwnd_shell = PInvoke.GetShellWindow();

        private static HWND _hwnd_desktop;
        private static HWND HWND_DESKTOP =>
            _hwnd_desktop != HWND.Null ? _hwnd_desktop : _hwnd_desktop = PInvoke.GetDesktopWindow();

        public static unsafe bool IsWindowFullscreen(HWND hWnd)
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
