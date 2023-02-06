using Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IRecycledItemService
    {
        RecycledItem Add(RecycledItem model);
        Task<List<RecycledItem>> GetAll();
        Task<RecycledItem> Get(int id);
        bool Delete(int id);
        Task<bool> Update(RecycledItem model);
    }
}
