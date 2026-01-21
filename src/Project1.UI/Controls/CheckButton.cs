using System.Windows;
using System.Windows.Controls;

namespace Project1.UI.Controls
{
    public class CheckButton : Control
    {

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Checked
        {
            get => (bool)GetValue(CheckedProperty); set => SetValue(CheckedProperty, value);
        }
        public static readonly DependencyProperty CheckedProperty =
            DependencyProperty.Register("Checked", typeof(bool), typeof(CheckButton), new PropertyMetadata(false));

        /// <summary>
        /// 内容
        /// </summary>
        public string Content
        {
            get => (string)GetValue(ContentProperty); set => SetValue(ContentProperty, value);
        }
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(string), typeof(CheckButton));

        public CheckButton()
        {
            DefaultStyleKey = typeof(CheckButton);
        }


    }
}
