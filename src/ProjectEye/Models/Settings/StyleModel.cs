using System.Xml.Serialization;

namespace ProjectEye.Models.Settings
{
    [XmlRootAttribute("Style")]
    public class StyleModel
    {
        /// <summary>
        /// 提醒内容
        /// </summary>
        public string TipContent { get; set; }

        /// <summary>
        /// 休息提示询问，如果开启可以使用预提醒功能以及在全屏提示窗口点击，关闭后则每到休息时间直接开始计时不询问，且支持鼠标穿透
        /// </summary>
        public bool IsTipAsk { get; set; } = true;
        /// <summary>
        /// 全屏提示窗口鼠标穿透
        /// </summary>
        public bool IsThruTipWindow { get; set; } = false;

        /// <summary>
        /// 语言
        /// </summary>
        public ComboxModel Language { get; set; } = new ComboxModel() { DisplayName = "English", Value = "en" };
    }
}
