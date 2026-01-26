using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using iNKORE.UI.WPF.Modern;
using ProjectEye.Core;
using ProjectEye.Core.Service;
using ProjectEye.Models;

namespace ProjectEye.ViewModels
{
    public partial class TipViewModel : TipModel, IViewModel
    {
        public string ScreenName { get; set; }
        public Window WindowInstance { get; set; }

        private readonly RestService reset;
        private readonly SoundService sound;
        private readonly ConfigService config;
        private readonly MainService main;

        public event ViewModelEventHandler ChangedEvent;

        public TipViewModel(RestService reset,
            SoundService sound,
            ConfigService config,
            MainService main,
            ThemeService theme)
        {
            this.reset = reset;
            this.reset.TimeChanged += new RestEventHandler(timeChanged);
            this.reset.RestCompleted += new RestEventHandler(resetCompleted);

            this.sound = sound;
            this.config = config;
            this.config.Changed += config_Changed;

            this.main = main;
            theme.OnChangedTheme += Theme_OnChangedTheme;
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
        }

        private void Main_OnHandleTimeout(object service, int msg)
        {
            if (config.options.Behavior.IsHandleTimeoutRest)
            {
                //  进入休息状态
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
            WindowInstance.IsVisibleChanged += WindowInstance_IsVisibleChanged;
        }

        private void WindowInstance_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // When window becomes visible, trigger the OnWShow logic
            if (WindowInstance.IsVisible)
            {
                UpdateVariable();
                UpdateUIData();
                if (!config.options.Style.IsThruTipWindow)
                {
                    WindowInstance.Focus();
                }
                HandleAutoAction();
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
            var imagePath = $"pack://application:,,,/ProjectEye;component/Resources/Images/{(isDarkTheme ? "Dark" : "Light")}/tipImage.png";
            try
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
                image.EndInit();
                TipImageSource = image;
            }
            catch
            {
                TipImageSource = BitmapImager.Load("pack://application:,,,/ProjectEye;component/Resources/Images/sunglasses.png");
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
            //鼠标穿透
            IsThruWindow = config.options.Style.IsThruTipWindow;
            
            // Update UI data when config changes
            if (WindowInstance != null)
            {
                UpdateUIData();
            }
        }

        //配置文件被修改时
        private void config_Changed(object sender, EventArgs e)
        {
            LoadConfig();
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
            //询问
            if (!config.options.Style.IsTipAsk)
            {
                //进入休息
                Reset(null);
                return;
            }
            //禁用跳过休息
            if (config.options.Behavior.IsDisabledSkip)
            {
                //进入休息
                Reset(null);
                return;
            }
        }

        public void OnChanged()
        {
            ChangedEvent?.Invoke();
        }
    }
}
