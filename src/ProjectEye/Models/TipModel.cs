using System.Windows;
using static Project1.UI.Controls.Project1UIWindow;

namespace ProjectEye.Models
{
    public class TipModel : UINotifyPropertyChanged
    {
        private bool IsAnimation_;
        /// <summary>
        /// 是否启用动画
        /// </summary>
        public bool IsAnimation
        {
            get => IsAnimation_;
            set
            {
                IsAnimation_ = value;
                OnPropertyChanged();
            }
        }

        private string TipContent_;
        /// <summary>
        /// 提醒文本
        /// </summary>
        public string TipContent
        {
            get => TipContent_;
            set
            {
                TipContent_ = value;
                OnPropertyChanged();
            }
        }
        private int CountDown_ = 20;
        /// <summary>
        /// 倒计时
        /// </summary>
        public int COUNTDOWN
        {
            get => CountDown_;
            set
            {
                CountDown_ = value;
                OnPropertyChanged();
            }
        }

        private Visibility CountDownVisibility_ = Visibility.Hidden;
        /// <summary>
        /// 倒计时文本可视状态
        /// </summary>
        public Visibility CountDownVisibility
        {
            get => CountDownVisibility_;
            set
            {
                CountDownVisibility_ = value;
                OnPropertyChanged("CountDownVisibility");
            }
        }

        private Visibility TakeButtonVisibility_ = Visibility.Visible;
        /// <summary>
        /// 休息按钮可视状态
        /// </summary>
        public Visibility TakeButtonVisibility
        {
            get => TakeButtonVisibility_;
            set
            {
                TakeButtonVisibility_ = value;
                OnPropertyChanged("TakeButtonVisibility");
            }
        }

        private string T_;
        /// <summary>
        /// 提醒间隔（分钟）
        /// </summary>
        public string T
        {
            get => T_;
            set
            {
                T_ = value;
                OnPropertyChanged();
            }
        }
        private string TIME_;
        /// <summary>
        /// 当前时间
        /// </summary>
        public string TIME
        {
            get => TIME_;
            set
            {
                TIME_ = value;
                OnPropertyChanged();
            }
        }
        private string Y_;
        /// <summary>
        /// 当前时间年
        /// </summary>
        public string Y
        {
            get => Y_;
            set
            {
                Y_ = value;
                OnPropertyChanged();
            }
        }
        private string M_;
        /// <summary>
        /// 当前时间月
        /// </summary>
        public string M
        {
            get => M_;
            set
            {
                M_ = value;
                OnPropertyChanged();
            }
        }
        private string D_;
        /// <summary>
        /// 当前时间日
        /// </summary>
        public string D
        {
            get => D_;
            set
            {
                D_ = value;
                OnPropertyChanged();
            }
        }
        private string H_;
        /// <summary>
        /// 当前时间小时
        /// </summary>
        public string H
        {
            get => H_;
            set
            {
                H_ = value;
                OnPropertyChanged();
            }
        }
        private string MINUTES_;
        /// <summary>
        /// 当前时间：分
        /// </summary>
        public string MINUTES
        {
            get => MINUTES_;
            set
            {
                MINUTES_ = value;
                OnPropertyChanged();
            }
        }
        private string TWT_;
        /// <summary>
        /// 今日用眼总时长
        /// </summary>
        public string TWT
        {
            get => TWT_;
            set
            {
                TWT_ = value;
                OnPropertyChanged();
            }
        }
        private string TRT_;
        /// <summary>
        /// 今日休息总时长
        /// </summary>
        public string TRT
        {
            get => TRT_;
            set
            {
                TRT_ = value;
                OnPropertyChanged();
            }
        }
        private string TSC_;
        /// <summary>
        /// 今日跳过次数
        /// </summary>
        public string TSC
        {
            get => TSC_;
            set
            {
                TSC_ = value;
                OnPropertyChanged();
            }
        }

        private AnimationType WindowAnimationType_;
        /// <summary>
        /// 窗口动画类型
        /// </summary>
        public AnimationType WindowAnimationType
        {
            get => WindowAnimationType_;
            set
            {
                WindowAnimationType_ = value;
                OnPropertyChanged();
            }
        }

        private bool IsThruWindow_;
        /// <summary>
        /// 是否启用窗口鼠标穿透
        /// </summary>
        public bool IsThruWindow
        {
            get => IsThruWindow_;
            set
            {
                IsThruWindow_ = value;
                OnPropertyChanged();
            }
        }
    }
}
