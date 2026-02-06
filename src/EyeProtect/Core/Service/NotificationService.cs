using System;
using System.Windows;
using Microsoft.Toolkit.Uwp.Notifications;

namespace EyeProtect.Core.Service
{
    /// <summary>
    /// Notification Service for Windows Toast Notifications
    /// </summary>
    public class NotificationService : IService
    {
        private MainService mainService;
        private RestService restService;

        public void Init()
        {
            // Subscribe to toast activation events
            ToastNotificationManagerCompat.OnActivated += OnToastActivated;
        }

        public void Exit()
        {
            ToastNotificationManagerCompat.OnActivated -= OnToastActivated;
            ToastNotificationManagerCompat.Uninstall();
        }

        /// <summary>
        /// Set the main service dependency for handling button actions
        /// </summary>
        public void SetMainService(MainService main)
        {
            this.mainService = main;
        }

        /// <summary>
        /// Set the rest service dependency for handling button actions
        /// </summary>
        public void SetRestService(RestService rest)
        {
            this.restService = rest;
        }

        /// <summary>
        /// Handle toast notification button clicks
        /// </summary>
        private void OnToastActivated(ToastNotificationActivatedEventArgsCompat e)
        {
            try
            {
                // Parse the arguments to determine which action was clicked
                var args = ToastArguments.Parse(e.Argument);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (args.Contains("action"))
                    {
                        var action = args["action"];

                        if (action == "startrest" && mainService != null && restService != null)
                        {
                            // Start rest - same as Reset command
                            mainService.StopBusyListener();
                            restService.Start();
                        }
                        else if (action == "restlater" && mainService != null)
                        {
                            // Rest later - same as Busy command
                            mainService.StopBusyListener();
                            mainService.ReStart();
                            WindowManager.Hide("TipWindow");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                LogHelper.Warning($"Failed to handle toast activation: {ex.Message}");
            }
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

                var startRestText = Application.Current.TryFindResource("Lang_StartRest") as string ?? "Start rest";
                var restLaterText = Application.Current.TryFindResource("Lang_RestLater") as string ?? "Rest later";

                new ToastContentBuilder()
                    .AddText(title)
                    .AddText(message)
                    .AddButton(new ToastButton()
                        .SetContent(startRestText)
                        .AddArgument("action", "startrest"))
                    .AddButton(new ToastButton()
                        .SetContent(restLaterText)
                        .AddArgument("action", "restlater"))
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
