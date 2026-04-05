using System.Windows;

namespace EyeProtect.Models
{
    public class WindowModel
    {
        /// <summary>
        /// 窗口实例
        /// </summary>
        public Window window { get; set; }
        /// <summary>
        /// 所在屏幕
        /// </summary>
        public MonitorInfo screen { get; set; }
    }
}
