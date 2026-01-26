using System.Xml.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectEye.Models.Settings
{
    [XmlRootAttribute("Style")]
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
        private string tipContent;
    }
}
