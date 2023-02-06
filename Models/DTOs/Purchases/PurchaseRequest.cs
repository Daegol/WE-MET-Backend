using Models.DbEntities;
using Models.DTOs.Client;
using Models.DTOs.PurchaseItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Purchases
{
    public class PurchaseRequest
    {
        public ClientUpdateRequest Client { get; set; }
        public List<PurchaseItemRequest> PurchaseItems { get; set; }
    }
}
