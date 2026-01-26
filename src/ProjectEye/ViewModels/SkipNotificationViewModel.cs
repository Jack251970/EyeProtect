using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using iNKORE.UI.WPF.Modern;
using ProjectEye.Core;
using ProjectEye.Core.Service;
using ProjectEye.Models;

namespace ProjectEye.ViewModels
{
    public partial class SkipNotificationViewModel : SkipNotificationModel, IViewModel
    {
        public string ScreenName { get; set; }
        public Window WindowInstance { get; set; }

        private readonly ConfigService config;
        private readonly ThemeService theme;
        private DispatcherTimer autoCloseTimer;

        public event ViewModelEventHandler ChangedEvent;

        public SkipNotificationViewModel(ConfigService config, ThemeService theme)
        {
            this.config = config;
            this.theme = theme;
            this.theme.OnChangedTheme += Theme_OnChangedTheme;
            ChangedEvent += SkipNotificationViewModel_ChangedEvent;

            // Initialize auto-close timer (5 seconds)
            autoCloseTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            autoCloseTimer.Tick += AutoCloseTimer_Tick;
        }

        private void SkipNotificationViewModel_ChangedEvent()
        {
            UpdateUIData();
            // Subscribe to IsVisibleChanged to handle window show/hide events
            WindowInstance.IsVisibleChanged += WindowInstance_IsVisibleChanged;
        }

        private void WindowInstance_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // When window becomes visible, start auto-close timer
            if (WindowInstance.IsVisible)
            {
                UpdateUIData();
                autoCloseTimer.Start();
            }
            else
            {
                autoCloseTimer.Stop();
            }
        }

        private void AutoCloseTimer_Tick(object sender, EventArgs e)
        {
            autoCloseTimer.Stop();
            WindowManager.Hide("SkipNotificationWindow");
        }

        private void Theme_OnChangedTheme(ApplicationTheme oldThemeName, ApplicationTheme newThemeName)
        {
            UpdateUIData();
        }

        private void UpdateUIData()
        {
            var isDarkTheme = ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark;
            
            // Set container background and opacity based on theme
            ContainerBackground = isDarkTheme ? new SolidColorBrush(Color.FromRgb(0x1A, 0x1B, 0x1C)) : Brushes.White;
            ContainerOpacity = 0.95;
        }

        public void SetNotificationContent(string message, string reason)
        {
            Message = message;
            Reason = reason;
        }

        public void OnChanged()
        {
            ChangedEvent?.Invoke();
        }
    }
}
