using System.Xml.Serialization;

namespace EyeProtect.Core.Core.Models.Options
{
    [XmlRootAttribute("theme")]
    public class ThemeModel
    {
        public int ID { get; set; }
        public string DisplayName { get; set; } = "默认";

        public string ThemeName { get; set; } = "Default";
        public string ThemeColor { get; set; } = "#ffc83d";

    }
}
