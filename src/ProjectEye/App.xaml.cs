using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using ProjectEye.Core;
using ProjectEye.Core.Service;
using ProjectEye.Views;

namespace ProjectEye
{
    public partial class App : Application, ISingleInstanceApp
    {
        private readonly ServiceCollection serviceCollection;

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance())
            {
                var application = new App();
                application.InitializeComponent();
                application.Run();
            }
        }

        public App()
        {
            serviceCollection = new ServiceCollection();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //全局异常捕获
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            //必须按优先级依次添加
            serviceCollection.AddInstance(this);
            //后台任务
            serviceCollection.Add<BackgroundWorkerService>();
            //系统资源
            serviceCollection.Add<SystemResourcesService>();
            //内存缓存
            serviceCollection.Add<CacheService>();
            //配置文件
            serviceCollection.Add<ConfigService>();
            //通知
            serviceCollection.Add<NotificationService>();
            //主题
            serviceCollection.Add<ThemeService>();
            //扩展显示器
            serviceCollection.Add<ScreenService>();
            //主要
            serviceCollection.Add<MainService>();
            //托盘
            serviceCollection.Add<TrayService>();
            //休息
            serviceCollection.Add<RestService>();
            //声音
            serviceCollection.Add<SoundService>();

            WindowManager.serviceCollection = serviceCollection;
            //初始化所有服务
            serviceCollection.Initialize();

            // 检查开机自启错误
            AutoStartup();
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
