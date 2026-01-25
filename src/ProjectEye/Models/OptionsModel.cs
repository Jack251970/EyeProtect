using System.Collections.Generic;
using System.Windows;
using ProjectEye.Core.Models.Options;

namespace ProjectEye.Models
{
    public class OptionsModel : UINotifyPropertyChanged
    {
        private ProjectEye.Core.Models.Options.OptionsModel data_;
        public ProjectEye.Core.Models.Options.OptionsModel Data
        {
            get => data_;
            set
            {
                data_ = value;
                OnPropertyChanged("Data");
            }
        }

        private string version_;
        public string Version
        {
            get => version_;
            set
            {
                version_ = value;
                OnPropertyChanged("Version");
            }
        }

        public string VersionLink => "https://github.com/Planshit/ProjectEye/releases/tag/" + Version;
        public string SelectedItem { get; set; }
        public List<ThemeModel> Themes { get; set; }
        public List<ComboxModel> PreAlertActions { get; set; }
        public List<AnimationModel> Animations { get; set; }
        public List<ComboxModel> Languages { get; set; }

        public bool IsPreAlert
        {
            get => Data.Style.IsPreAlert;
            set
            {
                Data.Style.IsPreAlert = value;
                OnPropertyChanged();
                OnPropertyChanged("PreAlertConfigVisibility");
            }
        }
        public Visibility PreAlertConfigVisibility => Data.Style.IsPreAlert ? Visibility.Visible : Visibility.Collapsed;

        public bool IsBreakProgressList
        {
            get => Data.Behavior.IsBreakProgressList;
            set
            {
                Data.Behavior.IsBreakProgressList = value;
                OnPropertyChanged();
                OnPropertyChanged("PreAlertConfigVisibility");
            }
        }
        public Visibility BreakProgressListVisibility => Data.Behavior.IsBreakProgressList ? Visibility.Visible : Visibility.Collapsed;

        //是否显示模态弹窗
        private bool ShowModal_ = false;
        public bool ShowModal
        {
            get => ShowModal_;
            set
            {
                ShowModal_ = value;
                OnPropertyChanged();
            }
        }
        //模态弹窗文本
        private string ModalText_ = "设置已更新";
        public string ModalText
        {
            get => ModalText_;
            set
            {
                ModalText_ = value;
                OnPropertyChanged();
            }
        }

        ///// <summary>
        ///// 预提醒选项是否可用
        ///// </summary>
        //public bool IsPreTipEnabled
        //{
        //    get
        //    {
        //        return Data.Style.IsTipAsk;
        //    }
        //}
        /// <summary>
        /// 鼠标穿透是否可用
        /// </summary>
        public bool IsThruTipWindowEnabled => !Data.Style.IsTipAsk;
        public bool IsTipAsk
        {
            get => Data.Style.IsTipAsk;
            set
            {
                Data.Style.IsTipAsk = value;
                //if (!value && Data.Style.IsPreAlert)
                //{
                //    Data.Style.IsPreAlert = value;
                //}
                if (value && Data.Style.IsThruTipWindow)
                {
                    Data.Style.IsThruTipWindow = !value;
                }
                OnPropertyChanged();

            }
        }
    }
}
