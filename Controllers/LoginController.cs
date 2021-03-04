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
            GuestModel loginOk = null;// = new User();
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                                                                
                                                                //API-adress ej funktionell
                using (var response = await httpClient.PostAsync("http://193.10.202.78/GuestAPI/api/Login", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    loginOk = JsonConvert.DeserializeObject<GuestModel>(apiResponse);
                }
            }

            if (loginOk.Id > 0)
            {
                await SetUserAuthenticated(loginOk);

                //Den ska inte vara med. Bara för att visa att det fungerar
                return Redirect("~/Home/Index/");
            }
            else
            {
                ViewData["failedlogin"] = "Inloggningen misslyckades";
                return View();
            }
        }

        private async Task SetUserAuthenticated(GuestModel loginOk)
        {
            //Inloggningsuppgifter stämmer, admin loggas in
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, loginOk.Status.ToString()));

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
