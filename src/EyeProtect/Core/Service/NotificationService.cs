using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using U5BFA.Libraries;

namespace EyeProtect.Core.Service
{
    /// <summary>
    /// Notification Service for TrayIconFlyout Notifications
    /// </summary>
    public class NotificationService : IService
    {
        private const int AutoHideSeconds = 4;

        private readonly DispatcherTimer autoHideTimer = new();
        private readonly TrayIconFlyout flyout = new();
        private readonly TextBlock titleText = new();
        private readonly TextBlock messageText = new();

        public void Init()
        {
            titleText.FontSize = 16;
            titleText.FontWeight = FontWeights.SemiBold;
            messageText.Margin = new Thickness(0, 8, 0, 0);
            messageText.TextWrapping = TextWrapping.Wrap;

            var content = new StackPanel
            {
                Margin = new Thickness(16),
                Children =
                {
                    titleText,
                    messageText
                }
            };

            flyout.Width = 360;
            flyout.Placement = TrayIconFlyoutPlacementMode.TopRight;
            flyout.PopupDirection = TrayIconFlyoutPopupDirection.Down;
            flyout.HideOnLostFocus = false;
            flyout.Islands.Add(new TrayIconFlyoutIsland
            {
                Content = content
            });

            autoHideTimer.Interval = TimeSpan.FromSeconds(AutoHideSeconds);
            autoHideTimer.Tick += AutoHideTimer_Tick;
        }

        public void Dispose()
        {
            autoHideTimer.Stop();
            autoHideTimer.Tick -= AutoHideTimer_Tick;
            flyout.Hide();
            flyout.Dispose();
        }

        /// <summary>
        /// Show a top flyout notification for skipped break
        /// </summary>
        /// <param name="reason">Reason for skipping the break (fullscreen or ignored app)</param>
        public void ShowBreakSkippedNotification(string reason)
        {
            try
            {
                var title = Application.Current.TryFindResource("Lang_NotificationTitle") as string ?? "Eye Protect Reminder";
                var message = Application.Current.TryFindResource("Lang_NotificationBreakSkipped") as string ?? "Break reminder skipped: {0}";
                message = string.Format(message, reason);

                titleText.Text = title;
                messageText.Text = message;

                if (flyout.IsOpen)
                {
                    flyout.Hide();
                }

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
            autoHideTimer.Stop();
            flyout.Hide();
        }
    }
}
