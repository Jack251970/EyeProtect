using System.Windows;
using EyeProtect.ViewModels;

namespace EyeProtect.Views
{
    public partial class AppSelectionWindow : Window
    {
        public AppSelectionViewModel ViewModel { get; }

        public AppSelectionWindow()
        {
            InitializeComponent();
            ViewModel = new AppSelectionViewModel();
            DataContext = ViewModel;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = ViewModel.SelectedApp != null;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void AppListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ViewModel.SelectedApp != null)
            {
                DialogResult = true;
                Close();
            }
        }
    }
}
