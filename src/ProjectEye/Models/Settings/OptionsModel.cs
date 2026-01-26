using System.Xml.Serialization;

namespace ProjectEye.Models.Settings
{
    [XmlRootAttribute("Options")]
    public class OptionsModel
    {
        /// <summary>
        /// 通用设置
        /// </summary>
        public GeneralModel General { get; set; }
        /// <summary>
        /// 外观
        /// </summary>
        public StyleModel Style { get; set; }
        /// <summary>
        /// 行为
        /// </summary>
        public BehaviorModel Behavior { get; set; }
    }
}
