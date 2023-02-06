using Models.DTOs.Client;
using Models.DTOs.PurchaseItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Purchases
{
    public class PurchaseUpdateRequest
    {
        public int Id { get; set; }
        public ClientUpdateRequest Client { get; set; }
        public List<PurchaseItemRequest> PurchaseItems { get; set; }
    }
}
