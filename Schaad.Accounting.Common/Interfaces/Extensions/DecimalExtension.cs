using System.Globalization;
using System.Text;

namespace Schaad.Accounting.Common.Extensions
{
    public static class DecimalExtension
    {
        public static string ToFormattedString(this decimal value)
        {
            var culture = new CultureInfo("de-CH");
            culture.NumberFormat.NumberGroupSeparator = "'";
            return value.ToString("#,0.00", culture);
        }
    }
}