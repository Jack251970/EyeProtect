using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using EyeProtect.Core;
using EyeProtect.Core.Service;
using EyeProtect.Models.AppInfo;
using EyeProtect.Models.Settings;
using EyeProtect.Views;

namespace EyeProtect.ViewModels
{
    public partial class OptionsViewModel : ObservableObject, IDisposable
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BreakProgressListVisibility))]
        private OptionsModel data;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(VersionLink))]
        private string version;

        public string VersionLink => "https://github.com/Jack251970/EyeProtect/releases/tag/" + Version;
        public AppInfo SelectedItem { get; set; }
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

        private readonly ConfigService config;
        private readonly MainService mainService;
        private bool _disposed = false;

        public OptionsViewModel()
        {
            config = Ioc.Default.GetRequiredService<ConfigService>();
            mainService = Ioc.Default.GetRequiredService<MainService>();
            Data = config.options;
            Languages = Ioc.Default.GetRequiredService<SystemResourcesService>().Languages;
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.');
            Version = version[0] + "." + version[1] + "." + version[2];
        }

        /// <summary>
        /// Subscribe to property changes on the settings models for auto-save
        /// </summary>
        public void SubscribeToPropertyChanges()
        {
            // Subscribe to General settings changes
            Data.General.PropertyChanged += OnGeneralPropertyChanged;

            // Subscribe to Style settings changes
            Data.Style.PropertyChanged += OnStylePropertyChanged;

            // Subscribe to Behavior settings changes
            Data.Behavior.PropertyChanged += OnBehaviorPropertyChanged;

            // Subscribe to collection changes for BreakProgressList
            Data.Behavior.BreakProgressList.CollectionChanged += OnBreakProgressListChanged;
        }

        /// <summary>
        /// Unsubscribe from property changes
        /// </summary>
        private void UnsubscribeFromPropertyChanges()
        {
            // Unsubscribe to General settings changes
            Data.General.PropertyChanged -= OnGeneralPropertyChanged;

            // Unsubscribe to Style settings changes
            Data.Style.PropertyChanged -= OnStylePropertyChanged;

            // Unsubscribe to Behavior settings changes
            Data.Behavior.PropertyChanged -= OnBehaviorPropertyChanged;

            // Unsubscribe to collection changes for BreakProgressList
            Data.Behavior.BreakProgressList.CollectionChanged -= OnBreakProgressListChanged;
        }

        private void OnGeneralPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnSettingChanged(e.PropertyName);
        }

        private void OnStylePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnSettingChanged(e.PropertyName);
        }

        private void OnBehaviorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnSettingChanged(e.PropertyName);
        }

        private void OnBreakProgressListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnSettingChanged("BreakProgressList");
        }

        /// <summary>
        /// Handle setting changes and auto-save
        /// </summary>
        /// <param name="propertyName">The name of the property that changed</param>
        private void OnSettingChanged(string propertyName)
        {
            // Save the configuration
            config.Save();

            // Handle specific property changes
            if (propertyName == nameof(Data.General.Startup))
            {
                StartupHelper.SetStartup(config.options.General.Startup);
            }
            else if (propertyName == nameof(Data.General.WarnTime))
            {
                mainService.SetWarnTime(config.options.General.WarnTime);
            }
        }

        /// <summary>
        /// 打开窗口命令
        /// </summary>
        /// <param name="obj"></param>
        [RelayCommand]
        private void OpenWindow(object obj)
        {
            var window = obj.ToString();
            WindowManager.CreateWindowInScreen(window);
            WindowManager.Show(window);
        }

        /// <summary>
        /// 移除进程命令
        /// </summary>
        /// <param name="obj"></param>
        [RelayCommand]
        private void RemoveBreackProcess(object obj)
        {
            Data.Behavior.BreakProgressList.Remove(SelectedItem);
        }

        /// <summary>
        /// 添加跳过进程命令
        /// </summary>
        /// <param name="obj"></param>
        [RelayCommand]
        private void AddBreackProcess(Button button)
        {
            // Show AppSelectionDialog and select an application
            var dialog = new AppSelectionWindow(Data.Behavior.BreakProgressList)
            {
                Owner = Window.GetWindow(button)
            };
            
            var result = dialog.ShowDialog();
            if (result == true && dialog.ViewModel.SelectedApp != null)
            {
                var addedApp = dialog.ViewModel.SelectedApp;
                
                // Check if app already exists in the list (additional safety check)
                var existingApp = Data.Behavior.BreakProgressList.FirstOrDefault(a => 
                    a.Equals(addedApp));
                
                if (existingApp == null)
                {
                    Data.Behavior.BreakProgressList.Add(addedApp);
                    SelectedItem = null; // Clear selection after adding
                }
            }
        }

        [RelayCommand]
        private void ShowWindow(object obj)
        {
            WindowManager.CreateWindowInScreen(obj.ToString());
            WindowManager.Show(obj.ToString());
        }

        /// <summary>
        /// Dispose method to clean up event subscriptions
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                UnsubscribeFromPropertyChanges();
                _disposed = true;
            }
        }
    }
}
