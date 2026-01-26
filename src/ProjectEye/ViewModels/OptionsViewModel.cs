using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using ProjectEye.Core;
using ProjectEye.Core.Service;
using ProjectEye.Models;

namespace ProjectEye.ViewModels
{
    public partial class OptionsViewModel
    {
        public OptionsModel Model { get; set; }

        private readonly ConfigService config;
        private readonly MainService mainService;
        private readonly ThemeService theme;
        public OptionsViewModel(ConfigService config,
            MainService mainService,
            SystemResourcesService systemResources,
            SoundService sound,
            ThemeService theme)
        {
            this.config = config;
            this.mainService = mainService;
            this.theme = theme;
            Model = new OptionsModel
            {
                Data = config.options,
                Languages = systemResources.Languages,
                AvailableApplications = ApplicationHelper.GetInstalledApplications()
            };

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.');
            Model.Version = version[0] + "." + version[1] + "." + version[2];
        }

        /// <summary>
        /// 打开窗口命令
        /// </summary>
        /// <param name="obj"></param>
        [RelayCommand]
        private void OpenWindow(object obj)
        {
            var window = obj.ToString();
            WindowManager.CreateWindowInScreen(window);
            WindowManager.Show(window);
        }

        /// <summary>
        /// 移除进程命令
        /// </summary>
        /// <param name="obj"></param>
        [RelayCommand]
        private void RemoveBreackProcess(object obj)
        {
            Model.Data.Behavior.BreakProgressList.Remove(Model.SelectedItem);
        }

        /// <summary>
        /// 添加跳过进程命令
        /// </summary>
        /// <param name="obj"></param>
        [RelayCommand]
        private void AddBreackProcess(object obj)
        {
            if (Model.SelectedApplication == null)
            {
                Modal($"{Application.Current.Resources["Lang_Pleaseselectanapplication"]}");
                return;
            }

            var processName = Model.SelectedApplication.ProcessName;
            if (string.IsNullOrWhiteSpace(processName))
            {
                Modal($"{Application.Current.Resources["Lang_Invalidapplication"]}");
            }
            else if (Model.Data.Behavior.BreakProgressList.Contains(processName))
            {
                Modal($"{Application.Current.Resources["Lang_Applicationexists"]}");
            }
            else
            {
                Model.Data.Behavior.BreakProgressList.Add(processName);
                Model.SelectedApplication = null; // Clear selection after adding
            }
        }

        [RelayCommand]
        private void ShowWindow(object obj)
        {

            WindowManager.CreateWindowInScreen(obj.ToString());

            WindowManager.Show(obj.ToString());
        }

        [RelayCommand]
        private void Apply(object obj)
        {
            var msg = "更新失败！请尝试重启程序或删除配置文件Config.xml！";
            if (config.Save())
            {
                msg = $"{Application.Current.Resources["Lang_Optionupdated"]}";
                //处理开机启动
                if (!StartupHelper.SetStartup(config.options.General.Startup))
                {
                    msg = $"{Application.Current.Resources["Lang_Optionupdated"]}";
                }
                //处理休息间隔调整
                if (mainService.SetWarnTime(config.options.General.WarnTime))
                {
                    msg = $"{Application.Current.Resources["Lang_Optionupdated"]}";
                }
            }
            Modal(msg);
        }

        private void Modal(string text)
        {
            Model.ModalText = text;
            Model.ShowModal = true;
        }
    }
}
