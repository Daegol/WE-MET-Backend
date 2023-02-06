using Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ISaleItemService
    {
        SaleItem Add(SaleItem model);
        Task<List<SaleItem>> GetAll();
        Task<SaleItem> Get(int id);
        bool Delete(int id);
        Task<bool> Update(SaleItem model);
    }
}
