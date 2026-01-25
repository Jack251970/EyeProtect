using System.Windows.Media;

namespace ProjectEye.Models
{
    public class ContainerModel : UINotifyPropertyChanged
    {
        private double Opacity_;
        public double Opacity
        {
            get => Opacity_;
            set
            {
                Opacity_ = value;
                OnPropertyChanged();
            }
        }
        private Brush Background_;
        public Brush Background
        {
            get => Background_;
            set
            {
                Background_ = value;
                OnPropertyChanged();
            }
        }
    }
}
