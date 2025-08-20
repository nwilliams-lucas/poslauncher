using System.Globalization;

namespace POSLauncher.Maui.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                if (status.Contains("✓") || status.Contains("Successfully") || status.Contains("Running"))
                {
                    return Colors.Green;
                }
                else if (status.Contains("✗") || status.Contains("Failed") || status.Contains("Error"))
                {
                    return Colors.Red;
                }
                else if (status.Contains("Starting") || status.Contains("Checking"))
                {
                    return Colors.Orange;
                }
            }
            
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}