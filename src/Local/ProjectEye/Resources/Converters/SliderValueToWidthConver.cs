using System;
using System.Globalization;
using System.Windows.Data;

namespace ProjectEye.Resources.Converters
{
    /// <summary>
    /// 滑块值转换为宽度
    /// </summary>
    public class SliderValueToWidthConver : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var sliderValue = (double)values[0];
            var sliderMaxValue = (double)values[1];
            var sliderWidth = (double)values[2];
            double result = 0;
            if (sliderWidth > 0)
            {
                result = sliderValue / sliderMaxValue * sliderWidth;
            }
            return result;
        }

        public object ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
