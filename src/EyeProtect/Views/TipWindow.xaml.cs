using System;
using System.Windows;
using System.Windows.Media.Animation;
using EyeProtect.ViewModels;

namespace EyeProtect.Views
{
    /// <summary>
    /// TipWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TipWindow : Window
    {
        private const double FadeInDuration = 0.9; // seconds
        private const double FadeOutDuration = 0.6; // seconds
        private bool isAnimating = false;
        
        public TipWindow()
        {
            InitializeComponent();
            
            // Subscribe to IsVisibleChanged event to handle fade-in animation
            IsVisibleChanged += OnIsVisibleChanged;
            
            // Subscribe to Closed event for cleanup
            Closed += OnClosed;
        }

        /// <summary>
        /// Clean up event handlers when window is closed
        /// </summary>
        private void OnClosed(object sender, EventArgs e)
        {
            IsVisibleChanged -= OnIsVisibleChanged;
            Closed -= OnClosed;
            // Clean up ViewModel resources
            if (DataContext is TipViewModel viewModel)
            {
                viewModel.Dispose();
            }
            Closed -= OnClosed;
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
