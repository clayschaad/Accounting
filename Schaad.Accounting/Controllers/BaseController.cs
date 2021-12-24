using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Schaad.Accounting.Controllers
{
    /// <summary>
    /// Base of all controllers
    /// </summary>
    public class BaseController : Microsoft.AspNetCore.Mvc.Controller
    {
        protected void SetSession(string key, string value)
        {
            Helper.SetSession(HttpContext, key, value);
        }

        protected async Task<string> Get(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    using (HttpContent content = response.Content)
                    {
                        // ... Read the string.
                        string result = await content.ReadAsStringAsync();
                        return result;
                    }
                }
            }
        }
    }
}