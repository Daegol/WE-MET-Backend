using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Category
{
    public class SubCategoryRequest
    {
        public string Name { get; set; }
        public string MainCategoryId { get; set; }
    }
}
