// Copyright (c) Files Community
// Licensed under the MIT License.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;

#nullable enable

namespace EyeProtect.Models.Taskbar
{
    /// <summary>
    /// Represents a tray icon of Notification Area so-called System Tray.
    /// </summary>
    public sealed partial class SystemTrayIcon : IDisposable
    {
        // Constants

        private const uint WM_FILES_UNIQUE_MESSAGE = 2048u;

        // Fields

        private readonly SystemTrayIconWindow _IconWindow;

        private readonly uint _taskbarRestartMessageId;

        private bool _notifyIconCreated;

        // Properties

        public Guid Id { get; private set; }

        private bool _IsVisible;
        public bool IsVisible
        {
            get => _IsVisible;
            private set
            {
                if (_IsVisible != value)
                {
                    _IsVisible = value;

                    if (!value)
                        DeleteNotifyIcon();
                    else
                        CreateOrModifyNotifyIcon();
                }
            }
        }

        private string _Tooltip;
        public string Tooltip
        {
            get => _Tooltip;
            set
            {
                if (_Tooltip != value)
                {
                    _Tooltip = value;

                    CreateOrModifyNotifyIcon();
                }
            }
        }

        private Icon _Icon;
        public Icon Icon
        {
            get => _Icon;
            set
            {
                if (_Icon != value)
                {
                    _Icon = value;

                    CreateOrModifyNotifyIcon();
                }
            }
        }

        // Events

        public event EventHandler<MouseEventReceivedEventArgs>? LeftClicked;
        public event EventHandler<MouseEventReceivedEventArgs>? RightClicked;
        public event EventHandler<MouseEventReceivedEventArgs>? MouseMoved;

        // Constructor

        /// <summary>
        /// Initializes an instance of <see cref="SystemTrayIcon"/>.
        /// </summary>
        /// <remarks>
        /// Note that initializing an instance won't make the icon visible.
        /// </remarks>
        public SystemTrayIcon(Icon icon, string tooltip, Guid id, bool isVisible = true)
        {
            _Icon = icon;
            _Tooltip = tooltip;
            _taskbarRestartMessageId = PInvoke.RegisterWindowMessage("TaskbarCreated");

            Id = id;
            _IsVisible = isVisible;
            _IconWindow = new SystemTrayIconWindow(this);

            CreateOrModifyNotifyIcon();
        }

        // Public Methods

        /// <summary>
        /// Shows the tray icon.
        /// </summary>
        public SystemTrayIcon Show()
        {
            IsVisible = true;

            return this;
        }

        /// <summary>
        /// Hides the tray icon.
        /// </summary>
        public SystemTrayIcon Hide()
        {
            IsVisible = false;

            return this;
        }

        // Private Methods

        private void CreateOrModifyNotifyIcon()
        {
            if (IsVisible)
            {
                NOTIFYICONDATAW lpData = default;

                lpData.cbSize = (uint)Marshal.SizeOf<NOTIFYICONDATAW>();
                lpData.hWnd = _IconWindow.WindowHandle;
                lpData.uCallbackMessage = WM_FILES_UNIQUE_MESSAGE;
                lpData.hIcon = (Icon != null) ? new HICON(Icon.Handle) : default;
                lpData.guidItem = Id;
                lpData.uFlags = NOTIFY_ICON_DATA_FLAGS.NIF_MESSAGE | NOTIFY_ICON_DATA_FLAGS.NIF_ICON | NOTIFY_ICON_DATA_FLAGS.NIF_TIP | NOTIFY_ICON_DATA_FLAGS.NIF_GUID | NOTIFY_ICON_DATA_FLAGS.NIF_SHOWTIP;
                lpData.szTip = _Tooltip ?? string.Empty;

                if (!_notifyIconCreated)
                {
                    // Delete the existing icon
                    PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_DELETE, in lpData);

                    _notifyIconCreated = true;

                    // Add a new icon
                    PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_ADD, in lpData);

                    lpData.Anonymous.uVersion = 4u;

                    // Set the icon handler version
                    // NOTE: Do not omit this code. If you remove, the icon won't be shown.
                    PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_SETVERSION, in lpData);
                }
                else
                {
                    // Modify the existing icon
                    PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_MODIFY, in lpData);
                }
            }
        }

        private void DeleteNotifyIcon()
        {
            if (_notifyIconCreated)
            {
                _notifyIconCreated = false;

                NOTIFYICONDATAW lpData = default;

                lpData.cbSize = (uint)Marshal.SizeOf<NOTIFYICONDATAW>();
                lpData.hWnd = _IconWindow.WindowHandle;
                lpData.guidItem = Id;
                lpData.uFlags = NOTIFY_ICON_DATA_FLAGS.NIF_MESSAGE | NOTIFY_ICON_DATA_FLAGS.NIF_ICON | NOTIFY_ICON_DATA_FLAGS.NIF_TIP | NOTIFY_ICON_DATA_FLAGS.NIF_GUID | NOTIFY_ICON_DATA_FLAGS.NIF_SHOWTIP;

                // Delete the existing icon
                PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_DELETE, in lpData);
            }
        }

        internal LRESULT WindowProc(HWND hWnd, uint uMsg, WPARAM wParam, LPARAM lParam)
        {
            switch (uMsg)
            {
                case WM_FILES_UNIQUE_MESSAGE:
                    {
                        switch ((uint)(lParam.Value & 0xFFFF))
                        {
                            case PInvoke.WM_MOUSEMOVE:
                                {
                                    PInvoke.SetForegroundWindow(hWnd);
                                    MouseMoved?.Invoke(this, new MouseEventReceivedEventArgs());

                                    break;
                                }
                            case PInvoke.WM_LBUTTONUP:
                                {
                                    PInvoke.SetForegroundWindow(hWnd);
                                    LeftClicked?.Invoke(this, new MouseEventReceivedEventArgs());

                                    break;
                                }
                            case PInvoke.WM_RBUTTONUP:
                                {
                                    PInvoke.SetForegroundWindow(hWnd);
                                    RightClicked?.Invoke(this, new MouseEventReceivedEventArgs());

                                    break;
                                }
                        }

                        break;
                    }
                case PInvoke.WM_DESTROY:
                    {
                        DeleteNotifyIcon();

                        break;
                    }
                default:
                    {
                        if (uMsg == _taskbarRestartMessageId)
                        {
                            DeleteNotifyIcon();
                            CreateOrModifyNotifyIcon();
                        }

                        return PInvoke.DefWindowProc(hWnd, uMsg, wParam, lParam);
                    }
            }
            return default;
        }

        public void Dispose()
        {
            _IconWindow.Dispose();
        }
    }

    public class MouseEventReceivedEventArgs : EventArgs
    {
        
    }
}
