using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DbEntities
{
    public class PurchaseItem : BaseEntity
    {
        public double Amount { get; set; }
        public double Contamination { get; set; }
        public int SubCategoryId { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        public int PurchaseId { get; set; }
        public virtual Purchase Purchase { get; set; }
    }
}
