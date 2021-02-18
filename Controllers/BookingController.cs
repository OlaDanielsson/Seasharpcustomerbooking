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
            List<CategoryModel> Categorylist = new List<CategoryModel>();

            var categoryresponse = await ApiConnection.ApiClient.GetAsync("CategoryModels");
            string jsoncategoryresponse = await categoryresponse.Content.ReadAsStringAsync();
            Categorylist = JsonConvert.DeserializeObject<List<CategoryModel>>(jsoncategoryresponse);
            ////La till den raden här under
            ViewData["CategoryId"] = new SelectList(Categorylist, "Id", "Description");

            List<RoomModel> roomlist = new List<RoomModel>();

            var roomresponse = await ApiConnection.ApiClient.GetAsync("RoomModels");
            string jsonroomresponse = await roomresponse.Content.ReadAsStringAsync();
            roomlist = JsonConvert.DeserializeObject<List<RoomModel>>(jsonroomresponse);

            List<BookingModel> bookinglist = new List<BookingModel>();

            var bookingresponse = await ApiConnection.ApiClient.GetAsync("Bookingmodels");
            string jsonbookingresponse = await bookingresponse.Content.ReadAsStringAsync();
            bookinglist = JsonConvert.DeserializeObject<List<BookingModel>>(jsonbookingresponse);

            List<RoomModel> qualifiedrooms = new List<RoomModel>();
            List<RoomModel> corcatroom = new List<RoomModel>();

            foreach (var item in roomlist) //loopar igenom listan room
            {
                if (item.CategoryId == ViewBag.CategoryId) //om något item i listan room har samma categori Id som användarinmatningen så går den vidare
                {
                    corcatroom.Add(item);    
                }
            }
            List<RoomModel> CompareList = new List<RoomModel>();
            foreach (var item in corcatroom) //loopar igenom bokningslistan
            {
                foreach (var element in bookinglist)
                {
                    if (element.RoomId == item.Id)
                    {
                        int start = int.Parse(DateTime.Parse(element.StartDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1));
                        int end = int.Parse(DateTime.Parse(element.EndDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1));
                        int bookingstart = int.Parse(DateTime.Parse(booking.StartDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1));
                        int bookingend = int.Parse(DateTime.Parse(booking.EndDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1));


                        if ((start < bookingstart && end < bookingstart) || (start > bookingend && end > bookingend)) // testar tidsintervallet, finns ingen bokning lägg till rum i listan
                        {
                            qualifiedrooms.Add(item);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        CompareList.Add(item);  
                    }
                    if (bookinglist.Count == CompareList.Count)
                    {
                        qualifiedrooms.Add(item);
                        break;
                    }

                }
                CompareList.Clear();
            }

            // skapa bokning av rum som ligger först i listan Qualifiedrooms
            int roomid = qualifiedrooms.First().Id;

            HttpResponseMessage responseBooking = ApiConnection.ApiClient.GetAsync("BookingModels/" + roomid.ToString()).Result;
            return View(new BookingModel());
        }
    }
}
