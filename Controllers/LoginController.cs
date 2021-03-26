using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<LoginController> logger;

        public LoginController(ILogger<LoginController> logger)
        {
            this.logger = (ILogger<LoginController>)logger;
        }
        public IActionResult Index(string msg)
        {
            ViewBag.Errormsg = msg;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginModel login)
        {

            try
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

                if (Guest != null)
                {
                    await SetUserAuthenticated(Guest);

                    var str = JsonConvert.SerializeObject(Guest);
                    HttpContext.Session.SetString("GuestSession", str);

                    return Redirect("~/Booking/Create/");
                }
                else
                {
                    ViewData["failedlogin"] = "Inloggningen misslyckades";
                    return View();
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning("Couldn't login.");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return View();
            }
        }

        private async Task SetUserAuthenticated(GuestModel Guest)
        {
            try
            {
                //Inloggningsuppgifter stämmer, admin loggas in
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, Guest.Id.ToString()));

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity));
            }
            catch (Exception ex)
            {
                logger.LogWarning("Couldn't authenticate.");
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public async Task<IActionResult> SignOut(LoginModel login)
        {

            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                logger.LogWarning("Couldn't signout.");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
