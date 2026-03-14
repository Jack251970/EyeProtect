using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using EyeProtect.Models;
using EyeProtect.ViewModels;
using Windows.Win32.UI.HiDpi;

namespace EyeProtect.Core
{
    /// <summary>
    /// 窗口管理
    /// </summary>
    public class WindowManager
    {
        private static readonly List<WindowModel> windowList;
        private static readonly List<object> viewModelList;
        
        static WindowManager()
        {
            windowList = [];
            viewModelList = [];
        }

        //window
        #region 创建窗口
        private static Window CreateWindow(string name, string screen, double left = -999999, double top = -999999, double width = -999999, double height = -999999, bool newViewModel = false)
        {
            //var selectWindow = GetWindowByScreen(name, screen);
            //if (selectWindow != null)
            //{
            //    return selectWindow;
            //}
            var viewModel = GetCreateViewModel(name, newViewModel);

            var type = Type.GetType("EyeProtect.Views." + name);
            var objWindow = (Window)type.Assembly.CreateInstance(type.FullName);
            objWindow.Uid = name;
            objWindow.DataContext = viewModel;
            objWindow.Closed += window_closed;
            if (left > -999999)
            {
                objWindow.Left = left;
            }
            if (top > -999999)
            {
                objWindow.Top = top;
            }
            if (width > -999999)
            {
                objWindow.Width = width;
            }
            if (height > -999999)
            {
                objWindow.Height = height;
            }

            if (viewModel != null)
            {
                if (viewModel is IViewModel basicModel)
                {
                    basicModel.ScreenName = screen.Replace("\\", "");
                    basicModel.WindowInstance = objWindow;
                    basicModel.OnChanged();
                }
            }

            var windowModel = new WindowModel
            {
                window = objWindow,
                screen = screen
            };

            windowList.Add(windowModel);
            return objWindow;
        }

        /// <summary>
        /// 在指定显示器上创建一个window（默认在主显示器）
        /// </summary>
        /// <param name="name">窗口类名</param>
        /// <param name="screen">显示器</param>
        /// <returns></returns>
        public static Window CreateWindowInScreen(string name, MonitorInfo screen = null, bool isMaximized = false, bool newViewModel = false)
        {
            //创建
            double left = -999999, top = -999999, width = -999999, height = -999999;
            if (screen == null)
            {
                screen = MonitorInfo.GetPrimaryDisplayMonitor();
            }
            if (isMaximized)
            {
                var size = GetSize(screen);
                left = ToDips(screen.Bounds.Left, size.XDPI);
                top = ToDips(screen.Bounds.Top, size.YDPI);

                width = size.Width;
                height = size.Height;
            }
            var window = CreateWindow(name,
                screen.Name,
                left,
                top,
                width,
                height,
                newViewModel);
            return window;
        }
        /// <summary>
        /// 在所有显示器中创建一个窗口
        /// </summary>
        /// <param name="name">窗口类名</param>
        /// <param name="isMaximized">是否全屏</param>
        /// <returns></returns>
        public static Window[] CreateWindow(string name, bool isMaximized, bool newViewModel = false)
        {
            var allScreens = MonitorInfo.GetDisplayMonitors();
            var screenCount = allScreens.Count;
            var screens = allScreens;
            var windows = new Window[screenCount];

            for (var index = 0; index < screenCount; index++)
            {
                var screen = screens[index];
                var size = GetSize(screen);
                double width = -999999;
                double height = -999999;
                if (isMaximized)
                {
                    width = size.Width;
                    height = size.Height;
                }
                var left = ToDips(screen.Bounds.Left, size.XDPI);
                var top = ToDips(screen.Bounds.Top, size.YDPI);

                var window = CreateWindow(name, screen.Name, left, top, width, height, newViewModel);
                windows[index] = window;

            }
            return windows;
        }
        #endregion

