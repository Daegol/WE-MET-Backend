using Models.DTOs.Client;
using Models.DTOs.SaleItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Sale
{
    public class SaleUpdateRequest
    {
        public int Id { get; set; }
        public ClientUpdateRequest Client { get; set; }
        public List<SaleItemRequest> SaleItems { get; set; }
    }
}
