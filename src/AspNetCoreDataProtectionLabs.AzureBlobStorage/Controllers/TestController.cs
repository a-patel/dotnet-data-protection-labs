#region Imports
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
#endregion

namespace AspNetCoreDataProtectionLabs.AzureBlobStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        #region Members

        private readonly IDataProtector _dataProtector;

        public string ReadValue { get; set; }
        public string WrittenValue { get; set; }

        #endregion

        #region Ctor

        public TestController(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtector = dataProtectionProvider.CreateProtector("Test");
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("test")]
        public IActionResult Test()
        {
            const string CookieName = "DataProtectionTestCookie";

            if (Request.Cookies.TryGetValue(CookieName, out var cookieValue))
            {
                /*
                 Read value from cookie: This same value should show up if you refresh the page.
                 It should also show up even if you restart the app process since the keys are persisted in Azure Storage.
                 */
                ReadValue = Encoding.UTF8.GetString(_dataProtector.Unprotect(Convert.FromBase64String(cookieValue)));
            }
            else
            {
                /*
                 Wrote value to cookie: This same value should show up if you refresh the page.
                 It should also show up even if you restart the app process since the keys are persisted in Azure Storage.
                 */
                var value = $"Data written at {DateTime.Now}";
                cookieValue = Convert.ToBase64String(_dataProtector.Protect(Encoding.UTF8.GetBytes(value)));
                Response.Cookies.Append(CookieName, cookieValue, new CookieOptions
                {
                    IsEssential = true
                });
                WrittenValue = value;
            }

            return Ok(new { ReadValue, WrittenValue });
        }

        #endregion
    }
}
