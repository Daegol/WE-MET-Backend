using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DbEntities
{
    public class RecycledItem : BaseEntity
    {
        public double Amount { get; set; }
        public int SubCategoryId { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        public int RecycledId { get; set; }
        public virtual Recycled Recycled { get; set; }
    }
}
