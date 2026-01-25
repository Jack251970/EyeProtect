using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace ProjectEye.Views
{
    /// <summary>
    /// ErrorReportWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ErrorReportWindow : Window
    {
        public ErrorReportWindow()
        {
            InitializeComponent();
        }

        private void bugreportbtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/Jack251970/EyeProtect/issues/new") { UseShellExecute = true });
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
    }
}
