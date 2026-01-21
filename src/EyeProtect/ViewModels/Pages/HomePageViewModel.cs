using CommunityToolkit.Mvvm.ComponentModel;

namespace EyeProtect.ViewModels.Pages;

public partial class HomePageViewModel : ObservableRecipient
{
    [ObservableProperty]
    public partial string AppDisplayName { get; set; } = ConstantHelper.AppDisplayName;

    public HomePageViewModel()
    {

    }
}
