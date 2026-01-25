using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using Project1.UI.Controls.Models;
using Project1.UI.Cores;
using ProjectEye.Models.UI;

namespace ProjectEye.Core.Service
{
    public class ThemeService : IService
    {
        private readonly ConfigService config;
        private readonly SystemResourcesService systemResources;


        public delegate void ThemeChangedEventHandler(string OldThemeName, string NewThemeName);
        /// <summary>
        /// 当切换主题时发生
        /// </summary>
        public event ThemeChangedEventHandler OnChangedTheme;
        public ThemeService(ConfigService config,
            SystemResourcesService systemResources)
        {
            this.config = config;
            this.systemResources = systemResources;
        }
        public void Init()
        {
            // iNKORE.UI.WPF.Modern will follow system theme by default (ApplicationTheme = null)
            // No manual theme setting needed
        }
        
        /// <summary>
        /// 设置主题 - Deprecated: Now follows system theme
        /// </summary>
        /// <param name="themeName"></param>
        public void SetTheme(string themeName)
        {
            // Theme setting removed - application now follows system theme
            // iNKORE.UI.WPF.Modern.ThemeManager.Current.ApplicationTheme is null by default
        }

        /// <summary>
        /// 创建默认的提示界面布局UI
        /// </summary>
        /// <param name="themeName">主题名</param>
        /// <param name="screenName">屏幕名称</param>
        /// <returns></returns>
        public UIDesignModel GetCreateDefaultTipWindowUI(
            string themeName,
            string screenName)
        {
            screenName = screenName.Replace("\\", "");

            var screen = System.Windows.Forms.Screen.PrimaryScreen;
            if (screenName != string.Empty)
            {
                foreach (var item in System.Windows.Forms.Screen.AllScreens)
                {
                    var itemScreenName = item.DeviceName.Replace("\\", "");
                    if (itemScreenName == screenName)
                    {
                        screen = item;
                        break;
                    }
                }
            }

            var screenSize = WindowManager.GetSize(screen);

            //创建默认布局
            var data = new UIDesignModel();
            data.ContainerAttr = new ContainerModel()
            {
                Background = Brushes.White,
                Opacity = .98
            };

            var elements = new List<ElementModel>();
            var tipimage = new ElementModel();
            tipimage.Type = Project1.UI.Controls.Enums.DesignItemType.Image;
            tipimage.Width = 272;
            tipimage.Opacity = 1;
            tipimage.Height = 187;
            tipimage.Image = $"pack://application:,,,/ProjectEye;component/Resources/Themes/{themeName}/Images/tipImage.png";
            tipimage.X = screenSize.Width / 2 - tipimage.Width / 2;
            tipimage.Y = screenSize.Height * .24;

            var tipText = new ElementModel();
            tipText.Type = Project1.UI.Controls.Enums.DesignItemType.Text;
            tipText.Text = config.options.Style.TipContent ?? "You have been using your eyes for {t} minutes. Take a break! Please focus your attention at least 6 meters away for 20 seconds!";
            tipText.Opacity = 1;
            tipText.TextColor = Project1UIColor.Get("#45435b");
            tipText.Width = 400;
            tipText.Height = 120;
            tipText.X = screenSize.Width / 2 - tipText.Width / 2;
            tipText.Y = tipimage.Y + tipimage.Height + 10;
            tipText.FontSize = 20;

            var restBtn = new ElementModel();
            restBtn.Type = Project1.UI.Controls.Enums.DesignItemType.Button;
            restBtn.Width = 110;
            restBtn.Height = 45;
            restBtn.FontSize = 14;
            restBtn.Text = "好的";
            restBtn.Opacity = 1;
            restBtn.Command = "rest";

            restBtn.X = screenSize.Width / 2 - (restBtn.Width * 2 + 10) / 2;
            restBtn.Y = tipText.Y + tipText.Height + 20;

            var breakBtn = new ElementModel();
            breakBtn.Type = Project1.UI.Controls.Enums.DesignItemType.Button;
            breakBtn.Width = 110;
            breakBtn.Height = 45;
            breakBtn.FontSize = 14;
            breakBtn.Text = "暂时不";
            breakBtn.Style = "basic";
            breakBtn.Command = "break";
            breakBtn.Opacity = 1;
            breakBtn.X = screenSize.Width / 2 - (restBtn.Width * 2 + 10) / 2 + (restBtn.Width + 10);
            breakBtn.Y = tipText.Y + tipText.Height + 20;

            var countDownText = new ElementModel();
            countDownText.Text = "{countdown}";
            countDownText.FontSize = 50;
            countDownText.IsTextBold = true;
            countDownText.Type = Project1.UI.Controls.Enums.DesignItemType.Text;
            countDownText.TextColor = Brushes.Black;
            countDownText.Opacity = 1;
            countDownText.Width = 100;
            countDownText.Height = 60;
            countDownText.X = screenSize.Width / 2 - countDownText.Width / 2;
            countDownText.Y = restBtn.Y + restBtn.Height;



            if (themeName == "Dark")
            {
                //深色主题的样式

                data.ContainerAttr.Background = Project1UIColor.Get("#1A1B1C");
                tipText.TextColor = Project1UIColor.Get("#D9D9D9");
                countDownText.TextColor = Project1UIColor.Get("#D9D9D9");

            }
            elements.Add(tipimage);
            elements.Add(tipText);
            elements.Add(restBtn);
            elements.Add(breakBtn);
            elements.Add(countDownText);


            data.Elements = elements;

            return data;
        }
    }
}
