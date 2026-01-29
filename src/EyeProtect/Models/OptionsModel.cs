using System.Collections.Generic;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using EyeProtect.Models.Settings;

namespace EyeProtect.Models
{
    public partial class OptionsModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BreakProgressListVisibility))]
        private Settings.OptionsModel data;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(VersionLink))]
        private string version;

        public string VersionLink => "https://github.com/Jack251970/EyeProtect/releases/tag/" + Version;
        public AppInfo.AppInfo SelectedItem { get; set; }
        public List<ComboxModel> Languages { get; set; }

        public bool IsBreakProgressList
        {
            get => Data?.Behavior.IsBreakProgressList ?? false;
            set
            {
                if (Data != null)
                {
                    Data.Behavior.IsBreakProgressList = value;
                    OnPropertyChanged(nameof(IsBreakProgressList));
                    OnPropertyChanged(nameof(BreakProgressListVisibility));
                }
            }
        }
        public Visibility BreakProgressListVisibility => (Data?.Behavior.IsBreakProgressList ?? false) ? Visibility.Visible : Visibility.Collapsed;
    }
}
