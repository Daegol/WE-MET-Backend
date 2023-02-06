using Data.Repos;
using Models.DbEntities;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace Services.Concrete
{
    public class SaleItemService : ISaleItemService
    {
        private readonly IGenericRepository<SaleItem> _repository;
        private readonly IGenericRepository<Sale> _saleRepository;
        public SaleItemService(IGenericRepository<SaleItem> repository,
            IGenericRepository<Sale> saleRepository)
        {
            _repository = repository;
            _saleRepository = saleRepository;
        }
        public SaleItem Add(SaleItem model)
        {
            if (_saleRepository.GetById(model.SaleId) != null)
            {
                return _repository.Insert(model);
            }
            return null;
        }

        public bool Delete(int id)
        {
            var Sale = _repository.Find(x => x.Id == id);
            if (Sale != null)
            {
                if (_repository.Delete(Sale) > 0)
                    return true;
            }
            return false;
        }

        public async Task<SaleItem> Get(int id)
        {
            return await _repository.GetByIdWithInclude(id, x => x
            .Include(x => x.SubCategory));
        }

        public async Task<List<SaleItem>> GetAll()
        {
            return await _repository.GetAllWithInclude(x => x
            .Include(x => x.SubCategory));
        }

        public async Task<bool> Update(SaleItem model)
        {
            var SaleItem = _repository.Find(x => x.Id == model.Id);
            if (SaleItem != null)
            {
                SaleItem.Amount = model.Amount;
                SaleItem.SubCategoryId = model.SubCategoryId;
                await _repository.Update(SaleItem);
                return true;
            }
            return false;
        }
    }
}
