using Models.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.SaleItem
{
    public class SaleItemDto
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public SubCategoryDto SubCategory { get; set; }
    }
}
