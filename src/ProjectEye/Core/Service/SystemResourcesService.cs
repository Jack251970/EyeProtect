using System.Collections.Generic;
using System.Windows;
using ProjectEye.Core.Models.Options;

namespace ProjectEye.Core.Service
{
    public class SystemResourcesService : IService
    {
        public List<ThemeModel> Themes { get; set; }
        public List<AnimationModel> Animations { get; set; }
        public List<ComboxModel> Languages { get; set; }

        public void Init()
        {
            Themes = new List<ThemeModel>();
            Animations = new List<AnimationModel>();
            Languages = new List<ComboxModel>();

            Themes.Add(new ThemeModel()
            {
                DisplayName = $"{Application.Current.Resources["Lang_Light"]}",
                ThemeName = "Blue",
                ThemeColor = "#4F6BED"
            });
            Themes.Add(new ThemeModel()
            {
                DisplayName = $"{Application.Current.Resources["Lang_Dark"]}",
                ThemeName = "Dark",
                ThemeColor = "#4F6BED"
            });

            //预置动画
            Animations.Add(new AnimationModel()
            {
                ID = 0,
                AnimationType = AnimationType.None,
                DisplayName = $"{Application.Current.Resources["Lang_None"]}"
            });
            Animations.Add(new AnimationModel()
            {
                ID = 1,
                AnimationType = AnimationType.RightBottomScale,
                DisplayName = $"{Application.Current.Resources["Lang_Zoomfrombottom-rightcorner"]}"
            });
            Animations.Add(new AnimationModel()
            {
                ID = 2,
                AnimationType = AnimationType.Opacity,
                DisplayName = $"{Application.Current.Resources["Lang_Fadeinandfadeout"]}"
            });
            Animations.Add(new AnimationModel()
            {
                ID = 3,
                AnimationType = AnimationType.Cool,
                DisplayName = $"Cool"
            });
            //语言
            Languages.Add(new ComboxModel()
            {
                DisplayName = "English",
                Value = "en"
            });
            Languages.Add(new ComboxModel()
            {
                DisplayName = "简体中文",
                Value = "zh-cn"
            });
            Languages.Add(new ComboxModel()
            {
                DisplayName = "繁體中文",
                Value = "zh-tw"
            });
        }
    }
}
