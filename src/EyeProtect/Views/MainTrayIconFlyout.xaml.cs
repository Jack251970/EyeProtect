using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.DependencyInjection;
using EyeProtect.Core;
using EyeProtect.Core.Service;
using U5BFA.Libraries;

namespace EyeProtect.Views
{
    public partial class MainTrayIconFlyout : TrayIconFlyout
    {
        private readonly MainService mainService;
        private readonly ConfigService config;
        private readonly DispatcherTimer uiTimer;

        public MainTrayIconFlyout(MainService mainService, ConfigService config) : base(new MainTrayIconFlyoutWindow())
        {
            this.mainService = mainService;
            this.config = config;
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
            if (mainService.IsWorkTimerRun())
            {
                StatusText.Text = Application.Current.Resources["Lang_Thenextbreak"] as string ?? "Next break in";
            }
            else
            {
                StatusText.Text = Application.Current.Resources["Lang_Reminderisoff"] as string ?? "Reminder is off";
                TimerText.Text = "--:--"; // Show dashes when not running
            }
            if (config.options.General.Noreset)
            {
                PauseIcon.Glyph = "\uE768"; // Play icon
                PauseText.Text = Application.Current.Resources["Lang_Resume"] as string ?? "Resume";
                BtnPause.ToolTip = Application.Current.Resources["Lang_Resume"] as string ?? "Resume";
            }
            else
            {
                PauseIcon.Glyph = "\uE769"; // Pause icon
                PauseText.Text = Application.Current.Resources["Lang_Suspend"] as string ?? "Suspend";
                BtnPause.ToolTip = Application.Current.Resources["Lang_Suspend"] as string ?? "Suspend";
            }
        }

        private void BtnStartRest_Click(object sender, RoutedEventArgs e)
        {
            mainService.StartRestNow();
            Hide();
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            if (!config.options.General.Noreset)
            {
                // Show the suspend options context menu above the button
                BtnPause.ContextMenu.PlacementTarget = BtnPause;
                BtnPause.ContextMenu.Placement = PlacementMode.Top;
                BtnPause.ContextMenu.IsOpen = true;
            }
            else
            {
                ApplySuspend(-1);
            }
        }

        private void MenuItemOneHour_Click(object sender, RoutedEventArgs e)
        {
            ApplySuspend(1);
        }

        private void MenuItemTwoHour_Click(object sender, RoutedEventArgs e)
        {
            ApplySuspend(2);
        }

        private void MenuItemForever_Click(object sender, RoutedEventArgs e)
        {
            ApplySuspend(0);
        }

        private void ApplySuspend(int hour)
        {
            var trayService = Ioc.Default.GetRequiredService<TrayService>();
            trayService.SetNoReset(hour);
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
