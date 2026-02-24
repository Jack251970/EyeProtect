using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using EyeProtect.Core;
using EyeProtect.Core.Service;
using EyeProtect.Models;
using iNKORE.UI.WPF.Modern;

namespace EyeProtect.ViewModels
{
    public partial class TipViewModel : TipModel, IViewModel
    {
        public string ScreenName { get; set; }
        public Window WindowInstance { get; set; }

        private readonly RestService reset;
        private readonly SoundService sound;
        private readonly ConfigService config;
        private readonly MainService main;
        private readonly MediaControlService mediaControl;

        public event ViewModelEventHandler ChangedEvent;

        public TipViewModel()
        {
            reset = Ioc.Default.GetRequiredService<RestService>();
            reset.TimeChanged += timeChanged;
            reset.RestCompleted += resetCompleted;

            sound = Ioc.Default.GetRequiredService<SoundService>();
            config = Ioc.Default.GetRequiredService<ConfigService>();
            config.options.General.PropertyChanged += config_Changed;
            config.options.Style.PropertyChanged += config_Changed;
            config.options.Behavior.PropertyChanged += config_Changed;

            main = Ioc.Default.GetRequiredService<MainService>();
            mediaControl = Ioc.Default.GetRequiredService<MediaControlService>();
            Ioc.Default.GetRequiredService<ThemeService>().OnChangedTheme += Theme_OnChangedTheme;
            ChangedEvent += TipViewModel_ChangedEvent;
            main.OnHandleTimeout += Main_OnHandleTimeout;
            LoadConfig();
        }

        /// <summary>
        /// 休息命令
        /// </summary>
        [RelayCommand]
        private void Reset(object obj)
        {
            main.StopBusyListener();
            CountDownVisibility = Visibility.Visible;
            TakeButtonVisibility = Visibility.Hidden;
            reset.Start();
        }

        /// <summary>
        /// 跳过命令
        /// </summary>
        [RelayCommand]
        private void Busy(object obj)
        {
            main.StopBusyListener();
            main.ReStart();
            WindowManager.Hide("TipWindow");

            // Resume media when tip window is hidden (if enabled)
            if (config.options.Behavior.IsAutoPauseMedia)
            {
                mediaControl.ResumeMedia();
            }
        }

        private void Main_OnHandleTimeout(object service, int msg)
        {
            if (config.options.Behavior.IsHandleTimeoutRest)
            {
                // 进入休息状态
                Reset(null);
            }
            else
            {
                //关闭窗口
                WindowManager.Hide("TipWindow");

                //进入离开状态
                main.OnLeave();
            }
        }

        private void Theme_OnChangedTheme(ApplicationTheme oldThemeName, ApplicationTheme newThemeName)
        {
            UpdateUIData();
        }

        private void TipViewModel_ChangedEvent()
        {
            UpdateUIData();
            // Subscribe to IsVisibleChanged to handle window show/hide events
            WindowInstance.IsVisibleChanged -= WindowInstance_IsVisibleChanged;
            WindowInstance.IsVisibleChanged += WindowInstance_IsVisibleChanged;
        }

        private void WindowInstance_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (WindowInstance.IsVisible)
            {
                UpdateVariable();
                UpdateUIData();
                HandleAutoAction();
                WindowInstance.Focus();
            }
        }

        private void UpdateVariable()
        {
            try
            {
                Countdown = config.options.General.RestTime;
                //提醒间隔变量
                T = config.options.General.WarnTime.ToString();
                //当前时间
                Time = DateTime.Now.ToString();
                //年
                Y = DateTime.Now.ToString("yyyy");
                //月
                M = DateTime.Now.ToString("MM");
                //日
                D = DateTime.Now.ToString("dd");
                //时
                H = DateTime.Now.ToString("HH");
                //分
                Minutes = DateTime.Now.ToString("mm");
                //今日用眼时长
                Twt = "0";
                //今日休息时长
                Trt = "0";
                //今日跳过次数
                Tsc = "0";
            }
            catch (Exception ex)
            {
                LogHelper.Error("UpdateVariable Fail,Exception message:" + ex.Message);
            }
        }
        private void UpdateUIData()
        {
            var isDarkTheme = ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark;
            
            // Set container background and opacity based on theme
            ContainerBackground = isDarkTheme ? new SolidColorBrush(Color.FromRgb(0x1A, 0x1B, 0x1C)) : Brushes.White;
            ContainerOpacity = 0.98;
            
            // Set tip image based on theme
            var imagePath = ResourcePaths.GetTipImagePackUri(isDarkTheme);
            try
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
                image.EndInit();
                TipImageSource = image;
            }
            catch (Exception e)
            {
                LogHelper.Error("Failed to load tip image: " + e.Message);
            }
            
            // Set tip content with variable replacement
            var tipText = config.options.Style.TipContent ?? "You have been using your eyes for {t} minutes. Take a break! Please focus your attention at least 6 meters away for 20 seconds!";
            TipContent = ReplaceVariables(tipText);
        }
        
        private string ReplaceVariables(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
                
            text = text.Replace("{t}", T);
            text = text.Replace("{time}", Time);
            text = text.Replace("{y}", Y);
            text = text.Replace("{m}", M);
            text = text.Replace("{d}", D);
            text = text.Replace("{h}", H);
            text = text.Replace("{minutes}", Minutes);
            text = text.Replace("{twt}", Twt);
            text = text.Replace("{trt}", Trt);
            text = text.Replace("{tsc}", Tsc);
            text = text.Replace("{countdown}", Countdown.ToString());
            
            return text;
        }


        //加载配置
        private void LoadConfig()
        {
            // Update UI data when config changes
            if (WindowInstance != null)
            {
                UpdateUIData();
            }
        }

        //配置文件被修改时
        private void config_Changed(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(config.options.General.RestTime):
                case nameof(config.options.General.WarnTime):
                case nameof(config.options.Style.TipContent):
                    LoadConfig();
                    break;
            }
        }

        private void resetCompleted(object sender, int timed)
        {
            //休息结束
            Init();
            //播放提示音
            if (config.options.General.Sound)
            {
                sound.Play();
            }
            //重启计时
            main.ReStart();
        }

        private void Init()
        {
            Countdown = 20;
            CountDownVisibility = Visibility.Hidden;
            TakeButtonVisibility = Visibility.Visible;
        }

        private void timeChanged(object sender, int timed)
        {
            Countdown = timed;
        }

        /// <summary>
        /// 处理自动操作
        /// </summary>
        private void HandleAutoAction()
        {
            //禁用跳过休息
            if (config.options.Behavior.IsDisabledSkip)
            {
                //进入休息
                Reset(null);
            }
        }

        public void OnChanged()
        {
            ChangedEvent?.Invoke();
        }
    }
}
