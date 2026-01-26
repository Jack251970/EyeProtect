using System;
using System.Windows;
using System.Windows.Media.Animation;
using ProjectEye.Core;
using ProjectEye.Models;

namespace ProjectEye.Views
{
    /// <summary>
    /// SkipNotificationWindow.xaml interaction logic
    /// </summary>
    public partial class SkipNotificationWindow : Window
    {
        private const double FadeInDuration = 0.3; // seconds
        private const double FadeOutDuration = 0.3; // seconds
        private bool isAnimating = false;
        
        public SkipNotificationWindow()
        {
            InitializeComponent();
            
            // Subscribe to IsVisibleChanged event to handle fade-in animation
            IsVisibleChanged += OnIsVisibleChanged;
            
            // Subscribe to Closed event for cleanup
            Closed += OnClosed;
            
            // Subscribe to Loaded event to position window
            Loaded += OnLoaded;
        }

        /// <summary>
        /// Position window at top-right corner of the screen
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Try to get the screen this window is associated with from the view model
            if (DataContext is ViewModels.SkipNotificationViewModel viewModel && !string.IsNullOrEmpty(viewModel.ScreenName))
            {
                // Find the monitor info for this screen
                var monitors = MonitorInfo.GetDisplayMonitors();
                MonitorInfo targetMonitor = null;
                
                foreach (var monitor in monitors)
                {
                    if (monitor.Name.Replace("\\", "") == viewModel.ScreenName)
                    {
                        targetMonitor = monitor;
                        break;
                    }
                }
                
                if (targetMonitor != null)
                {
                    // Position at top-right corner of the specific monitor with margins
                    var size = WindowManager.GetSize(targetMonitor);
                    var screenLeft = WindowManager.ToDips(targetMonitor.Bounds.Left, size.XDPI);
                    var screenTop = WindowManager.ToDips(targetMonitor.Bounds.Top, size.YDPI);
                    
                    Left = screenLeft + size.Width - Width - 20; // 20px margin from right
                    Top = screenTop + 20; // 20px margin from top
                    return;
                }
            }
            
            // Fallback: Position at top-right corner of primary work area
            var workArea = SystemParameters.WorkArea;
            Left = workArea.Right - Width - 20; // 20px margin from right
            Top = workArea.Top + 20; // 20px margin from top
        }

        /// <summary>
        /// Clean up event handlers when window is closed
        /// </summary>
        private void OnClosed(object sender, EventArgs e)
        {
            IsVisibleChanged -= OnIsVisibleChanged;
            Closed -= OnClosed;
            Loaded -= OnLoaded;
        }

        /// <summary>
        /// Check if system animations are enabled
        /// </summary>
        private static bool IsSystemAnimationEnabled()
        {
            // Check if Windows has animations enabled
            return SystemParameters.MenuAnimation;
        }

        /// <summary>
        /// Handle visibility changes to apply fade-in animation
        /// </summary>
        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsSystemAnimationEnabled())
            {
                // If system animations are disabled, keep default behavior
                return;
            }

            if (IsVisible && !isAnimating)
            {
                // Set initial opacity to 0 for fade-in effect
                Opacity = 0.0;
                
                // Fade in when window becomes visible
                var fadeIn = new DoubleAnimation
                {
                    From = 0.0,
                    To = 1.0,
                    Duration = TimeSpan.FromSeconds(FadeInDuration),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                BeginAnimation(OpacityProperty, fadeIn);
            }
        }

        /// <summary>
        /// Override Hide to add fade-out animation
        /// </summary>
        public new void Hide()
        {
            if (!IsSystemAnimationEnabled())
            {
                // If system animations are disabled, use default hide
                base.Hide();
                return;
            }

            if (!IsVisible)
            {
                // Already hidden
                return;
            }

            // Prevent IsVisibleChanged from triggering during animation
            isAnimating = true;

            // Fade out animation
            var fadeOut = new DoubleAnimation
            {
                From = Opacity,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(FadeOutDuration),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };

            fadeOut.Completed += (s, e) =>
            {
                isAnimating = false;
                base.Hide();
                // Reset opacity for next show
                Opacity = 1.0;
            };

            BeginAnimation(OpacityProperty, fadeOut);
        }
    }
}
