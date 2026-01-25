using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using iNKORE.UI.WPF.Modern;
using ProjectEye.Core;
using ProjectEye.Core.Service;
using ProjectEye.Models;

namespace ProjectEye.ViewModels
{
    public class TipViewModel : TipModel, IViewModel
    {
        public string ScreenName { get; set; }
        public Window WindowInstance { get; set; }

        /// <summary>
        /// 休息命令
        /// </summary>
        public Command resetCommand { get; set; }
        /// <summary>
        /// 跳过命令
        /// </summary>
        public Command busyCommand { get; set; }

        private readonly RestService reset;
        private readonly SoundService sound;
        private readonly ConfigService config;
        private readonly MainService main;

        public event ViewModelEventHandler ChangedEvent;

        public TipViewModel(RestService reset,
            SoundService sound,
            ConfigService config,
            MainService main,
            App app,
            ThemeService theme)
        {
            this.reset = reset;
            this.reset.TimeChanged += new RestEventHandler(timeChanged);
            this.reset.RestCompleted += new RestEventHandler(resetCompleted);

            this.sound = sound;
            this.config = config;
            this.config.Changed += config_Changed;


            resetCommand = new Command(new Action<object>(resetCommand_action));
            busyCommand = new Command(new Action<object>(busyCommand_action));

            this.main = main;
            theme.OnChangedTheme += Theme_OnChangedTheme;
            ChangedEvent += TipViewModel_ChangedEvent;
            main.OnHandleTimeout += Main_OnHandleTimeout;
            LoadConfig();
        }

        private void Main_OnHandleTimeout(object service, int msg)
        {
            if (config.options.Behavior.IsHandleTimeoutRest)
            {
                //  进入休息状态
                resetCommand_action(null);
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
            WindowInstance.SizeChanged += WindowInstance_SizeChanged;
            // Subscribe to IsVisibleChanged to handle window show/hide events
            WindowInstance.IsVisibleChanged += WindowInstance_IsVisibleChanged;
        }

        private void WindowInstance_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // When window becomes visible, trigger the OnWShow logic
            if (WindowInstance.IsVisible)
            {
                TipViewModel_OnWShow(sender, EventArgs.Empty);
            }
        }

        private void WindowInstance_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // No need to recreate UI anymore since it's defined in XAML
            // Size changes are handled automatically by layout system
        }

        private void TipViewModel_OnWShow(object sender, EventArgs e)
        {
            UpdateVariable();
            if (!config.options.Style.IsThruTipWindow)
            {
                WindowInstance.Focus();
            }
            HandleAutoAction();
        }

        private void WindowInstance_Activated(object sender, EventArgs e)
        {
            //UpdateVariable();
            //var window = sender as Window;
            //window.Focus();
            //HandleAutoAction();
        }

        private void UpdateVariable()
        {
            try
            {
                COUNTDOWN = config.options.General.RestTime;
                //提醒间隔变量
                T = config.options.General.WarnTime.ToString();
                //当前时间
                TIME = DateTime.Now.ToString();
                //年
                Y = DateTime.Now.ToString("yyyy");
                //月
                M = DateTime.Now.ToString("MM");
                //日
                D = DateTime.Now.ToString("dd");
                //时
                H = DateTime.Now.ToString("HH");
                //分
                MINUTES = DateTime.Now.ToString("mm");
                //今日用眼时长
                TWT = "0";
                //今日休息时长
                TRT = "0";
                //今日跳过次数
                TSC = "0";
            }
            catch (Exception ex)
            {
                LogHelper.Error("UpdateVariable Fail,Exception message:" + ex.Message);
            }
        }
        private void UpdateUIData()
        {
            var isDarkTheme = iNKORE.UI.WPF.Modern.ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark;
            
            // Set container background and opacity based on theme
            ContainerBackground = isDarkTheme ? new SolidColorBrush(Color.FromRgb(0x1A, 0x1B, 0x1C)) : Brushes.White;
            ContainerOpacity = 0.98;
            
            // Set tip image based on theme
            var imagePath = $"pack://application:,,,/ProjectEye;component/Resources/Images/{(isDarkTheme ? "Dark" : "Light")}/Images/tipImage.png";
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
            text = text.Replace("{time}", TIME);
            text = text.Replace("{y}", Y);
            text = text.Replace("{m}", M);
            text = text.Replace("{d}", D);
            text = text.Replace("{h}", H);
            text = text.Replace("{minutes}", MINUTES);
            text = text.Replace("{twt}", TWT);
            text = text.Replace("{trt}", TRT);
            text = text.Replace("{tsc}", TSC);
            text = text.Replace("{countdown}", COUNTDOWN.ToString());
            
            return text;
        }


        //加载配置
        private void LoadConfig()
        {
            //动画开关
            IsAnimation = config.options.Style.IsAnimation;

            //动画类型
            WindowAnimationType = config.options.Style.TipWindowAnimation.AnimationType;

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
            COUNTDOWN = 20;
            CountDownVisibility = System.Windows.Visibility.Hidden;
            TakeButtonVisibility = System.Windows.Visibility.Visible;
        }

        private void resetCommand_action(object obj)
        {
            main.StopBusyListener();
            CountDownVisibility = System.Windows.Visibility.Visible;
            TakeButtonVisibility = System.Windows.Visibility.Hidden;
            reset.Start();
        }
        private void busyCommand_action(object obj)
        {
            main.StopBusyListener();
            main.ReStart();
            WindowManager.Hide("TipWindow");
        }
        private void timeChanged(object sender, int timed)
        {
            COUNTDOWN = timed;

        }

        ///// <summary>
        ///// 窗口监听
        ///// </summary>
        //private void WindowsListener()
        //{
        //    var windows = WindowManager.GetWindows("TipWindow");
        //    foreach (var window in windows)
        //    {
        //        window.Activated += Window_Activated;
        //        window.KeyDown += Window_KeyDown;
        //    }
        //}




        /// <summary>
        /// 处理自动操作
        /// </summary>
        private void HandleAutoAction()
        {
            //询问
            if (!config.options.Style.IsTipAsk)
            {
                //进入休息
                resetCommand_action(null);
                return;
            }
            //禁用跳过休息
            if (config.options.Behavior.IsDisabledSkip)
            {
                //进入休息
                resetCommand_action(null);
                return;
            }

        }

        public void OnChanged()
        {
            ChangedEvent?.Invoke();
        }
    }
}
