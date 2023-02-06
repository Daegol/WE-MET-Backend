using Data.Repos;
using Models.DbEntities;
using Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete
{
    public class RecycledItemService : IRecycledItemService
    {
        private readonly IGenericRepository<RecycledItem> _repository;
        private readonly IGenericRepository<Recycled> _recycledRepository;
        public RecycledItemService(IGenericRepository<RecycledItem> repository,
            IGenericRepository<Recycled> recycledRepository)
        {
            _repository = repository;
            _recycledRepository = recycledRepository;
        }
        public RecycledItem Add(RecycledItem model)
        {
            if (_recycledRepository.GetById(model.RecycledId) != null)
            {
                return _repository.Insert(model);
            }
            return null;
        }

        public bool Delete(int id)
        {
            var recycled = _repository.Find(x => x.Id == id);
            if (recycled != null)
            {
                if (_repository.Delete(recycled) > 0)
                    return true;
            }
            return false;
        }

        public async Task<RecycledItem> Get(int id)
        {
            return await _repository.GetByIdWithInclude(id, x => x
            .Include(x => x.SubCategory));
        }

        public async Task<List<RecycledItem>> GetAll()
        {
            return await _repository.GetAllWithInclude(x => x
            .Include(x => x.SubCategory));
        }

        public async Task<bool> Update(RecycledItem model)
        {
            var recycledItem = _repository.Find(x => x.Id == model.Id);
            if (recycledItem != null)
            {
                recycledItem.Amount = model.Amount;
                recycledItem.SubCategoryId = model.SubCategoryId;
                await _repository.Update(recycledItem);
                return true;
            }
            return false;
        }
    }
}
