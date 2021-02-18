using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seasharpcustomerbooking.Models
{
    public class RoomModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int Floor { get; set; }
        public int RoomNumber { get; set; }
        public List<CategoryModel> Category { get; set; }

    }
}
