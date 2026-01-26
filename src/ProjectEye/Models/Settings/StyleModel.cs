using System.Xml.Serialization;

namespace ProjectEye.Models.Settings
{
    [XmlRootAttribute("Style")]
    public class StyleModel
    {
        /// <summary>
        /// 语言
        /// </summary>
        public ComboxModel Language { get; set; } = new ComboxModel() { DisplayName = "English", Value = "en" };

        /// <summary>
        /// 提醒内容
        /// </summary>
        public string TipContent { get; set; }
    }
}
