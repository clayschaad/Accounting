using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Schaad.Accounting
{
    public static class Helper
    {
        public static void SetSession(HttpContext context, string key, string value)
        {
            context.Session.Set(key, Encoding.UTF8.GetBytes(value));
        }

        public static string GetSession(HttpContext context, string key)
        {
            byte[] value;
            context.Session.TryGetValue(key, out value);
            return value != null ? Encoding.UTF8.GetString(value) : "";
        }
    }
}