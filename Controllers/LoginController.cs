using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Seasharpcustomerbooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Seasharpcustomerbooking.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginModel login)
        {
            GuestModel Guest = null;// = new User();
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");

                                                                   
                using (var response = await httpClient.PostAsync("https://informatik8.ei.hv.se/GuestAPI/api/Login", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    Guest = JsonConvert.DeserializeObject<GuestModel>(apiResponse);
                }
            }

            if (Guest.Id > 0)
            {
                await SetUserAuthenticated(Guest);

                
                return Redirect("~/Booking/Create/" + Guest.Id);
            }
            else
            {
                ViewData["failedlogin"] = "Inloggningen misslyckades";
                return View();
            }
        }

        private async Task SetUserAuthenticated(GuestModel Guest)
        {
            //Inloggningsuppgifter stämmer, admin loggas in
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, Guest.Id.ToString()));

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
        }

        public async Task<IActionResult> SignOut(LoginModel login)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
