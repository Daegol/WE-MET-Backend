using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DbEntities
{
    public class SubCategory : BaseEntity
    {
        public string Name { get; set; }
        public int MainCategoryId { get; set; }
        public virtual MainCategory MainCategory { get; set; }
        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; }
        public virtual ICollection<SaleItem> SaleItems   { get; set; }
        public virtual ICollection<RecycledItem> RecycledItems { get; set; }
    }
}
