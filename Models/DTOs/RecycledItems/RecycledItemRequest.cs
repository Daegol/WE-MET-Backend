using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.RecycledItems
{
    public class RecycledItemRequest
    {
        public double Amount { get; set; }
        public int SubCategoryId { get; set; }
    }
}
