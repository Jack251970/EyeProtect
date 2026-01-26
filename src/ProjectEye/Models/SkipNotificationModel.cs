using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectEye.Models
{
    public partial class SkipNotificationModel : ObservableObject
    {
        /// <summary>
        /// Notification message text
        /// </summary>
        [ObservableProperty]
        private string message;

        /// <summary>
        /// Skip reason text
        /// </summary>
        [ObservableProperty]
        private string reason;

        /// <summary>
        /// Container background
        /// </summary>
        [ObservableProperty]
        private System.Windows.Media.Brush containerBackground;

        /// <summary>
        /// Container opacity
        /// </summary>
        [ObservableProperty]
        private double containerOpacity = 0.95;
    }
}
