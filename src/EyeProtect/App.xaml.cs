using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using iNKORE.UI.WPF.Modern.Common;
using EyeProtect.Core;
using EyeProtect.Core.Service;
using EyeProtect.Views;

namespace EyeProtect
{
    public partial class App : Application, IDisposable, ISingleInstanceApp
    {
        private static bool _disposed;

        // To prevent two disposals running at the same time.
        private static readonly Lock _disposingLock = new();

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

            // Configure IoC container
            Ioc.Default.ConfigureServices(ConfigureServices());
        }

        private ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Register App instance
            services.AddSingleton(this);

            // Register services in order of priority
            services.AddSingleton<BackgroundWorkerService>();
            services.AddSingleton<SystemResourcesService>();
            services.AddSingleton<CacheService>();
            services.AddSingleton<ConfigService>();
            services.AddSingleton<NotificationService>();
            services.AddSingleton<ThemeService>();
            services.AddSingleton<ScreenService>();
            services.AddSingleton<MainService>();
            services.AddSingleton<TrayService>();
            services.AddSingleton<RestService>();
            services.AddSingleton<SoundService>();

            return services.BuildServiceProvider();
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

            // Initialize all services
            InitializeServices();

            // 检查开机自启错误
            AutoStartup();

            LogHelper.Info("App Start");
        }

        private static void InitializeServices()
        {
            // Initialize services in registration order
            Ioc.Default.GetService<BackgroundWorkerService>()?.Init();
            Ioc.Default.GetService<SystemResourcesService>()?.Init();
            Ioc.Default.GetService<CacheService>()?.Init();
            Ioc.Default.GetService<ConfigService>()?.Init();
            Ioc.Default.GetService<NotificationService>()?.Init();
            Ioc.Default.GetService<ThemeService>()?.Init();
            Ioc.Default.GetService<ScreenService>()?.Init();
            Ioc.Default.GetService<MainService>()?.Init();
            Ioc.Default.GetService<TrayService>()?.Init();
            Ioc.Default.GetService<RestService>()?.Init();
            Ioc.Default.GetService<SoundService>()?.Init();
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
                Ioc.Default.GetService<MainService>()?.Exit();
                Ioc.Default.GetService<TrayService>()?.Exit();
                Ioc.Default.GetService<NotificationService>()?.Exit();
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
