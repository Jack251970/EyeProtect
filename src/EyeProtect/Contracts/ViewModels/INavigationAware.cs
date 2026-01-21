namespace EyeProtect.Contracts.ViewModels;

internal interface INavigationAware
{
    void OnNavigatedTo(object parameter);

    void OnNavigatedFrom();
}
