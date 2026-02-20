using System;
using System.Windows;
using System.Windows.Threading;
using EyeProtect.Core;
using EyeProtect.Core.Service;
using U5BFA.Libraries;

namespace EyeProtect.Views
{
    public partial class MainTrayIconFlyout : TrayIconFlyout
    {
        private readonly MainService mainService;
        private readonly DispatcherTimer uiTimer;

        public MainTrayIconFlyout(MainService mainService) : base(new MainTrayIconFlyoutWindow())
        {
            this.mainService = mainService;
            InitializeComponent();

            // Setup UI timer to update countdown
            uiTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            uiTimer.Tick += UiTimer_Tick;

            Loaded += MainTrayIconFlyout_Loaded;
            Unloaded += MainTrayIconFlyout_Unloaded;
        }

        private void MainTrayIconFlyout_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateStatus();
            uiTimer.Start();
        }

        private void MainTrayIconFlyout_Unloaded(object sender, RoutedEventArgs e)
        {
            uiTimer.Stop();
        }

        private void UiTimer_Tick(object sender, EventArgs e)
        {
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            if (mainService == null) return;

            // Update Countdown
            var minutesLeft = mainService.GetRestCountdownMinutes();

            if (minutesLeft < 0) minutesLeft = 0;

            var span = TimeSpan.FromMinutes(minutesLeft);
            if (span.TotalHours >= 1)
            {
                TimerText.Text = $"{(int)span.TotalHours}:{span.Minutes:D2}:{span.Seconds:D2}";
            }
            else
            {
                TimerText.Text = $"{span.Minutes:D2}:{span.Seconds:D2}";
            }

            // Update Status based solely on timer running state
            bool isRunning = mainService.IsWorkTimerRun();

            if (isRunning)
            {
                StatusText.Text = Application.Current.Resources["Lang_Thenextbreak"] as string ?? "Next break in";
                PauseIcon.Glyph = "\uE769"; // Pause icon
                PauseText.Text = "Pause";
                BtnPause.ToolTip = "Pause";
            }
            else
            {
                StatusText.Text = Application.Current.Resources["Lang_Reminderisoff"] as string ?? "Reminder is off";
                TimerText.Text = "--:--"; // Or keep showing time
                PauseIcon.Glyph = "\uE768"; // Play icon
                PauseText.Text = "Resume";
                BtnPause.ToolTip = "Resume";
            }
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            // Simple toggle: Start if paused, Pause if running
            if (mainService.IsWorkTimerRun())
            {
                mainService.Pause(false); // Pause without stopping statistic
            }
            else
            {
                mainService.Start();
            }
            UpdateStatus();
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.CreateWindowInScreen("OptionsWindow");
            WindowManager.Show("OptionsWindow");
            Hide();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.Hide("OptionsWindow");
            Hide();
            Application.Current.Shutdown();
        }
    }
}