        #region 获取窗口
        /// <summary>
        /// 通过窗口类名获取已经创建的窗口实例
        /// </summary>
        /// <param name="name">窗口类名</param>
        /// <returns>成功返回窗口实例数组，失败返回NULL</returns>
        public static Window[] GetWindows(string name)
        {
            var window = windowList.Where(m => m.window.Uid == name).Select(s => s.window);
            if (window.Count() > 0)
            {
                return window.ToArray();
            }
            return null;
        }
        /// <summary>
        /// 获取窗口实例，如果没有找到则会创建
        /// </summary>
        /// <param name="name"></param>
        /// <returns>成功返回窗口实例数组</returns>
        public static Window[] GetCreateWindow(string name, bool isMaximized, bool newViewModel = false)
        {
            var window = GetWindows(name);
            if (window == null)
            {
                window = CreateWindow(name, isMaximized, newViewModel);
            }
            return window;
        }
        /// <summary>
        /// 获取window通过窗口类名+显示器（驱动名称）查找
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="screen"></param>
        /// <returns>成功只会返回window实例</returns>
        public static Window GetWindowByScreen(string windowName, string screen)
        {
            var select = windowList.Where(m => m.window.Uid == windowName
              && m.screen == screen).Select(s => s.window);
            if (select.Count() == 1)
            {
                return select.Single();
            }
            return null;
        }
        /// <summary>
        /// 获取windowmodel
        /// </summary>
        /// <param name="windowName">窗口类名</param>
        /// <param name="screen">显示器</param>
        /// <returns></returns>
        public static WindowModel GetWindowModel(string windowName, string screen)
        {
            var select = windowList.Where(m => m.window.Uid == windowName
              && m.screen == screen);
            if (select.Count() > 0)
            {
                return select.Single();
            }
            return null;
        }
        #endregion

        #region 显示窗口
        public static void Show(string name)
        {
            var screens = MonitorInfo.GetDisplayMonitors();
            foreach (var screen in screens)
            {
                var window = GetWindowByScreen(name, screen.Name);
                if (window != null)
                {
                    if (window.DataContext is IViewModel viewmodel)
                    {
                        viewmodel.BeforeShown();
                    }
                    Show(window);
                }
            }
        }
        private static void Show(Window window)
        {
            // Fix UI bug
            // Add `window.WindowState = WindowState.Normal`
            // If only use `window.Show()`, Settings-window doesn't show when minimized in taskbar 
            // Not sure why this works tho
            // Probably because, when `.Show()` fails, `window.WindowState == Minimized` (not `Normal`) 
            // https://stackoverflow.com/a/59719760/4230390
            // Ensure the window is not minimized before showing it
            if (window.WindowState == WindowState.Minimized)
            {
                window.WindowState = WindowState.Normal;
            }

            // Ensure the window is visible
            if (!window.IsVisible)
            {
                window.Show();
            }
            else
            {
                window.Activate(); // Bring the window to the foreground if already open
            }

            window.Focus();
        }
        #endregion

        #region 关闭窗口
        /// <summary>
        /// 关闭窗口（所有显示器）
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int Close(string name)
        {
            var windows = GetWindows(name);
            if (windows == null)
            {
                return 0;
            }

            foreach (var window in windows)
            {
                window.Close();
            }
            RemoveViewModel(name);
            RemoveWindow(name);
            return windows.Length;
        }
        #endregion

        #region 隐藏窗口
        public static int Hide(string name)
        {
            var windows = GetWindows(name);
            if (windows == null)
            {
                return 0;
            }

            foreach (var window in windows)
            {
                window.Hide();
            }
            return windows.Length;
        }
        #endregion

        #region 移除窗口实例
        private static void RemoveWindow(string name)
        {
            var select = windowList.Where(m => m.window.Uid == name).ToList();
            foreach (var windowModel in select)
            {
                windowList.Remove(windowModel);
            }
        }
        #endregion

