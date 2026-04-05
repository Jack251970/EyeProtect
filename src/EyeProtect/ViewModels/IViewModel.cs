using System.Windows;

namespace EyeProtect.ViewModels
{
    internal interface IViewModel
    {
        Window WindowInstance { get; set; }
        event ViewModelEventHandler ChangedEvent;
        void OnChanged();
        void BeforeShown();
    }
}
