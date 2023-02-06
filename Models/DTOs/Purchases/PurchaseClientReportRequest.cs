using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Purchases
{
    public class PurchaseClientReportRequest
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int ClientId { get; set; }
    }
}
