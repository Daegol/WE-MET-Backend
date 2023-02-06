using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DbEntities
{
    public class Sale : BaseEntity
    {
        public DateTime Date { get; set; }
        public DateTime DateToApproval { get; set; }
        public bool Approved { get; set; }
        public string CreatedBy { get; set; }
        public virtual ICollection<SaleItem> SaleItems { get; set; }
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }
    }
}
