using Models.DbEntities;
using Models.DTOs.Purchases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IPurchaseItemService
    {
        PurchaseItem Add(PurchaseItem model);
        Task<List<PurchaseItem>> GetAll();
        Task<PurchaseItem> Get(int id);
        bool Delete(int id);
        Task<bool> Update(PurchaseItem model);
    }
}
