using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Windows.Win32;

namespace EyeProtect.Core.Service
{
    /// <summary>
    /// 屏幕监听服务
    /// 用于处理插拔显示器时创建和刷新提示窗口
    /// </summary>
    public class ScreenService : IService
    {
        private readonly DispatcherTimer timer;
        private HwndSource source;
        private readonly HwndSourceHook hwndSourceHook;
        public ScreenService()
        {
            hwndSourceHook = new HwndSourceHook(WndProc);

            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 3);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            WindowManager.UpdateAllScreensWindow("TipWindow", true);
        }

        public void Init()
        {
            //创建一个隐藏的窗口，用于接收显示器拔插消息
            var hookWindow = new Window
            {
                Width = 0,
                Height = 0,
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.None,
                //hookWindow.WindowState = WindowState.Minimized;
                Visibility = Visibility.Hidden
            };
            hookWindow.SourceInitialized += new EventHandler(hookWindow_SourceInitialized);
            hookWindow.Show();
        }

        public void Dispose()
        {
            if (source != null)
            {
                source.RemoveHook(hwndSourceHook);
                source.Dispose();
            }
        }

        private void hookWindow_SourceInitialized(object sender, EventArgs e)
        {
            source = HwndSource.FromHwnd(new WindowInteropHelper((Window)sender).Handle);
            source.AddHook(hwndSourceHook);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case (int)PInvoke.WM_DISPLAYCHANGE:
                    timer.Start();
                    break;
            }
            return IntPtr.Zero;
        }
    }
}
