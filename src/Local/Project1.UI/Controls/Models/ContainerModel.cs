using System.Windows.Media;
using Project1.UI.Cores;

namespace Project1.UI.Controls.Models
{
    public class ContainerModel : UINotifyPropertyChanged
    {
        private double Opacity_;
        public double Opacity
        {
            get
            {
                return Opacity_;
            }
            set
            {
                Opacity_ = value;
                OnPropertyChanged();
            }
        }
        private Brush Background_;
        public Brush Background
        {
            get
            {

                return Background_;
            }
            set
            {
                Background_ = value;
                OnPropertyChanged();
            }
        }
    }
}
