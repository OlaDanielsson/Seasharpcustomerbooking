using Seasharpcustomerbooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seasharpcustomerbooking
{
    public class BookingHandler
    {
        public static void RoomAvailableCheckV2(List<BookingModel> bookingList, List<RoomModel> corcatroom, int bookingstart, int bookingend)
        {
            for (int i = 0; i < corcatroom.Count; i++)
            {
                foreach (var element in bookingList)
                {
                    if (element.RoomId == corcatroom[i].Id)
                    {
                        int start = int.Parse(DateTime.Parse(element.StartDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1));
                        int end = int.Parse(DateTime.Parse(element.EndDate.ToString()).ToString().Remove(10, 9).Remove(4, 1).Remove(6, 1));

                        if ((bookingstart >= start && bookingstart <= end) || (bookingend >= start && bookingend <= end) || (bookingstart < start && bookingend > end))
                        {
                            corcatroom.Remove(corcatroom[i]);
                            i--;
                            break;
                        }
                    }
                }
            }
        }
        public static BookingModel SetFinalBooking(BookingModel booking, RoomModel room)
        {
            BookingModel finalBooking = new BookingModel();
            finalBooking.Id = booking.Id;
            finalBooking.StartDate = booking.StartDate;
            finalBooking.EndDate = booking.EndDate;
            finalBooking.RoomId = room.Id;
            finalBooking.GuestId = booking.GuestId;
            return finalBooking;
        }
    }
}
