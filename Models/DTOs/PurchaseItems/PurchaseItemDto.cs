using Models.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.PurchaseItems
{
    public class PurchaseItemDto
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public double Contamination { get; set; }
        public SubCategoryDto SubCategory { get; set; }
    }
}
