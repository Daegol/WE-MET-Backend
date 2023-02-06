using Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ISubCategoryService
    {
        SubCategory Add(SubCategory model);
        Task<List<SubCategory>> GetAll();
        Task<SubCategory> Get(int id);
        bool Delete(int id);
        Task<bool> Update(SubCategory model);
    }
}
