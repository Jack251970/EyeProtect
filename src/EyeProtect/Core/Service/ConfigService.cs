using System;
using System.IO;
using EyeProtect.Models.Settings;

namespace EyeProtect.Core.Service
{
    /// <summary>
    /// 配置文件  Service
    /// </summary>
    public class ConfigService : IService
    {
        private readonly string configPath;
        private readonly JsonExtensions jsonExtensions;
        private readonly SystemResourcesService systemResources;

        /// <summary>
        /// 存放文件夹
        /// </summary>
        private readonly string dir = "Data";

        /// <summary>
        /// 配置
        /// </summary>
        public OptionsModel options { get; set; }

        public ConfigService(SystemResourcesService systemResources)
        {
            this.systemResources = systemResources;
            configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir, "config.json");
            jsonExtensions = new JsonExtensions(configPath);
            LogHelper.Info("ConfigService initialized. Config path: " + configPath);
        }

        public void Init()
        {
            // Read config
            if (File.Exists(configPath))
            {
                var obj = jsonExtensions.ToModel(typeof(OptionsModel));
                if (obj is OptionsModel model)
                {
                    options = model;
                }
            }

            // Create default config if not exist
            if (options == null)
            {
                options = new OptionsModel
                {
                    General = new GeneralModel(),
                    Style = new StyleModel()
                    {
                        Language = systemResources.Languages[0]
                    },
                    Behavior = new BehaviorModel()
                };
                Save();
            }
            else
            {
                if (options.General == null)
                {
                    options.General = new GeneralModel();
                    Save();
                }
                if (options.Style == null)
                {
                    options.Style = new StyleModel()
                    {
                        Language = systemResources.Languages[0]
                    };
                    Save();
                }
                if (options.Behavior == null)
                {
                    options.Behavior = new BehaviorModel();
                    Save();
                }
            }

            // Reset flag
            options.General.Noreset = false;

            LogHelper.Info("ConfigService initialized. Config loaded.");
        }

        public bool Save()
        {
            if (options != null)
            {
                return jsonExtensions.Save(options);
            }
            return false;
        }

        public void Dispose()
        {

        }
    }
}
