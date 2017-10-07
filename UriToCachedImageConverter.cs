using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MiniPlayer
{
    public class UriToCachedImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;
            if (!string.IsNullOrEmpty(value?.ToString()))
            {
                try
                {
                    var bi = new BitmapImage();
                    bi.BeginInit();
                    var filename = (string) value;
                    bi.UriSource = new Uri(filename);

                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.EndInit();
                    if (bi.CanFreeze)
                    {
                        bi.Freeze();
                    }

                    return bi;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException("Two way conversion is not supported.");
        }
    }
}