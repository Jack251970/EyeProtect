using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using iNKORE.UI.WPF.Modern;
using ProjectEye.Models;

namespace ProjectEye.Core.Service
{
    /// <summary>
    /// 管理和显示托盘图标
    /// </summary>
    public class TrayService : IService
    {
        // Icon
#if DEBUG
        private readonly Guid notifyIconGuid = new("A80B9DBC-DCE9-4F07-87C9-AE161869878C");
#else
        private readonly Guid notifyIconGuid = new("F995B3D0-0F4B-415C-B343-F592296F2749");
#endif
        private readonly SystemTrayIcon notifyIcon;

        // Service
        private readonly MainService mainService;
        private readonly ConfigService config;
        private readonly BackgroundWorkerService backgroundWorker;
        private readonly ThemeService theme;

        // Menu
        private ContextMenu contextMenu;
        private MenuItem menuItem_NoReset;
        private MenuItem menuItem_Options;
        private MenuItem menuItem_Quit;

        private MenuItem menuItem_NoReset_OneHour;
        private MenuItem menuItem_NoReset_TwoHour;
        private MenuItem menuItem_NoReset_Forver;
        private MenuItem menuItem_NoReset_Off;

        private DispatcherTimer noresetTimer;

        private string lastIcon = string.Empty;

        public TrayService(
            App app,
            MainService mainService,
            ConfigService config,
            BackgroundWorkerService backgroundWorker,
            ThemeService theme)
        {
            this.mainService = mainService;
            this.config = config;
            this.backgroundWorker = backgroundWorker;
            this.theme = theme;
            this.config.Changed += new EventHandler(config_Changed);
            this.theme.OnChangedTheme += Theme_OnChangedTheme;
            app.Exit += new ExitEventHandler(app_Exit);
            mainService.OnLeaveEvent += MainService_OnLeaveEvent;
            mainService.OnStart += MainService_OnStart;
            mainService.OnLoadedLanguage += MainService_OnLoadedLanguage;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.OnCompleted += BackgroundWorker_OnCompleted;
            var iconUri = new Uri("/ProjectEye;component/Resources/sunglasses.ico", UriKind.RelativeOrAbsolute);
            var info = Application.GetResourceStream(iconUri);
            using var stream = info.Stream;
            notifyIcon = new SystemTrayIcon(new Icon(stream), "Eye Protect", notifyIconGuid, true);
        }

        private void MainService_OnLoadedLanguage(object service, int msg)
        {
            CreateTrayMenu();
        }

        private void Theme_OnChangedTheme(ApplicationTheme oldTheme, ApplicationTheme newTheme)
        {
            CreateTrayMenu();
        }

        private void MainService_OnStart(object service, int msg)
        {
            if (!backgroundWorker.IsBusy)
            {
                if (config.options.General.Noreset)
                {
                    UpdateIcon("dizzy");
                }
                else
                {
                    UpdateIcon("sunglasses");
                }
            }
            if (contextMenu != null && !config.options.General.Noreset)
            {
                menuItem_NoReset_OneHour.IsChecked = false;
                menuItem_NoReset_TwoHour.IsChecked = false;
                menuItem_NoReset_Forver.IsChecked = false;
                menuItem_NoReset.IsChecked = false;
                menuItem_NoReset_Off.IsChecked = true;
            }
        }

        private void MainService_OnLeaveEvent(object service, int msg)
        {
            UpdateIcon("sleeping");
        }

        public void Init()
        {
            CreateTrayMenu();

            notifyIcon.IsVisible = true;
            notifyIcon.MouseMoved += NotifyIcon_MouseMoved;
            notifyIcon.RightClicked += NotifyIcon_RightClicked;

            noresetTimer = new DispatcherTimer();
        }

        #region Events

        // 有后台工作任务在运行时
        private void BackgroundWorker_DoWork()
        {
            UpdateIcon("overheated", false);
            SetText($"Eye Protect: {Application.Current.Resources["Lang_TimeconsumingOperation"]}");
        }

        // 后台工作任务运行结束时
        private void BackgroundWorker_OnCompleted()
        {
            SetText("Eye Protect");
            UpdateIcon();
        }

        private void MenuItem_NoReset_Off_Click(object sender, RoutedEventArgs e)
        {
            OnNoResetAction(sender, -1);
        }

        private void MenuItem_NoReset_Forver_Click(object sender, RoutedEventArgs e)
        {
            OnNoResetAction(sender, 0);
        }

        private void MenuItem_NoReset_TwoHour_Click(object sender, RoutedEventArgs e)
        {
            OnNoResetAction(sender, 2);
        }

        private void MenuItem_NoReset_OneHour_Click(object sender, RoutedEventArgs e)
        {
            OnNoResetAction(sender, 1);
        }

        private void config_Changed(object sender, EventArgs e)
        {
            menuItem_NoReset.IsChecked = config.options.General.Noreset;
        }

        private void menuItem_Options_Click(object sender, EventArgs e)
        {
            WindowManager.CreateWindowInScreen("OptionsWindow");
            WindowManager.Show("OptionsWindow");
        }

