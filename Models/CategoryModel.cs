using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Seasharpcustomerbooking.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Image { get; set; } //lista av bilder som är direkt kopplat till den kategorin
        public int NumberOfBeds { get; set; }
        public int Price { get; set; } // räcker int eller ska man ta en annna typ av variabel?
        public string Path { get; set; }
        [NotMapped]
        public Microsoft.AspNetCore.Http.IFormFile uploadedimg { get; set; } //för att lägga till bilder
    }
}
