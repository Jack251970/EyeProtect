using System.Collections.ObjectModel;
using System.Xml.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectEye.Models.Settings
{
    /// <summary>
    /// 行为模型
    /// </summary>
    [XmlRootAttribute("Behavior")]
    public partial class BehaviorModel : ObservableObject
    {
        /// <summary>
        /// 全屏时跳过休息
        /// </summary>
        [ObservableProperty]
        private bool isFullScreenBreak = false;

        /// <summary>
        /// 是否启用进程跳过功能
        /// </summary>
        [ObservableProperty]
        private bool isBreakProgressList = false;

        /// <summary>
        /// 跳过进程名单
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<AppInfo.AppInfo> breakProgressList = [];

        /// <summary>
        /// 是否禁用跳过休息（为true时将不允许跳过而是直接进入休息
        /// </summary>
        [ObservableProperty]
        private bool isDisabledSkip = false;

        /// <summary>
        /// 超时未选择时进入休息状态
        /// </summary>
        [ObservableProperty]
        private bool isHandleTimeoutRest = true;
    }
}
