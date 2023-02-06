using Models.DTOs.Employee;
using Models.DTOs.RecycledItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Recycled
{
    public class RecycledRequest
    {
        public EmployeeUpdateRequest Employee { get; set; }
        public List<RecycledItemRequest> RecycledItems { get; set; }
    }
}
