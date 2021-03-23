using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seasharpcustomerbooking.Models
{
    public class BookingModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RoomId { get; set; }
        public int GuestId { get; set; }
        public int CategoryId { get; set; }
        public string CatDescription { get; set; }
        public int Price { get; set; }
    }
}
