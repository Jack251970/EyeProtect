using CommunityToolkit.Mvvm.ComponentModel;

namespace EyeProtect.Models.Settings
{
    public partial class StyleModel : ObservableObject
    {
        /// <summary>
        /// 语言
        /// </summary>
        [ObservableProperty]
        private ComboxModel language = new ComboxModel() { DisplayName = "English", Value = "en" };

        /// <summary>
        /// 提醒内容
        /// </summary>
        [ObservableProperty]
        private string tipContent = string.Empty;
    }
}
