using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EyeProtect.Models
{
    public partial class ContainerModel : ObservableObject
    {
        [ObservableProperty]
        private double opacity;

        [ObservableProperty]
        private Brush background;
    }
}
