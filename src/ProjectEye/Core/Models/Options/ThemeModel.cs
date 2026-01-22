using System.Xml.Serialization;

namespace ProjectEye.Core.Models.Options
{
    [XmlRootAttribute("theme")]
    public class ThemeModel
    {
        public int ID { get; set; }
        public string DisplayName { get; set; } = "浅色";

        public string ThemeName { get; set; } = "Light";
        public string ThemeColor { get; set; } = "#ffc83d";

    }
}
