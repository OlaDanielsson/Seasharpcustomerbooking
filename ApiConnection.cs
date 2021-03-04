using Newtonsoft.Json;
using Seasharpcustomerbooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Seasharpcustomerbooking
{
    public static class ApiConnection
    {
        public static HttpClient ApiClient = new HttpClient();

        static ApiConnection()
        {
            ApiClient.BaseAddress = new Uri("http://193.10.202.81/ConnectingAPI/");
            ApiClient.DefaultRequestHeaders.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public static async Task<List<CategoryModel>> GetCategoryList()
        {
            List<CategoryModel> categoryList = new List<CategoryModel>();
            var response = await ApiClient.GetAsync("CategoryModels");
            string jsonresponse = await response.Content.ReadAsStringAsync();
            categoryList = JsonConvert.DeserializeObject<List<CategoryModel>>(jsonresponse);
            return categoryList;
        }

        public static async Task<List<BookingModel>> GetBookingList(/*List<BookingModel> bookingList*/)
        {
            List<BookingModel> bookingList = new List<BookingModel>();
            var response = await ApiClient.GetAsync("BookingModels");
            string jsonresponse = await response.Content.ReadAsStringAsync();
            bookingList = JsonConvert.DeserializeObject<List<BookingModel>>(jsonresponse);
            return bookingList;
        }

        public static async Task<List<RoomModel>> GetRoomList()
        {
            List<RoomModel> roomList = new List<RoomModel>();
            var response = await ApiClient.GetAsync("RoomModels");
            string jsonresponse = await response.Content.ReadAsStringAsync();
            roomList = JsonConvert.DeserializeObject<List<RoomModel>>(jsonresponse);
            return roomList;
        }
    }
}
