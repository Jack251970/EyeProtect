using ProjectEye.Models;
using Windows.Win32;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.HiDpi;

namespace ProjectEye.Core
{
    public static class ScreenExtensions
    {
        public readonly struct Dpi
        {
            public uint x { get; init; }
            public uint y { get; init; }
        }

        public static Dpi GetDpi(this MonitorInfo screen, MONITOR_DPI_TYPE dpiType)
        {
            var pnt = new System.Drawing.Point((int)screen.Bounds.Left + 1, (int)screen.Bounds.Top + 1);
            var mon = PInvoke.MonitorFromPoint(pnt, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST);
            PInvoke.GetDpiForMonitor(mon, dpiType, out var dpiX, out var dpiY);
            return new Dpi
            {
                x = dpiX,
                y = dpiY
            };
        }
    }
}
