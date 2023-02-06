using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DbEntities
{
    public class Recycled : BaseEntity
    {
        public DateTime Date { get; set; }
        public DateTime DateToApproval { get; set; }
        public bool Approved { get; set; }
        public string CreatedBy { get; set; }
        public virtual ICollection<RecycledItem> RecycledItems { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
