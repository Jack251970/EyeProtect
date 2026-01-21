using Project1.UI.Controls;

namespace ProjectEye.Models
{
    public class TipViewDesignModel : UINotifyPropertyChanged
    {
        private Project1UIDesignContainer Container_;
        public Project1UIDesignContainer Container
        {
            get => Container_;
            set
            {
                Container_ = value;
                OnPropertyChanged("Container");
            }
        }
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
    }
}
