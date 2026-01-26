using System;
using System.Windows;
using ProjectEye.ViewModels;

namespace ProjectEye.Views
{
    /// <summary>
    /// OptionsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsWindow()
        {
            InitializeComponent();
            Closed += OnClosed;
        }

        private void OnClosed(object sender, EventArgs e)
        {
            // Clean up ViewModel resources
            if (DataContext is OptionsViewModel viewModel)
            {
                viewModel.Dispose();
            }
            Closed -= OnClosed;
        }
    }
}
