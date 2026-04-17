using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
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
        private TrayIconFlyout flyout;
        private TextBlock messageText;

        public void Init()
        {
            messageText = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap
            };

            var content = new StackPanel
            {
                Margin = new Thickness(16),
                Orientation = Orientation.Vertical,
                Children =
                {
                    messageText
                }
            };

            flyout = new TrayIconFlyout(new MainTrayIconFlyoutWindow())
            {
                Width = 360,
                Placement = TrayIconFlyoutPlacementMode.Custom,
                PopupDirection = TrayIconFlyoutPopupDirection.Down,
                HideOnLostFocus = false
            };
            flyout.Islands.Add(new TrayIconFlyoutIsland
            {
                Content = content
            });
            flyout.CustomLocationCallback += CustomLocationCallback;

            autoHideTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(AutoHideSeconds)
            };
            autoHideTimer.Tick += AutoHideTimer_Tick;
        }

        public void Dispose()
        {
            if (autoHideTimer != null)
            {
                autoHideTimer.Stop();
                autoHideTimer.Tick -= AutoHideTimer_Tick;
                autoHideTimer = null;
            }

            flyout?.Hide();
            flyout?.Dispose();
            flyout = null;
            messageText = null;
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

                if (flyout == null || messageText == null || autoHideTimer == null)
                {
                    return;
                }

                messageText.Text = message;

                flyout.Hide();
                flyout.Show();
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
            flyout?.Hide();
        }

        private Point CustomLocationCallback(Size desireSize)
        {
            // Position the flyout at the top center of the primary screen
            var workingArea = SystemParameters.WorkArea;
            var x = workingArea.X + workingArea.Width / 2 - desireSize.Width / 2;
            var y = workingArea.Top;
            return new Point(x, y);
        }
    }
}
