using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace ProjectEye.Models.Settings
{
    /// <summary>
    /// 行为模型
    /// </summary>
    [XmlRootAttribute("Behavior")]
    public class BehaviorModel : INotifyPropertyChanged
    {
        private bool isFullScreenBreak = false;
        private bool isBreakProgressList = false;
        private ObservableCollection<AppInfo.AppInfo> breakProgressList = [];
        private bool isDisabledSkip = false;
        private bool isHandleTimeoutRest = true;

        /// <summary>
        /// 全屏时跳过休息
        /// </summary>
        public bool IsFullScreenBreak
        {
            get => isFullScreenBreak;
            set => SetProperty(ref isFullScreenBreak, value);
        }

        /// <summary>
        /// 是否启用进程跳过功能
        /// </summary>
        public bool IsBreakProgressList
        {
            get => isBreakProgressList;
            set => SetProperty(ref isBreakProgressList, value);
        }

        /// <summary>
        /// 跳过进程名单
        /// </summary>
        public ObservableCollection<AppInfo.AppInfo> BreakProgressList
        {
            get => breakProgressList;
            set => SetProperty(ref breakProgressList, value);
        }

        /// <summary>
        /// 是否禁用跳过休息（为true时将不允许跳过而是直接进入休息
        /// </summary>
        public bool IsDisabledSkip
        {
            get => isDisabledSkip;
            set => SetProperty(ref isDisabledSkip, value);
        }

        /// <summary>
        /// 超时未选择时进入休息状态
        /// </summary>
        public bool IsHandleTimeoutRest
        {
            get => isHandleTimeoutRest;
            set => SetProperty(ref isHandleTimeoutRest, value);
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
