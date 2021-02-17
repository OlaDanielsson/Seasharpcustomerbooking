using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Seasharpcustomerbooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Seasharpcustomerbooking.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            try
            {
                return View(new BookingModel());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return View();
            }

        }
        [HttpPost]
        public async Task<IActionResult> Create(BookingModel booking) //detta är för bokning av rum för kunder
        {
            List<CategoryModel> Category = new List<CategoryModel>();

            var response2 = await ApiConnection.ApiClient.GetAsync("CategoryModels");
            string jsonresponse2 = await response2.Content.ReadAsStringAsync();
            Category = JsonConvert.DeserializeObject<List<CategoryModel>>(jsonresponse2);
            ////La till den raden här under
            ViewData["CategoryId"] = new SelectList(Category, "Id", "Description");

            List<RoomModel> room = new List<RoomModel>();

            var response = await ApiConnection.ApiClient.GetAsync("RoomModels");
            string jsonresponse = await response.Content.ReadAsStringAsync();
            room = JsonConvert.DeserializeObject<List<RoomModel>>(jsonresponse);

            List<BookingModel> bookinglist = new List<BookingModel>();

            var response1 = await ApiConnection.ApiClient.GetAsync("Bookingmodels");
            string jsonresponse1 = await response.Content.ReadAsStringAsync();
            bookinglist = JsonConvert.DeserializeObject<List<BookingModel>>(jsonresponse);

            List<RoomModel> qualifiedrooms = new List<RoomModel>();
            foreach (var item in room) //Detta skapar en ny lista med bokningsbara rum
            {
                if (item.CategoryId == 5/*booking.CategoryId*/)
                {
                    foreach (var element in bookinglist)
                    {
                        int start = int.Parse(DateTime.Parse(element.StartDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1));
                        int end = int.Parse(DateTime.Parse(element.StartDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1));
                        int bookingstart = int.Parse(DateTime.Parse(booking.StartDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1));
                        int bookingend = int.Parse(DateTime.Parse(booking.StartDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1));


                        if (start >= bookingstart || end >= bookingstart)
                        {
                            if (start <= bookingend || end <= bookingend) //går inte in här
                            {

                                qualifiedrooms.Add(item);

                            }
                        }
                    }
                }
            }
            // skapa bokning av rum som ligger först i listan Qualifiedrooms
            int roomid = qualifiedrooms.First().Id;

            HttpResponseMessage responseBooking = ApiConnection.ApiClient.GetAsync("BookingModels/" + roomid.ToString()).Result;
            return View(new BookingModel());
        }
    }
}
