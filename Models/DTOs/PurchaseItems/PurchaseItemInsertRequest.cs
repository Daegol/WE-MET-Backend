using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.PurchaseItems
{
    public class PurchaseItemInsertRequest
    {
        public double Amount { get; set; }
        public double Contamination { get; set; }
        public int SubCategoryId { get; set; }
        public int PurchaseId { get; set; }
    }
}
