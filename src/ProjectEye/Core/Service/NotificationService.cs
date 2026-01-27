using System;
using System.Windows;
using Microsoft.Toolkit.Uwp.Notifications;

namespace ProjectEye.Core.Service
{
    /// <summary>
    /// Notification Service for Windows Toast Notifications
    /// </summary>
    public class NotificationService : IService
    {
        public void Init()
        {
            // No initialization needed for toast notifications
        }

        public void Exit()
        {
            ToastNotificationManagerCompat.Uninstall();
        }

        /// <summary>
        /// Show a toast notification for skipped break
        /// </summary>
        /// <param name="reason">Reason for skipping the break (fullscreen or ignored app)</param>
        public void ShowBreakSkippedNotification(string reason)
        {
            try
            {
                var title = Application.Current.TryFindResource("Lang_NotificationTitle") as string ?? "Eye Protect Reminder";
                var message = Application.Current.TryFindResource("Lang_NotificationBreakSkipped") as string ?? "Break reminder skipped: {0}";
                message = string.Format(message, reason);

                new ToastContentBuilder()
                    .AddText(title)
                    .AddText(message)
                    .Show();
            }
            catch (Exception ex)
            {
                // Log error but don't crash the app
                LogHelper.Warning($"Failed to show notification: {ex.Message}");
            }
        }

        /// <summary>
        /// Show a toast notification for fullscreen app detection
        /// </summary>
        public void ShowFullscreenSkippedNotification()
        {
            var reason = Application.Current.TryFindResource("Lang_NotificationFullscreen") as string ?? "Fullscreen application detected";
            ShowBreakSkippedNotification(reason);
        }

        /// <summary>
        /// Show a toast notification for ignored app detection
        /// </summary>
        public void ShowIgnoredAppSkippedNotification()
        {
            var reason = Application.Current.TryFindResource("Lang_NotificationIgnoredApp") as string ?? "Ignored application is running";
            ShowBreakSkippedNotification(reason);
        }
    }
}
