using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DbEntities
{
    public class SaleItem : BaseEntity
    {
        public double Amount { get; set; }
        public int SubCategoryId { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        public int SaleId { get; set; }
        public virtual Sale Sale { get; set; }
    }
}
