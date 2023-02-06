using Models.DTOs.Employee;
using Models.DTOs.RecycledItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Recycled
{
    public class RecycledDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EmployeeDto Client { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateToApproval { get; set; }
        public string CreatedBy { get; set; }
        public bool Approved { get; set; }
        public List<RecycledItemDto> RecycledItems { get; set; }
    }
}
