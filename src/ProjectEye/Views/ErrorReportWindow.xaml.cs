using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace ProjectEye.Views
{
    /// <summary>
    /// ErrorReportWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ErrorReportWindow : Project1.UI.Controls.Project1UIWindow
    {
        public ErrorReportWindow()
        {
            InitializeComponent();
        }

        private void Project1UIButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/Planshit/ProjectEye/releases") { UseShellExecute = true });
        }

        private void bugreportbtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/Planshit/ProjectEye/issues/new") { UseShellExecute = true });
        }

        private void Project1UIButton_Click_1(object sender, RoutedEventArgs e)
        {
            var exePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "ProjectEye.exe");
            Process.Start(new ProcessStartInfo(exePath) { UseShellExecute = true });
            Application.Current.Shutdown();
        }

        private void Run_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Log");
            if (System.IO.Directory.Exists(logPath))
            {
                Process.Start(new ProcessStartInfo(logPath) { UseShellExecute = true });
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText("heis@thelittlepandaisbehind.com");
            MessageBox.Show("邮箱已复制");
        }
    }
}
