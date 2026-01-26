using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectEye.Models.Settings
{
    /// <summary>
    /// 通用设置模型
    /// </summary>
    public partial class GeneralModel : ObservableObject
    {
        /// <summary>
        /// 开机启动
        /// </summary>
        [ObservableProperty]
        private bool startup = false;

        /// <summary>
        /// 不要提醒
        /// </summary>
        [ObservableProperty]
        private bool noreset = false;

        /// <summary>
        /// 统计数据
        /// </summary>
        [ObservableProperty]
        private bool data = false;

        /// <summary>
        /// 休息结束提示音
        /// </summary>
        [ObservableProperty]
        private bool sound = true;

        /// <summary>
        /// 离开监听
        /// </summary>
        [ObservableProperty]
        private bool leaveListener = false;

        /// <summary>
        /// 提醒间隔时间（单位：分钟）
        /// </summary>
        [ObservableProperty]
        private int warnTime = 20;

        /// <summary>
        /// 休息时间（单位：秒）
        /// </summary>
        [ObservableProperty]
        private int restTime = 20;

        /// <summary>
        /// 上次退出时间戳（用于恢复计时器）
        /// </summary>
        [ObservableProperty]
        private long lastExitTimestamp = 0;

        /// <summary>
        /// 上次退出时剩余秒数（用于恢复计时器）
        /// </summary>
        [ObservableProperty]
        private double remainingSeconds = 0;
    }
}
