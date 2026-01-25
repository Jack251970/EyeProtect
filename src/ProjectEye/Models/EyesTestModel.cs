using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectEye.Models
{
    public partial class EyesTestModel : ObservableObject
    {
        [ObservableProperty]
        private int score = 0;

        [ObservableProperty]
        private string scoreInfo = "";

        [ObservableProperty]
        private int index = 1;

        [ObservableProperty]
        private double fontSize;

        [ObservableProperty]
        private Visibility infoVisibility = Visibility.Visible;

        [ObservableProperty]
        private Visibility testVisibility = Visibility.Hidden;

        [ObservableProperty]
        private Visibility scoreVisibility = Visibility.Hidden;

        [ObservableProperty]
        private double[] eyesData;

        [ObservableProperty]
        private string[] labels;
    }
}
