using System.Collections.Generic;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using ProjectEye.Core.Models.Options;

namespace ProjectEye.Models
{
    public partial class OptionsModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BreakProgressListVisibility))]
        private Core.Models.Options.OptionsModel data;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(VersionLink))]
        private string version;

        public string VersionLink => "https://github.com/Planshit/ProjectEye/releases/tag/" + Version;
        public string SelectedItem { get; set; }
        public List<ComboxModel> Languages { get; set; }

        public bool IsBreakProgressList
        {
            get => Data?.Behavior.IsBreakProgressList ?? false;
            set
            {
                if (Data != null)
                {
                    Data.Behavior.IsBreakProgressList = value;
                    OnPropertyChanged(nameof(IsBreakProgressList));
                    OnPropertyChanged(nameof(BreakProgressListVisibility));
                }
            }
        }
        public Visibility BreakProgressListVisibility => (Data?.Behavior.IsBreakProgressList ?? false) ? Visibility.Visible : Visibility.Collapsed;

        [ObservableProperty]
        private bool showModal = false;
        //是否显示模态弹窗

        [ObservableProperty]
        private string modalText = "设置已更新";
        //模态弹窗文本

        /// <summary>
        /// 鼠标穿透是否可用
        /// </summary>
        public bool IsThruTipWindowEnabled => !(Data?.Style.IsTipAsk ?? false);
        public bool IsTipAsk
        {
            get => Data?.Style.IsTipAsk ?? false;
            set
            {
                if (Data != null)
                {
                    Data.Style.IsTipAsk = value;
                    if (value && Data.Style.IsThruTipWindow)
                    {
                        Data.Style.IsThruTipWindow = !value;
                    }
                    OnPropertyChanged(nameof(IsTipAsk));
                }
            }
        }
    }
}
