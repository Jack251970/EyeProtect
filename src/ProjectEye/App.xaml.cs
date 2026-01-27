using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using iNKORE.UI.WPF.Modern.Common;
using ProjectEye.Core;
using ProjectEye.Core.Service;
using ProjectEye.Views;

namespace ProjectEye
{
    public partial class App : Application, IDisposable, ISingleInstanceApp
    {
        private static bool _disposed;

        // To prevent two disposals running at the same time.
        private static readonly Lock _disposingLock = new();

        private readonly ServiceCollection serviceCollection;

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance())
            {
                using var application = new App();
                application.InitializeComponent();
                application.Run();
            }
        }

        public App()
        {
            // Do not use bitmap cache since it can cause WPF second window freezing issue
            ShadowAssist.UseBitmapCache = false;

            serviceCollection = new ServiceCollection();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 退出事件
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                Dispose();
            };
            Current.Exit += (s, e) =>
            {
                Dispose();
            };
            Current.SessionEnding += (s, e) =>
            {
                Dispose();
            };

            // 全局异常捕获
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // 必须按优先级依次添加
            serviceCollection.AddInstance(this);
            // 后台任务
            serviceCollection.Add<BackgroundWorkerService>();
            // 系统资源
            serviceCollection.Add<SystemResourcesService>();
            // 内存缓存
            serviceCollection.Add<CacheService>();
            // 配置文件
            serviceCollection.Add<ConfigService>();
            // 通知
            serviceCollection.Add<NotificationService>();
            // 主题
            serviceCollection.Add<ThemeService>();
            // 扩展显示器
            serviceCollection.Add<ScreenService>();
            // 主要
            serviceCollection.Add<MainService>();
            // 托盘
            serviceCollection.Add<TrayService>();
            // 休息
            serviceCollection.Add<RestService>();
            // 声音
            serviceCollection.Add<SoundService>();

            WindowManager.serviceCollection = serviceCollection;

            //初始化所有服务
            serviceCollection.Initialize();

            // 检查开机自启错误
            AutoStartup();

            LogHelper.Info("App Start");
        }

        [Conditional("RELEASE")]
        private static void AutoStartup()
        {
            _ = StartupHelper.CheckStartup();
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogHelper.Error(e.Exception.ToString());

            // Show error report window instead of launching external process
            var errorWindow = new ErrorReportWindow();
            errorWindow.Show();

            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogHelper.Error(e.ExceptionObject.ToString());

            // Show error report window instead of launching external process
            var errorWindow = new ErrorReportWindow();
            errorWindow.Show();
        }

        protected virtual void Dispose(bool disposing)
        {
            // Prevent two disposes at the same time.
            lock (_disposingLock)
            {
                if (!disposing)
                {
                    return;
                }

                if (_disposed)
                {
                    return;
                }

                _disposed = true;
            }

            LogHelper.Info("App Exit");
            if (disposing)
            {
                // Dispose needs to be called on the main Windows thread,
                // since some resources owned by the thread need to be disposed.
                ((MainService)serviceCollection.GetInstance(typeof(MainService).FullName)).Exit();
                ((TrayService)serviceCollection.GetInstance(typeof(TrayService).FullName)).Exit();
                ((NotificationService)serviceCollection.GetInstance(typeof(NotificationService).FullName)).Exit();
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void OnSecondAppStarted()
        {
            Current.Dispatcher.Invoke(() =>
            {
                WindowManager.CreateWindowInScreen("OptionsWindow");
                WindowManager.Show("OptionsWindow");
            });
        }
    }
}
