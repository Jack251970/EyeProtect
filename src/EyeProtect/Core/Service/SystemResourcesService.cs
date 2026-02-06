using System.Collections.Generic;
using EyeProtect.Models.Settings;

namespace EyeProtect.Core.Service
{
    public class SystemResourcesService : IService
    {
        public List<ComboxModel> Languages { get; set; }

        public void Init()
        {
            Languages =
            [
                //语言
                new ComboxModel()
                {
                    DisplayName = "English",
                    Value = "en"
                },
                new ComboxModel()
                {
                    DisplayName = "简体中文",
                    Value = "zh-cn"
                },
                new ComboxModel()
                {
                    DisplayName = "繁體中文",
                    Value = "zh-tw"
                },
            ];
        }

        public void Dispose()
        {

        }
    }
}
