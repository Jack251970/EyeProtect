using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using ProjectEye.Core.Models.Options;

namespace ProjectEye.Core.Service
{
    /// <summary>
    /// 配置文件  Service
    /// </summary>
    public class ConfigService : IService
    {
        private readonly string configPath;
        private readonly XmlExtensions xmlExtensions;
        private readonly SystemResourcesService systemResources;
        //存放文件夹
        private readonly string dir = "Data";
        /// <summary>
        /// 未更改的配置
        /// </summary>
        private OptionsModel oldOptions_;
        public OptionsModel options { get; set; }
        /// <summary>
        /// 配置文件被修改时发生
        /// </summary>
        public event EventHandler Changed;
        public ConfigService(SystemResourcesService systemResources)
        {
            this.systemResources = systemResources;
            configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                dir,
                "config.xml");
            xmlExtensions = new XmlExtensions(configPath);
            oldOptions_ = new OptionsModel();
        }
        public void Init()
        {
            if (File.Exists(configPath))
            {
                var obj = xmlExtensions.ToModel(typeof(OptionsModel));
                if (obj != null)
                {
                    options = obj as OptionsModel;
                    SaveOldOptions();
                }
                else
                {
                    CreateDefaultConfig();
                }
            }
            else
            {
                CreateDefaultConfig();
            }
            CheckOptions();
            //每次启动都把不提醒重置
            options.General.Noreset = false;
        }
        public bool Save()
        {
            if (options != null)
            {
                OnChanged();
                SaveOldOptions();
                return xmlExtensions.Save(options);
            }
            return false;
        }

        /// <summary>
        /// 保存旧选项数据
        /// </summary>
        public void SaveOldOptions()
        {
            var optionsStr = JsonConvert.SerializeObject(options);
            oldOptions_ = JsonConvert.DeserializeObject<OptionsModel>(optionsStr);
        }
        /// <summary>
        /// 创建默认配置文件
        /// </summary>
        private void CreateDefaultConfig()
        {
            options = new OptionsModel();
            options.General = new GeneralModel();
            options.General.Data = false;
            options.General.Noreset = false;
            options.General.Sound = true;
            options.General.Startup = false;
            options.General.LeaveListener = true;
            options.General.WarnTime = 20;

            options.Style = new StyleModel();
            options.Style.Theme = systemResources.Themes[0];
            options.Style.TipContent = "You have been using your eyes for {t} minutes. Take a break! Please focus your attention at least 6 meters away for 20 seconds!";
            options.Style.TipWindowAnimation = systemResources.Animations[0];
            options.Style.Language = systemResources.Languages[0];

            options.Behavior = new BehaviorModel();

            SaveOldOptions();

            xmlExtensions.Save(options);
        }
        private void CheckOptions()
        {
            CheckOptions(options);
            CheckOptions(options.Style);
            SaveOldOptions();
        }
        private void CheckOptions(object obj)
        {
            var properties = obj.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            foreach (var item in properties)
            {
                var name = item.Name;
                var value = item.GetValue(obj, null);
                if (value == null)
                {
                    //配置项不存在时创建
                    var types = new Type[0];
                    var objs = new object[0];

                    var ctor = item.PropertyType.GetConstructor(types);
                    if (ctor != null)
                    {
                        var instance = ctor.Invoke(objs);
                        item.SetValue(obj, instance);
                    }
                }

                Debug.WriteLine(string.Format("{0}:{1},", name, value));
            }
        }

        public void OnChanged()
        {
            if (oldOptions_ == null || options == null)
            {
                return;
            }
            Changed?.Invoke(oldOptions_, null);
        }

    }
}
