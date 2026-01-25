using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectEye.Models
{
    public partial class TipModel : ObservableObject
    {
        [ObservableProperty]
        private string tipContent;
        /// <summary>
        /// 提醒文本
        /// </summary>

        [ObservableProperty]
        private int countdown = 20;
        /// <summary>
        /// 倒计时
        /// </summary>

        [ObservableProperty]
        private Visibility countDownVisibility = Visibility.Hidden;
        /// <summary>
        /// 倒计时文本可视状态
        /// </summary>

        [ObservableProperty]
        private Visibility takeButtonVisibility = Visibility.Visible;
        /// <summary>
        /// 休息按钮可视状态
        /// </summary>

        [ObservableProperty]
        private string t;
        /// <summary>
        /// 提醒间隔（分钟）
        /// </summary>

        [ObservableProperty]
        private string time;
        /// <summary>
        /// 当前时间
        /// </summary>

        [ObservableProperty]
        private string y;
        /// <summary>
        /// 当前时间年
        /// </summary>

        [ObservableProperty]
        private string m;
        /// <summary>
        /// 当前时间月
        /// </summary>

        [ObservableProperty]
        private string d;
        /// <summary>
        /// 当前时间日
        /// </summary>

        [ObservableProperty]
        private string h;
        /// <summary>
        /// 当前时间小时
        /// </summary>

        [ObservableProperty]
        private string minutes;
        /// <summary>
        /// 当前时间：分
        /// </summary>

        [ObservableProperty]
        private string twt;
        /// <summary>
        /// 今日用眼总时长
        /// </summary>

        [ObservableProperty]
        private string trt;
        /// <summary>
        /// 今日休息总时长
        /// </summary>

        [ObservableProperty]
        private string tsc;
        /// <summary>
        /// 今日跳过次数
        /// </summary>

        [ObservableProperty]
        private bool isThruWindow;
        /// <summary>
        /// 是否启用窗口鼠标穿透
        /// </summary>

        [ObservableProperty]
        private System.Windows.Media.ImageSource tipImageSource;
        /// <summary>
        /// 提示图片源
        /// </summary>

        [ObservableProperty]
        private System.Windows.Media.Brush containerBackground;
        /// <summary>
        /// 容器背景
        /// </summary>

        [ObservableProperty]
        private double containerOpacity = 0.98;
        /// <summary>
        /// 容器透明度
        /// </summary>
    }
}
