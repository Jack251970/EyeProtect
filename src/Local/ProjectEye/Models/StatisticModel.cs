using System.Collections.Generic;
using System.Windows.Media;
using Project1.UI.Controls.ChartControl.Models;

namespace ProjectEye.Models
{
    public class StatisticModel : UINotifyPropertyChanged
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

        private bool IsShowOnboarding_;
        /// <summary>
        /// 是否显示引导页
        /// </summary>
        public bool IsShowOnboarding
        {
            get => IsShowOnboarding_;
            set
            {
                IsShowOnboarding_ = value;
                OnPropertyChanged();
            }
        }


        private int Year_;
        public int Year
        {
            get => Year_;
            set
            {
                Year_ = value;
                OnPropertyChanged("Year");
            }
        }
        private int Month_;
        public int Month
        {
            get => Month_;
            set
            {
                Month_ = value;
                OnPropertyChanged("Month");
            }
        }

        private List<ChartDataModel> MonthWorkData_;
        /// <summary>
        /// 月份工作统计数据
        /// </summary>
        public List<ChartDataModel> MonthWorkData { get => MonthWorkData_; set { MonthWorkData_ = value; OnPropertyChanged(); } }


        private List<ChartDataModel> MonthRestData_;
        /// <summary>
        /// 月份休息统计数据
        /// </summary>
        public List<ChartDataModel> MonthRestData { get => MonthRestData_; set { MonthRestData_ = value; OnPropertyChanged(); } }

        private List<ChartDataModel> MonthSkipData_;
        /// <summary>
        /// 月份跳过统计数据
        /// </summary>
        public List<ChartDataModel> MonthSkipData { get => MonthSkipData_; set { MonthSkipData_ = value; OnPropertyChanged(); } }

        #region 本周数据
        private List<ChartDataModel> WeekWorkData_;
        /// <summary>
        /// 本周工作统计数据
        /// </summary>
        public List<ChartDataModel> WeekWorkData { get => WeekWorkData_; set { WeekWorkData_ = value; OnPropertyChanged(); } }


        private List<ChartDataModel> WeekRestData_;
        /// <summary>
        /// 本周休息统计数据
        /// </summary>
        public List<ChartDataModel> WeekRestData { get => WeekRestData_; set { WeekRestData_ = value; OnPropertyChanged(); } }

        private List<ChartDataModel> WeekSkipData_;
        /// <summary>
        /// 本周跳过统计数据
        /// </summary>
        public List<ChartDataModel> WeekSkipData { get => WeekSkipData_; set { WeekSkipData_ = value; OnPropertyChanged(); } }
        #endregion

        #region 本月数据总和
        private double MonthWork_;
        public double MonthWork
        {
            get => MonthWork_;
            set
            {
                MonthWork_ = value;
                OnPropertyChanged();
            }
        }
        private double MonthRest_;
        public double MonthRest
        {
            get => MonthRest_;
            set
            {
                MonthRest_ = value;
                OnPropertyChanged();
            }
        }
        private int MonthSkip_;
        public int MonthSkip
        {
            get => MonthSkip_;
            set
            {
                MonthSkip_ = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 上月数据总和
        private double LastMonthWork_;
        public double LastMonthWork
        {
            get => LastMonthWork_;
            set
            {
                LastMonthWork_ = value;
                OnPropertyChanged();
            }
        }
        private double LastMonthRest_;
        public double LastMonthRest
        {
            get => LastMonthRest_;
            set
            {
                LastMonthRest_ = value;
                OnPropertyChanged();
            }
        }
        private int LastMonthSkip_;
        public int LastMonthSkip
        {
            get => LastMonthSkip_;
            set
            {
                LastMonthSkip_ = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 本周数据总和

        private int WeekTrueWorkDays_;
        /// <summary>
        /// 本周真实工作天数（筛选用眼时长>0）
        /// </summary>
        public int WeekTrueWorkDays
        {
            get => WeekTrueWorkDays_;
            set
            {
                WeekTrueWorkDays_ = value;
                OnPropertyChanged();
            }
        }

        private double WeekWork_;
        /// <summary>
        /// 本周工作总时长（小时）
        /// </summary>
        public double WeekWork
        {
            get => WeekWork_;
            set
            {
                WeekWork_ = value;
                OnPropertyChanged();
            }
        }
        private double WeekRest_;
        /// <summary>
        /// 本周休息总时长（分钟）
        /// </summary>
        public double WeekRest
        {
            get => WeekRest_;
            set
            {
                WeekRest_ = value;
                OnPropertyChanged();
            }
        }
        private int WeekSkip_;
        /// <summary>
        /// 本周跳过次数
        /// </summary>
        public int WeekSkip
        {
            get => WeekSkip_;
            set
            {
                WeekSkip_ = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 上周数据总和
        private double LastWeekWork_;
        public double LastWeekWork
        {
            get => LastWeekWork_;
            set
            {
                LastWeekWork_ = value;
                OnPropertyChanged();
            }
        }
        private double LastWeekRest_;
        public double LastWeekRest
        {
            get => LastWeekRest_;
            set
            {
                LastWeekRest_ = value;
                OnPropertyChanged();
            }
        }
        private int LastWeekSkip_;
        public int LastWeekSkip
        {
            get => LastWeekSkip_;
            set
            {
                LastWeekSkip_ = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 本周数据评估
        private string WorkAnalysis_;
        public string WorkAnalysis
        {
            get => WorkAnalysis_;
            set
            {
                WorkAnalysis_ = value;
                OnPropertyChanged();
            }
        }
        private string RestAnalysis_;
        public string RestAnalysis
        {
            get => RestAnalysis_;
            set
            {
                RestAnalysis_ = value;
                OnPropertyChanged();
            }
        }
        private string SkipAnalysis_;
        public string SkipAnalysis
        {
            get => SkipAnalysis_;
            set
            {
                SkipAnalysis_ = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 窗口占位图片
        private ImageSource WorktimeImageSource_;
        public ImageSource WorktimeImageSource
        {
            get => WorktimeImageSource_; set => WorktimeImageSource_ = value;
        }

        private ImageSource ResttimeImageSource_;
        public ImageSource ResttimeImageSource
        {
            get => ResttimeImageSource_; set => ResttimeImageSource_ = value;
        }
        private ImageSource SkipImageSource_;
        public ImageSource SkipImageSource
        {
            get => SkipImageSource_; set => SkipImageSource_ = value;
        }

        #endregion


        #region 本周状态等级
        private int WeekWorkLevel_;
        public int WeekWorkLevel
        {
            get => WeekWorkLevel_;
            set
            {
                WeekWorkLevel_ = value;
                OnPropertyChanged("WeekWorkLevel");
            }
        }
        private int WeekRestLevel_;
        public int WeekRestLevel
        {
            get => WeekRestLevel_;
            set
            {
                WeekRestLevel_ = value;
                OnPropertyChanged("WeekRestLevel");
            }
        }
        private int WeekSkipLevel_;
        public int WeekSkipLevel
        {
            get => WeekSkipLevel_;
            set
            {
                WeekSkipLevel_ = value;
                OnPropertyChanged("WeekSkipLevel");
            }
        }
        #endregion
    }
}
