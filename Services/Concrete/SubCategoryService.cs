using Data.Repos;
using Models.DbEntities;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete
{
    public class SubCategoryService : ISubCategoryService
    {
        private readonly IGenericRepository<SubCategory> _repository;
        public SubCategoryService(IGenericRepository<SubCategory> repository)
        {
            _repository = repository;
        }
        public SubCategory Add(SubCategory model)
        {
            var category = _repository.Find(x => x.Name == model.Name);
            return _repository.Insert(model);
        }

        public bool Delete(int id)
        {
            var subCategory = _repository.Find(x => x.Id == id);
            if (subCategory != null)
            {
                if (_repository.Delete(subCategory) > 0)
                    return true;
            }
            return false;
        }

        public async Task<SubCategory> Get(int id)
        {
            return _repository.GetById(id);
        }

        public async Task<List<SubCategory>> GetAll()
        {
            return _repository.GetAll();
        }

        public async Task<bool> Update(SubCategory model)
        {

            var subCategory = _repository.Find(x => x.Id == model.Id);
            if (subCategory != null)
            {
                subCategory.Name = model.Name;
                await _repository.Update(subCategory);
                return true;
            }
            return false;
        }
    }
}
