using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace ProjectEye.Models.Settings
{
    /// <summary>
    /// 通用设置模型
    /// </summary>
    [XmlRootAttribute("General")]
    public class GeneralModel : INotifyPropertyChanged
    {
        private bool startup = false;
        private bool noreset = false;
        private bool data = false;
        private bool sound = true;
        private bool leaveListener = false;
        private int warnTime = 20;
        private int restTime = 20;

        /// <summary>
        /// 开机启动
        /// </summary>
        public bool Startup
        {
            get => startup;
            set => SetProperty(ref startup, value);
        }

        /// <summary>
        /// 不要提醒
        /// </summary>
        public bool Noreset
        {
            get => noreset;
            set => SetProperty(ref noreset, value);
        }

        /// <summary>
        /// 统计数据
        /// </summary>
        public bool Data
        {
            get => data;
            set => SetProperty(ref data, value);
        }

        /// <summary>
        /// 休息结束提示音
        /// </summary>
        public bool Sound
        {
            get => sound;
            set => SetProperty(ref sound, value);
        }

        /// <summary>
        /// 离开监听
        /// </summary>
        public bool LeaveListener
        {
            get => leaveListener;
            set => SetProperty(ref leaveListener, value);
        }

        /// <summary>
        /// 提醒间隔时间（单位：分钟）
        /// </summary>
        public int WarnTime
        {
            get => warnTime;
            set => SetProperty(ref warnTime, value);
        }

        /// <summary>
        /// 休息时间（单位：秒）
        /// </summary>
        public int RestTime
        {
            get => restTime;
            set => SetProperty(ref restTime, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
