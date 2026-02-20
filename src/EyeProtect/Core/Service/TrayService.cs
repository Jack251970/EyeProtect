using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using EyeProtect.Views;
using iNKORE.UI.WPF.Modern;
using iNKORE.UI.WPF.Modern.Controls;
using U5BFA.Libraries;

namespace EyeProtect.Core.Service
{
    /// <summary>
    /// 管理和显示托盘图标
    /// </summary>
    public class TrayService : IService
    {
        // Icon
#if DEBUG
        private readonly Guid notifyIconGuid = new("3C8FB3D8-F0E2-43E0-B777-16C3668C7EF3");
#else
        private readonly Guid notifyIconGuid = new("FCAE0D64-2D9C-4569-9439-5767B9C9D7AE");
#endif
        private readonly SystemTrayIcon notifyIcon;
        private readonly MainTrayIconFlyout trayIconFlyout;

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

        private IconType? lastIcon = null;

        public TrayService(
            MainService mainService,
            ConfigService config,
            BackgroundWorkerService backgroundWorker,
            ThemeService theme)
        {
            this.mainService = mainService;
            this.config = config;
            this.backgroundWorker = backgroundWorker;
            this.theme = theme;
            this.theme.OnChangedTheme += Theme_OnChangedTheme;
            mainService.OnLeaveEvent += MainService_OnLeaveEvent;
            mainService.OnStart += MainService_OnStart;
            mainService.OnLoadedLanguage += MainService_OnLoadedLanguage;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.OnCompleted += BackgroundWorker_OnCompleted;
            var iconUri = new Uri(ResourcePaths.Icons.Sunglasses, UriKind.RelativeOrAbsolute);
            var info = Application.GetResourceStream(iconUri);
            using var stream = info.Stream;
            notifyIcon = new SystemTrayIcon(notifyIconGuid, new Icon(stream), "Eye Protect", true);
            trayIconFlyout = new MainTrayIconFlyout(mainService, config);
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
                    UpdateIcon(IconType.Dizzy);
                }
                else
                {
                    UpdateIcon(IconType.Sunglasses);
                }
            }
            if (contextMenu != null && !config.options.General.Noreset)
            {
                menuItem_NoReset_OneHour.Icon = null;
                menuItem_NoReset_TwoHour.Icon = null;
                menuItem_NoReset_Forver.Icon = null;
                menuItem_NoReset_Off.Icon = new FontIcon { Glyph = "\uE915" };
            }
        }

        private void MainService_OnLeaveEvent(object service, int msg)
        {
            UpdateIcon(IconType.Sleeping);
        }

        public void Init()
        {
            CreateTrayMenu();

            notifyIcon.LeftClicked += NotifyIcon_LeftClicked;
            notifyIcon.MouseMoved += NotifyIcon_MouseMoved;
            notifyIcon.RightClicked += NotifyIcon_RightClicked;
            notifyIcon.Show();

            noresetTimer = new DispatcherTimer();
            noresetTimer.Tick += NoresetTimer_Tick;
        }

        private void NotifyIcon_LeftClicked(object sender, MouseEventReceivedEventArgs e)
        {
            if (trayIconFlyout is null)
                return;

            if (trayIconFlyout.IsOpen)
                trayIconFlyout.Hide();
            else
                trayIconFlyout.Show();
        }

        #region Events

        // 有后台工作任务在运行时
        private void BackgroundWorker_DoWork()
        {
            UpdateIcon(IconType.Overheated, false);
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

        private void menuItem_Options_Click(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                WindowManager.CreateWindowInScreen("OptionsWindow");
                WindowManager.Show("OptionsWindow");
            });
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

        public void Dispose()
        {
            notifyIcon.Hide();
            notifyIcon.Dispose();
        }

        #endregion

        #region Function

        private void CreateTrayMenu()
        {
            contextMenu = new ContextMenu();
            // 托盘菜单项
            menuItem_Options = new MenuItem
            {
                Header = Application.Current.Resources["Lang_Settings"]
            };
            menuItem_Options.Click += menuItem_Options_Click;

            menuItem_NoReset = new MenuItem
            {
                Header = Application.Current.Resources["Lang_Suspendnow"]
            };

            menuItem_NoReset_OneHour = new MenuItem
            {
                Header = Application.Current.Resources["Lang_Onehours"]
            };
            menuItem_NoReset_OneHour.Click += MenuItem_NoReset_OneHour_Click;
            menuItem_NoReset_TwoHour = new MenuItem
            {
                Header = Application.Current.Resources["Lang_Twohours"]
            };
            menuItem_NoReset_TwoHour.Click += MenuItem_NoReset_TwoHour_Click;
            menuItem_NoReset_Forver = new MenuItem
            {
                Header = Application.Current.Resources["Lang_Suspenduntilnextstartup"]
            };
            menuItem_NoReset_Forver.Click += MenuItem_NoReset_Forver_Click;
            menuItem_NoReset_Off = new MenuItem
            {
                Header = Application.Current.Resources["Lang_Disabled"],
                Icon = new FontIcon { Glyph = "\uE915" }
            };
            menuItem_NoReset_Off.Click += MenuItem_NoReset_Off_Click;

            menuItem_NoReset.Items.Add(menuItem_NoReset_OneHour);
            menuItem_NoReset.Items.Add(menuItem_NoReset_TwoHour);
            menuItem_NoReset.Items.Add(menuItem_NoReset_Forver);
            menuItem_NoReset.Items.Add(menuItem_NoReset_Off);

            menuItem_Quit = new MenuItem { Header = Application.Current.Resources["Lang_Quit"] };
            ;
            menuItem_Quit.Click += menuItem_Exit_Click;

            //添加托盘菜单项
            contextMenu.Items.Add(menuItem_Options);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(menuItem_NoReset);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(menuItem_Quit);
        }

        public void UpdateIcon(IconType? type = null, bool save = true)
        {
            type = type is null ? lastIcon : type;
            type ??= IconType.Sunglasses;
            if (notifyIcon != null && type is not null)
            {
                var iconUri = new Uri(ResourcePaths.GetIconPath(type), UriKind.RelativeOrAbsolute);
                var info = Application.GetResourceStream(iconUri);
                using var stream = info.Stream;
                notifyIcon.Icon = new Icon(stream);
                if (save)
                {
                    lastIcon = type;
                }
            }
        }

        /// <summary>
        /// 设置不提醒操作
        /// </summary>
        /// <param name="hour">-1时关闭；0打开；大于0则在到达设定的值（小时）后重新启动</param>
        public void SetNoReset(int hour)
        {
            menuItem_NoReset_OneHour.Icon = null;
            menuItem_NoReset_TwoHour.Icon = null;
            menuItem_NoReset_Forver.Icon = null;
            menuItem_NoReset_Off.Icon = null;

            if (hour == -1)
            {
                //关闭
                UpdateIcon(IconType.Sunglasses);
                config.options.General.Noreset = false;
                menuItem_NoReset_Off.Icon = new FontIcon { Glyph = "\uE915" };
                mainService.Start();

                noresetTimer.Stop();
            }
            else if (hour == 0)
            {
                //直到下次启动
                UpdateIcon(IconType.Dizzy);
                config.options.General.Noreset = true;
                menuItem_NoReset_Forver.Icon = new FontIcon { Glyph = "\uE915" };
                mainService.Pause(false);

                noresetTimer.Stop();
            }
            else
            {
                //指定计时
                UpdateIcon(IconType.Dizzy);
                config.options.General.Noreset = true;
                if (hour == 1)
                {
                    menuItem_NoReset_OneHour.Icon = new FontIcon { Glyph = "\uE915" };
                }
                else if (hour == 2)
                {
                    menuItem_NoReset_TwoHour.Icon = new FontIcon { Glyph = "\uE915" };
                }
                mainService.Pause(false);

                noresetTimer.Interval = new TimeSpan(hour, 0, 0);
                noresetTimer.Start();
            }
        }

        private void NoresetTimer_Tick(object sender, EventArgs e)
        {
            SetNoReset(-1);
            menuItem_NoReset_Off.Icon = new FontIcon { Glyph = "\uE915" };
            noresetTimer.Stop();
        }

        private void OnNoResetAction(object sender, int hour)
        {
            var item = sender as MenuItem;
            if (!item.IsChecked)
            {
                SetNoReset(hour);
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
