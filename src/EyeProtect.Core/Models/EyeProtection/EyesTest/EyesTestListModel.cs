using System.Collections.Generic;
using System.Xml.Serialization;

namespace EyeProtect.Core.Core.Models.EyesTest
{
    [XmlRootAttribute("EyesTestList")]
    public class EyesTestListModel
    {
        public List<EyesTestModel> Data { get; set; }
    }
}
