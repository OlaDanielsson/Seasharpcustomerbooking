using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seasharpcustomerbooking.Models
{
    public class GuestModel
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Street_Adress { get; set; }
        public int PostalCode { get; set; }
        public string City { get; set; }
        public int Phonenumber { get; set; }
        public string E_Mail { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Password { get; set; }
    }
}
