using iNKORE.UI.WPF.Modern;

namespace ProjectEye.Core.Service
{
    public class ThemeService : IService
    {
        public delegate void ThemeChangedEventHandler(ApplicationTheme OldTheme, ApplicationTheme NewTheme);
        /// <summary>
        /// 当切换主题时发生
        /// </summary>
        public event ThemeChangedEventHandler OnChangedTheme;

        public void Init()
        {
            ThemeManager.Current.ActualApplicationThemeChanged += Current_ActualApplicationThemeChanged;
        }

        private void Current_ActualApplicationThemeChanged(ThemeManager sender, object args)
        {
            if (sender.ActualApplicationTheme == ApplicationTheme.Dark)
            {
                OnChangedTheme?.Invoke(ApplicationTheme.Light, ApplicationTheme.Dark);
            }
            else if (sender.ActualApplicationTheme == ApplicationTheme.Light)
            {
                OnChangedTheme?.Invoke(ApplicationTheme.Dark, ApplicationTheme.Light);
            }
        }
    }
}
