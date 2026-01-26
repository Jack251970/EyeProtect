using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace ProjectEye.Models.Settings
{
    [XmlRootAttribute("Style")]
    public class StyleModel : INotifyPropertyChanged
    {
        private ComboxModel language = new ComboxModel() { DisplayName = "English", Value = "en" };
        private string tipContent = string.Empty;

        /// <summary>
        /// 语言
        /// </summary>
        public ComboxModel Language
        {
            get => language;
            set => SetProperty(ref language, value);
        }

        /// <summary>
        /// 提醒内容
        /// </summary>
        public string TipContent
        {
            get => tipContent;
            set => SetProperty(ref tipContent, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
