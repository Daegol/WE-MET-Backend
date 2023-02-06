using Models.DbEntities;
using Models.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IMainCategoryService
    {
        MainCategory Add(MainCategory model);
        Task<List<MainCategory>> GetAll();
        Task<MainCategory> Get(int id);
        bool Delete(int id);
        Task<bool> Update(MainCategory model);
    }
}
