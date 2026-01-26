using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using ProjectEye.Core;
using ProjectEye.Core.Service;
using ProjectEye.Models;
using ProjectEye.Views;

namespace ProjectEye.ViewModels
{
    public partial class OptionsViewModel : IDisposable
    {
        public OptionsModel Model { get; set; }

        private readonly ConfigService config;
        private readonly MainService mainService;
        private bool _disposed = false;

        public OptionsViewModel(ConfigService config,
            MainService mainService,
            SystemResourcesService systemResources,
            SoundService sound)
        {
            this.config = config;
            this.mainService = mainService;
            Model = new OptionsModel
            {
                Data = config.options,
                Languages = systemResources.Languages
            };

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.');
            Model.Version = version[0] + "." + version[1] + "." + version[2];

            // Subscribe to property changes for auto-save
            SubscribeToPropertyChanges();
        }

        /// <summary>
        /// Subscribe to property changes on the settings models for auto-save
        /// </summary>
        private void SubscribeToPropertyChanges()
        {
            if (Model.Data != null)
            {
                // Subscribe to General settings changes
                Model.Data.General?.PropertyChanged += OnGeneralPropertyChanged;

                // Subscribe to Style settings changes
                Model.Data.Style?.PropertyChanged += OnStylePropertyChanged;

                // Subscribe to Behavior settings changes
                if (Model.Data.Behavior != null)
                {
                    Model.Data.Behavior.PropertyChanged += OnBehaviorPropertyChanged;
                    
                    // Subscribe to collection changes for BreakProgressList
                    Model.Data.Behavior.BreakProgressList.CollectionChanged += OnBreakProgressListChanged;
                }
            }
        }

        /// <summary>
        /// Unsubscribe from property changes
        /// </summary>
        private void UnsubscribeFromPropertyChanges()
        {
            if (Model.Data != null)
            {
                if (Model.Data.General != null)
                {
                    Model.Data.General.PropertyChanged -= OnGeneralPropertyChanged;
                }

                if (Model.Data.Style != null)
                {
                    Model.Data.Style.PropertyChanged -= OnStylePropertyChanged;
                }

                if (Model.Data.Behavior != null)
                {
                    Model.Data.Behavior.PropertyChanged -= OnBehaviorPropertyChanged;
                    Model.Data.Behavior.BreakProgressList.CollectionChanged -= OnBreakProgressListChanged;
                }
            }
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
            if (_disposed)
            {
                return;
            }

            // Save the configuration
            config.Save();

            // Handle specific property changes
            if (propertyName == nameof(Model.Data.General.Startup))
            {
                StartupHelper.SetStartup(config.options.General.Startup);
            }
            else if (propertyName == nameof(Model.Data.General.WarnTime))
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
            Model.Data.Behavior.BreakProgressList.Remove(Model.SelectedItem);
        }

        /// <summary>
        /// 添加跳过进程命令
        /// </summary>
        /// <param name="obj"></param>
        [RelayCommand]
        private void AddBreackProcess(object obj)
        {
            // Show AppSelectionDialog and select an application
            var dialog = new AppSelectionWindow
            {
                Owner = Application.Current.MainWindow
            };
            
            var result = dialog.ShowDialog();
            if (result == true && dialog.ViewModel.SelectedApp != null)
            {
                var addedApp = dialog.ViewModel.SelectedApp;
                
                // Check if app already exists in the list
                var existingApp = Model.Data.Behavior.BreakProgressList.FirstOrDefault(a => 
                    a.DefaultDisplayName == addedApp.DefaultDisplayName);
                
                if (existingApp == null)
                {
                    Model.Data.Behavior.BreakProgressList.Add(addedApp);
                    Model.SelectedItem = null; // Clear selection after adding
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
