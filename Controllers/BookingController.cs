using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create(int Id, int GuestId)
        {
            try
            {
                List<CategoryModel> categoryList = await ApiConnection.GetCategoryList();

                ViewData["Desc"] = new SelectList(categoryList, "Id", "Description"); //för att fixa viewdata
                HttpResponseMessage responseRoom = ApiConnection.ApiClient.GetAsync("CategoryModels/").Result;

                return View(new BookingModel());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(BookingModel booking)
        {
            try
            {
                List<CategoryModel> categoryList = await ApiConnection.GetCategoryList();
                List<RoomModel> roomList = await ApiConnection.GetRoomList();
                List<BookingModel> bookingList = await ApiConnection.GetBookingList();

                List<RoomModel> qualifiedrooms = new List<RoomModel>();
                List<RoomModel> corcatroom = new List<RoomModel>();

                int bookingstart = int.Parse(DateTime.Parse(booking.StartDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1)); //parsar datetime till int
                int bookingend = int.Parse(DateTime.Parse(booking.EndDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1)); //parsar datetime till int
                int dateToday = int.Parse(DateTime.Parse(DateTime.Today.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1));

                if (bookingstart < bookingend && bookingstart >= dateToday) //kollar så bokningens start datum inte är efter slutdatum
                {
                    corcatroom.AddRange(from item in roomList
                                        where item.CategoryId == booking.CategoryId
                                        select item);

                    List<BookingModel> corcatbooking = new List<BookingModel>();

                    //Används inte än men kan tas tag i imorgon.
                    //BookingHandler.GetCorCatBookingList(bookingList, corcatbooking, booking.CategoryId); //Hämtar lista med enbart bokningar av korrekta kategori.
                    //---------------------------------------------------------------------------------------------------------------------------------------------

                    BookingHandler.RoomAvailableCheckV2(bookingList, corcatroom, bookingstart, bookingend);

                    if (corcatroom.Count > 0) //kollar ifall det finns tillgängliga rum
                    {
                        var room = corcatroom.First();
                        BookingModel finalBooking = BookingHandler.SetFinalBooking(booking, room);

                        var postTask = ApiConnection.ApiClient.PostAsJsonAsync<BookingModel>("BookingModels", finalBooking);
                        postTask.Wait();

                        var result = postTask.Result;
                        return RedirectToAction("Confirmation", "Booking");
                    }


                    else
                    {
                        ViewData["norooms"] = "Det finns inga lediga rum av din preferenser";
                        await ViewbagCategory();
                        return View(new BookingModel());
                    }
                }
                else
                {
                    ViewData["wrongtime"] = "Vänligen fyll i en korrekt tid. Slutdatumet måste vara senare än startdatumet.";
                    await ViewbagCategory();
                    return View(new BookingModel());
                }
            }
            catch (Exception ex)
            {
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