        #region 在所有显示器中刷新一个窗口
        /// <summary>
        /// 在所有显示器中刷新一个窗口，如果在某个显示器中没有实例则会创建。
        /// </summary>
        /// <param name="name"></param>
        public static void UpdateAllScreensWindow(string name, bool isMaximized)
        {
            var screens = MonitorInfo.GetDisplayMonitors();
            foreach (var screen in screens)
            {
                var window = GetWindowByScreen(name, screen.Name);
                if (window != null)
                {
                    var size = GetSize(screen);
                    window.Left = ToDips(screen.Bounds.Left, size.XDPI);
                    window.Top = ToDips(screen.Bounds.Top, size.YDPI);
                    window.Width = size.Width;
                    window.Height = size.Height;
                }
                else
                {
                    CreateWindowInScreen(name, screen, isMaximized, newViewModel: true);
                }
            }
        }
        #endregion

        #region 获得显示器宽高dips
        public class Size
        {
            public double Width { get; set; }
            public double Height { get; set; }
            public uint XDPI { get; set; }
            public uint YDPI { get; set; }

        }
        public static Size GetSize(MonitorInfo screen)
        {
            //uint xDpi, yDpi;
            var dpi = screen.GetDpi(MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI);
            var size = new Size
            {
                Width = screen.Bounds.Width / (dpi.x / 96.0),
                Height = screen.Bounds.Height / (dpi.y / 96.0),
                XDPI = dpi.x,
                YDPI = dpi.y
            };
            return size;
        }
        #endregion

        #region 计算dips
        public enum DpiDirection
        {
            X,
            Y
        }
        public static double ToDips(MonitorInfo screen, double value, DpiDirection dpiDirection = DpiDirection.X)
        {
            var dpi = screen.GetDpi(MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI);

            return value / (dpiDirection == DpiDirection.X ? dpi.x : dpi.y / 96.0);
        }

        public static double ToDips(double value, uint dpi)
        {
            return value / (dpi / 96.0);
        }
        #endregion

        //window event
        #region 窗口被关闭event
        private static void window_closed(object sender, EventArgs e)
        {
            var window = sender as Window;
            Close(window.Uid);
        }
        #endregion

        //viewmodel
        #region 创建viewmodel实例
        private static object CreateViewModel(string windowName)
        {
            var nameSpace = "EyeProtect.ViewModels";
            var viewModelName = windowName.Replace("Window", "ViewModel");
            var type = Type.GetType(nameSpace + "." + viewModelName);
            if (type == null)
            {
                //找不到对应的ViewModel
                return null;
            }
            var constructorInfoObj = type.GetConstructors()[0];
            var constructorParameters = constructorInfoObj.GetParameters();
            var constructorParametersLength = constructorParameters.Length;
            var types = new Type[constructorParametersLength];
            var objs = new object[constructorParametersLength];
            for (var i = 0; i < constructorParametersLength; i++)
            {
                var parameterType = constructorParameters[i].ParameterType;
                types[i] = parameterType;
                objs[i] = Ioc.Default.GetService(parameterType);
            }
            var ctor = type.GetConstructor(types);
            var instance = ctor.Invoke(objs);
            viewModelList.Add(instance);
            return instance;
        }
        #endregion

        #region 获取viewmodel实例
        private static List<object> GetViewModel(string windowName)
        {
            var viewModelName = windowName.Replace("Window", "ViewModel");
            var select = viewModelList.Where(m => m.GetType().Name == viewModelName);
            if (select.Count() > 0)
            {
                return select.ToList();
            }
            return null;
        }
        #endregion

        #region 获取viewmodel实例，不存在时创建
        private static object GetCreateViewModel(string windowName, bool newViewmodel = false)
        {
            var viewModel = GetViewModel(windowName);
            if (viewModel == null || newViewmodel)
            {
                return CreateViewModel(windowName);
            }
            return viewModel[0];
        }
        #endregion

        #region 移除viewmodel实例
        private static void RemoveViewModel(string windowName)
        {
            var viewModel = GetViewModel(windowName);
            if (viewModel != null)
            {
                foreach (var model in viewModel)
                {
                    viewModelList.Remove(model);
                }
            }
        }
        #endregion
    }
}
