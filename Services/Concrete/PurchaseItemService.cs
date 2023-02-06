using Data.Repos;
using Models.DbEntities;
using Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTOs.Purchases;

namespace Services.Concrete
{
    public class PurchaseItemService : IPurchaseItemService
    {
        private readonly IGenericRepository<PurchaseItem> _repository;
        private readonly IGenericRepository<Purchase> _purchaseRepository;
        public PurchaseItemService(IGenericRepository<PurchaseItem> repository, 
            IGenericRepository<Purchase> purchaseRepository)
        {
            _repository = repository;
            _purchaseRepository = purchaseRepository;
        }
        public PurchaseItem Add(PurchaseItem model)
        {
            if(_purchaseRepository.GetById(model.PurchaseId) != null)
            {
                return _repository.Insert(model);
            }
            return null;
        }

        public bool Delete(int id)
        {
            var purchase = _repository.Find(x => x.Id == id);
            if (purchase != null)
            {
                if (_repository.Delete(purchase) > 0)
                    return true;
            }
            return false;
        }

        public async Task<PurchaseItem> Get(int id)
        {
            return await _repository.GetByIdWithInclude(id, x => x
            .Include(x => x.SubCategory));
        }

        public async Task<List<PurchaseItem>> GetAll()
        {
            return await _repository.GetAllWithInclude(x => x
            .Include(x => x.SubCategory));
        }

        public async Task<bool> Update(PurchaseItem model)
        {
            var purchaseItem = _repository.Find(x => x.Id == model.Id);
            if (purchaseItem != null)
            {
                purchaseItem.Amount = model.Amount;
                purchaseItem.SubCategoryId = model.SubCategoryId;
                purchaseItem.Contamination = model.Contamination;
                await _repository.Update(purchaseItem);
                return true;
            }
            return false;
        }
    }
}
