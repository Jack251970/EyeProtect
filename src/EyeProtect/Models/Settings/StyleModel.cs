using CommunityToolkit.Mvvm.ComponentModel;

namespace EyeProtect.Models.Settings
{
    public partial class StyleModel : ObservableObject
    {
        /// <summary>
        /// 语言
        /// </summary>
        [ObservableProperty]
        private ComboxModel language = new() { DisplayName = "English", Value = "en" };

        /// <summary>
        /// 提醒内容
        /// </summary>
        [ObservableProperty]
        private string tipContent = "You have been using your eyes for {t} minutes. Take a break! Please focus your attention at least 6 meters away for 20 seconds!";
    }
}
