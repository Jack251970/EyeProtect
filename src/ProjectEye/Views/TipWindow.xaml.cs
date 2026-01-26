using System.Windows;
using System.Windows.Interop;
using System;
using System.Runtime.InteropServices;

namespace ProjectEye.Views
{
    /// <summary>
    /// TipWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TipWindow : Window
    {
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("kernel32.dll")]
        private static extern uint GetLastError();

        public TipWindow()
        {
            InitializeComponent();
            Loaded += TipWindow_Loaded;
            Unloaded += TipWindow_Unloaded;
        }

        private void TipWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Subscribe to DataContext changes to handle IsThruWindow property
            DataContextChanged += TipWindow_DataContextChanged;
            UpdateWindowTransparency();
        }

        private void TipWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            // Clean up event handlers to prevent memory leaks
            DataContextChanged -= TipWindow_DataContextChanged;
            if (DataContext is ViewModels.TipViewModel viewModel)
            {
                viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }
        }

        private void TipWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // When DataContext changes, re-subscribe to property changes
            if (e.OldValue is ViewModels.TipViewModel oldViewModel)
            {
                oldViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            if (e.NewValue is ViewModels.TipViewModel newViewModel)
            {
                newViewModel.PropertyChanged += ViewModel_PropertyChanged;
                UpdateWindowTransparency();
            }
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModels.TipViewModel.IsThruWindow))
            {
                UpdateWindowTransparency();
            }
        }

        private void UpdateWindowTransparency()
        {
            if (DataContext is ViewModels.TipViewModel viewModel)
            {
                // Get window handle using WindowInteropHelper
                var hwnd = new WindowInteropHelper(this).Handle;
                
                // Skip if window handle is not yet created
                if (hwnd == IntPtr.Zero)
                {
                    return;
                }

                try
                {
                    // Clear last error before the API call
                    System.Runtime.InteropServices.Marshal.SetLastPInvokeError(0);
                    var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                    
                    // Check if GetWindowLong failed
                    var error = GetLastError();
                    if (extendedStyle == 0 && error != 0)
                    {
                        // GetWindowLong failed, log and return
                        System.Diagnostics.Debug.WriteLine($"GetWindowLong failed with error code: {error}");
                        return;
                    }

                    int newStyle;
                    if (viewModel.IsThruWindow)
                    {
                        // Enable mouse passthrough
                        newStyle = extendedStyle | WS_EX_TRANSPARENT;
                    }
                    else
                    {
                        // Disable mouse passthrough
                        newStyle = extendedStyle & ~WS_EX_TRANSPARENT;
                    }

                    // Clear last error before the API call
                    System.Runtime.InteropServices.Marshal.SetLastPInvokeError(0);
                    SetWindowLong(hwnd, GWL_EXSTYLE, newStyle);
                    
                    // Check if SetWindowLong failed
                    error = GetLastError();
                    if (error != 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"SetWindowLong failed with error code: {error}");
                    }
                }
                catch (Exception ex)
                {
                    // Log any exceptions during window style modification
                    System.Diagnostics.Debug.WriteLine($"Error updating window transparency: {ex.Message}");
                }
            }
        }
    }
}