        private void NotifyIcon_MouseMoved(object sender, MouseEventReceivedEventArgs e)
        {
            if (mainService.IsWorkTimerRun() && !backgroundWorker.IsBusy)
            {
                var restCT = mainService.GetRestCountdownMinutes();
                var restStr = Math.Round(restCT, 1) + $"{Application.Current.Resources["Lang_Minutes_n"]}";
                if (restCT < 1)
                {
                    restStr = Math.Round((restCT * 60), 0).ToString() + $"{Application.Current.Resources["Lang_Seconds_n"]}";
                }
                if (restCT > 60)
                {
                    restCT = Math.Round(restCT / 60, 1);

                    restStr = $"{restCT}{Application.Current.Resources["Lang_Hours_n"]}";
                    if (restCT.ToString().IndexOf(".") != -1)
                    {
                        restStr = $"{restCT.ToString().Split('.')[0]}{Application.Current.Resources["Lang_Hours_n"]} {restCT.ToString().Split('.')[1]}{Application.Current.Resources["Lang_Minutes_n"]}";
                    }
                }

                SetText($"Eye Protect\r\n{Application.Current.Resources["Lang_Thenextbreak"]}: " + restStr);
            }
            else if (config.options.General.Noreset)
            {
                SetText($"Eye Protect: {Application.Current.Resources["Lang_Reminderisoff"]}");
            }
            else if (!backgroundWorker.IsBusy)
            {
                SetText("Eye Protect");
            }
        }

        private void NotifyIcon_RightClicked(object sender, MouseEventReceivedEventArgs e)
        {
            if (backgroundWorker.IsBusy)
            {
                return;
            }

            contextMenu.IsOpen = true;
        }

        private void menuItem_Exit_Click(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void app_Exit(object sender, ExitEventArgs e)
        {
            mainService.Exit();
            notifyIcon?.Destroy();
        }

        #endregion

        #region Function

        private void CreateTrayMenu()
        {
            contextMenu = new ContextMenu();
            // 托盘菜单项
            menuItem_Options = new MenuItem();
            menuItem_Options.Header = Application.Current.Resources["Lang_Settings"];
            menuItem_Options.Click += menuItem_Options_Click;

            menuItem_NoReset = new MenuItem();
            menuItem_NoReset.Header = Application.Current.Resources["Lang_Suspendnow"];

            menuItem_NoReset_OneHour = new MenuItem();
            menuItem_NoReset_OneHour.Header = Application.Current.Resources["Lang_Onehours"];
            menuItem_NoReset_OneHour.Click += MenuItem_NoReset_OneHour_Click;
            menuItem_NoReset_TwoHour = new MenuItem();
            menuItem_NoReset_TwoHour.Header = Application.Current.Resources["Lang_Twohours"];
            menuItem_NoReset_TwoHour.Click += MenuItem_NoReset_TwoHour_Click;
            menuItem_NoReset_Forver = new MenuItem();
            menuItem_NoReset_Forver.Header = Application.Current.Resources["Lang_Suspenduntilnextstartup"];
            menuItem_NoReset_Forver.Click += MenuItem_NoReset_Forver_Click;
            menuItem_NoReset_Off = new MenuItem();
            menuItem_NoReset_Off.Header = Application.Current.Resources["Lang_Disabled"];
            menuItem_NoReset_Off.IsChecked = true;
            menuItem_NoReset_Off.Click += MenuItem_NoReset_Off_Click;

            menuItem_NoReset.Items.Add(menuItem_NoReset_OneHour);
            menuItem_NoReset.Items.Add(menuItem_NoReset_TwoHour);
            menuItem_NoReset.Items.Add(menuItem_NoReset_Forver);
            menuItem_NoReset.Items.Add(menuItem_NoReset_Off);

            menuItem_Quit = new MenuItem();
            menuItem_Quit.Header = Application.Current.Resources["Lang_Quit"]; ;
            menuItem_Quit.Click += menuItem_Exit_Click;

            //添加托盘菜单项
            contextMenu.Items.Add(menuItem_Options);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(menuItem_NoReset);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(menuItem_Quit);
        }

        public void UpdateIcon(string name = "", bool save = true)
        {
            name = name == "" ? lastIcon : name;
            if (name == "")
            {
                name = "sunglasses";
            }
            if (notifyIcon != null && name != "")
            {
                var iconUri = new Uri("/ProjectEye;component/Resources/" + name + ".ico", UriKind.RelativeOrAbsolute);
                var info = Application.GetResourceStream(iconUri);
                using var stream = info.Stream;
                notifyIcon.Icon = new Icon(stream);
                if (save)
                {
                    lastIcon = name;
                }
            }
        }

        /// <summary>
        /// 设置不提醒操作
        /// </summary>
        /// <param name="hour">-1时关闭；0打开；大于0则在到达设定的值（小时）后重新启动</param>
        private void SetNoReset(int hour)
        {
            config.options.General.Noreset = true;
            menuItem_NoReset_OneHour.IsChecked = false;
            menuItem_NoReset_TwoHour.IsChecked = false;
            menuItem_NoReset_Forver.IsChecked = false;
            menuItem_NoReset_Off.IsChecked = false;
            menuItem_NoReset.IsChecked = true;
            noresetTimer.Stop();
            UpdateIcon("dizzy");
            if (hour == -1)
            {
                //关闭
                config.options.General.Noreset = false;
                menuItem_NoReset.IsChecked = false;
                mainService.Start();
                UpdateIcon("sunglasses");

            }
            else if (hour == 0)
            {
                //直到下次启动
                menuItem_NoReset.IsChecked = true;
                mainService.Pause(false);
            }
            else
            {
                //指定计时
                menuItem_NoReset.IsChecked = true;
                mainService.Pause(false);

                noresetTimer.Interval = new TimeSpan(hour, 0, 0);
                noresetTimer.Tick += (e, c) =>
                {
                    SetNoReset(-1);
                    menuItem_NoReset_Off.IsChecked = true;
                    noresetTimer.Stop();
                };
                noresetTimer.Start();
            }
        }
        private void OnNoResetAction(object sender, int hour)
        {
            var item = sender as MenuItem;
            if (!item.IsChecked)
            {
                SetNoReset(hour);
                item.IsChecked = true;
            }
        }

        /// <summary>
        /// 设置托盘图标文本
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            notifyIcon.Tooltip = text.Length > 63 ? text[..63] : text;
        }

        #endregion
    }
}
