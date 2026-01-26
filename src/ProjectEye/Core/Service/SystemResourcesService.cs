using System.Collections.Generic;
using System.Windows;
using ProjectEye.Models.Settings;

namespace ProjectEye.Core.Service
{
    public class SystemResourcesService : IService
    {
        public List<ComboxModel> Languages { get; set; }

        public void Init()
        {
            Languages = new List<ComboxModel>();

            //语言
            Languages.Add(new ComboxModel()
            {
                DisplayName = "English",
                Value = "en"
            });
            Languages.Add(new ComboxModel()
            {
                DisplayName = "简体中文",
                Value = "zh-cn"
            });
            Languages.Add(new ComboxModel()
            {
                DisplayName = "繁體中文",
                Value = "zh-tw"
            });
        }
    }
}
