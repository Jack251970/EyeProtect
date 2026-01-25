using System;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;

namespace ProjectEye.ViewModels
{
    public partial class ContributorsViewModel
    {
        [RelayCommand]
        private void Openurl(object obj)
        {
            Process.Start(new ProcessStartInfo(obj.ToString()));
        }
    }
}
