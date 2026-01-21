using System.Windows;

namespace ProjectEye.Models
{
    public class EyesTestModel : UINotifyPropertyChanged
    {
        private int Score_ = 0;
        public int Score
        {
            get => Score_;

            set
            {
                Score_ = value;
                OnPropertyChanged();
            }
        }
        private string ScoreInfo_ = "";
        public string ScoreInfo
        {
            get => ScoreInfo_;

            set
            {
                ScoreInfo_ = value;
                OnPropertyChanged();
            }
        }

        private int Index_ = 1;
        public int Index
        {
            get => Index_;

            set
            {
                Index_ = value;
                OnPropertyChanged();
            }
        }

        private double FontSize_;
        public double FontSize
        {
            get => FontSize_;

            set
            {
                FontSize_ = value;
                OnPropertyChanged();
            }
        }

        private Visibility InfoVisibility_ = Visibility.Visible;
        public Visibility InfoVisibility
        {
            get => InfoVisibility_;
            set
            {
                InfoVisibility_ = value;
                OnPropertyChanged();
            }
        }

        private Visibility TestVisibility_ = Visibility.Hidden;
        public Visibility TestVisibility
        {
            get => TestVisibility_;
            set
            {
                TestVisibility_ = value;
                OnPropertyChanged();
            }
        }
        private Visibility ScoreVisibility_ = Visibility.Hidden;
        public Visibility ScoreVisibility
        {
            get => ScoreVisibility_;
            set
            {
                ScoreVisibility_ = value;
                OnPropertyChanged();
            }
        }

        public double[] EyesData_;
        public double[] EyesData
        {
            get => EyesData_;
            set
            {
                EyesData_ = value;
                OnPropertyChanged();
            }
        }
        public string[] Labels_;
        public string[] Labels
        {
            get => Labels_;
            set
            {
                Labels_ = value;
                OnPropertyChanged();
            }
        }
    }
}
