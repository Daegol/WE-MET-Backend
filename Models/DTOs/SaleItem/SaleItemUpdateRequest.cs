using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.SaleItem
{
    public class SaleItemUpdateRequest
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public int SubCategoryId { get; set; }
        public int SaleId { get; set; }
    }
}
