using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Seasharpcustomerbooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Seasharpcustomerbooking.Controllers
{
    [Authorize]

    public class BookingController : Controller
    {
        private readonly ILogger<BookingController> _logger;

        public BookingController(ILogger<BookingController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                var str = HttpContext.Session.GetString("GuestSession");              

                if(String.IsNullOrEmpty(str))
                {
                    SignOut();
                    return RedirectToAction("Index", "Login", new {msg = "Något gick fel, logga in igen"});
                }

                var obj = JsonConvert.DeserializeObject<GuestModel>(str);
                ViewBag.GuestBag = obj.Firstname + " " + obj.Lastname;

                List<CategoryModel> categoryList = await ApiConnection.GetCategoryList();
                ViewData["Desc"] = new SelectList(categoryList, "Id", "Description"); //för att fixa viewdata

                ViewData["Price"] = categoryList; //för att fixa viewdata

                HttpResponseMessage responseRoom = ApiConnection.ApiClient.GetAsync("CategoryModels/").Result;
                BookingModel bookingmodel = new BookingModel();
                DateTime today;
                today = DateTime.Today;
                bookingmodel.StartDate = today;
                bookingmodel.EndDate = today;

                return View(bookingmodel);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Couldn't book a room.");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, [Bind("CategoryId, StartDate, EndDate, GuestId")] BookingModel booking)
        {
            try
            {
                var str = HttpContext.Session.GetString("GuestSession");
                var obj = JsonConvert.DeserializeObject<GuestModel>(str);

                List<RoomModel> roomList = await ApiConnection.GetRoomList();
                List<BookingModel> bookingList = await ApiConnection.GetBookingList();

                List<RoomModel> qualifiedrooms = new List<RoomModel>();
                List<RoomModel> corcatroom = new List<RoomModel>();

                int bookingstart = int.Parse(DateTime.Parse(booking.StartDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1)); //parsar datetime till int
                int bookingend = int.Parse(DateTime.Parse(booking.EndDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1)); //parsar datetime till int
                int dateToday = int.Parse(DateTime.Parse(DateTime.Today.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1)); //parsar dagens datum till int

                if (bookingstart < bookingend && bookingstart >= dateToday) //kollar så bokningens start datum inte är efter slutdatum
                {
                    corcatroom.AddRange(from item in roomList
                                        where item.CategoryId == booking.CategoryId
                                        select item);

                    List<BookingModel> corcatbooking = new List<BookingModel>();

                    BookingHandler.RoomAvailableCheckV2(bookingList, corcatroom, bookingstart, bookingend);

                    if (corcatroom.Count > 0) //kollar ifall det finns tillgängliga rum
                    {
                        var room = corcatroom.First();
                        BookingModel finalBooking = BookingHandler.SetFinalBooking(booking, room, obj);

                        var postTask = ApiConnection.ApiClient.PostAsJsonAsync<BookingModel>("BookingModels", finalBooking);
                        postTask.Wait();

                        var result = postTask.Result;
                        return RedirectToAction("Confirmation", "Booking");
                    }

                    else
                    {
                        ViewBag.GuestBag = obj.Firstname + " " + obj.Lastname;
                        ViewData["norooms"] = "Det finns inga lediga rum av din preferenser";
                        await ViewbagCategory();
                        return View(new BookingModel());
                    }
                }
                else
                {
                    if(bookingstart < dateToday)
                    {
                        ViewData["wrongtime"] = "Om du inte har en tidsmaskin, kan du endast boka dagens datum och framåt.";
                    }
                    else
                    {
                        ViewData["wrongtime"] = "Vänligen fyll i en korrekt tid. Slutdatumet måste vara senare än startdatumet.";
                    }

                    ViewBag.GuestBag = obj.Firstname + " " + obj.Lastname;
                    await ViewbagCategory();
                    return View(new BookingModel());
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Couldn't book a room.");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return View();
            }
        }
        private async Task ViewbagCategory()
        {
            List<CategoryModel> categoryList = await ApiConnection.GetCategoryList();
            ViewData["Desc"] = new SelectList(categoryList, "Id", "Description"); //för att fixa viewdata
        }
        public ActionResult Confirmation()
        {
            return View();
        }
    }
}
