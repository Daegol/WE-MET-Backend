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
    public class PurchaseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateToApproval { get; set; }
        public string CreatedBy { get; set; }
        public bool Approved { get; set; }
        public ClientDto Client { get; set; }
        public List<PurchaseItemDto> PurchaseItems { get; set; }
    }
}
