using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using EyeProtect.Models;
using EyeProtect.Views;
using U5BFA.Libraries;

namespace EyeProtect.Core.Service
{
    /// <summary>
    /// Notification Service for TrayIconFlyout Notifications
    /// </summary>
    public class NotificationService : IService
    {
        private const int AutoHideSeconds = 4;

        private DispatcherTimer autoHideTimer;
        private readonly List<TrayIconFlyout> flyouts = [];
        private readonly List<TextBlock> messageTexts = [];

        public void Init()
        {
            autoHideTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(AutoHideSeconds)
            };
            autoHideTimer.Tick += AutoHideTimer_Tick;

            CreateFlyoutsForAllScreens();
        }

        private void CreateFlyoutsForAllScreens()
        {
            foreach (var flyout in flyouts)
            {
                flyout.Hide();
                flyout.Dispose();
            }
            flyouts.Clear();
            messageTexts.Clear();

            var monitors = MonitorInfo.GetDisplayMonitors();
            foreach (var monitor in monitors)
            {
                var messageText = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap
                };

                var content = new StackPanel
                {
                    Margin = new Thickness(16),
                    Orientation = Orientation.Vertical,
                    Children = { messageText }
                };

                var flyout = new TrayIconFlyout(new MainTrayIconFlyoutWindow())
                {
                    Width = 360,
                    Placement = TrayIconFlyoutPlacementMode.Custom,
                    PopupDirection = TrayIconFlyoutPopupDirection.Down,
                    HideOnLostFocus = false
                };
                flyout.Islands.Add(new TrayIconFlyoutIsland { Content = content });

                var capturedMonitor = monitor;
                flyout.CustomLocationCallback += desireSize =>
                {
                    var workingArea = capturedMonitor.WorkingArea;
                    var dpi = capturedMonitor.GetDpi(Windows.Win32.UI.HiDpi.MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI);
                    var scale = dpi.x / 96.0;
                    var x = workingArea.X / scale + workingArea.Width / scale / 2 - desireSize.Width / 2;
                    var y = workingArea.Y / scale;
                    return new Point(x, y);
                };

                flyouts.Add(flyout);
                messageTexts.Add(messageText);
            }
        }

        public void Dispose()
        {
            if (autoHideTimer != null)
            {
                autoHideTimer.Stop();
                autoHideTimer.Tick -= AutoHideTimer_Tick;
                autoHideTimer = null;
            }

            foreach (var flyout in flyouts)
            {
                flyout.Hide();
                flyout.Dispose();
            }
            flyouts.Clear();
            messageTexts.Clear();
        }

        /// <summary>
        /// Show a top flyout notification for skipped break
        /// </summary>
        /// <param name="reason">Reason for skipping the break (fullscreen or ignored app)</param>
        public void ShowBreakSkippedNotification(string reason)
        {
            try
            {
                var message = Application.Current.TryFindResource("Lang_NotificationBreakSkipped") as string ?? "Break reminder skipped: {0}";
                message = string.Format(message, reason);

                if (flyouts.Count == 0 || messageTexts.Count == 0 || autoHideTimer == null)
                {
                    return;
                }

                foreach (var messageText in messageTexts)
                {
                    messageText.Text = message;
                }

                foreach (var flyout in flyouts)
                {
                    flyout.Hide();
                    flyout.Show();
                }
                autoHideTimer.Stop();
                autoHideTimer.Start();
            }
            catch (Exception ex)
            {
                // Log error but don't crash the app
                LogHelper.Warning($"Failed to show notification: {ex.Message}");
            }
        }

        /// <summary>
        /// Show a flyout notification for fullscreen app detection
        /// </summary>
        public void ShowFullscreenSkippedNotification()
        {
            var reason = Application.Current.TryFindResource("Lang_NotificationFullscreen") as string ?? "Fullscreen application detected";
            ShowBreakSkippedNotification(reason);
        }

        /// <summary>
        /// Show a flyout notification for ignored app detection
        /// </summary>
        public void ShowIgnoredAppSkippedNotification()
        {
            var reason = Application.Current.TryFindResource("Lang_NotificationIgnoredApp") as string ?? "Ignored application is running";
            ShowBreakSkippedNotification(reason);
        }

        private void AutoHideTimer_Tick(object sender, EventArgs e)
        {
            autoHideTimer?.Stop();
            foreach (var flyout in flyouts)
            {
                flyout.Hide();
            }
        }
    }
}
