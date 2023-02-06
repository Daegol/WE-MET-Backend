using Data.Repos;
using Models.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete
{
    public class MainCategoryService : IMainCategoryService
    {
        private readonly IGenericRepository<MainCategory> _repository;
        public MainCategoryService(IGenericRepository<MainCategory> repository)
        {
            _repository = repository;
        }
        public MainCategory Add(MainCategory model)
        {
            var category = _repository.Find(x => x.Name == model.Name);
            return _repository.Insert(model);
        }

        public bool Delete(int id)
        {
            var mainCategory = _repository.Find(x => x.Id == id);
            if(mainCategory != null)
            {
                if (_repository.Delete(mainCategory) > 0)
                    return true;
            }
            return false;
        }

        public async Task<MainCategory> Get(int id)
        {
            return await _repository.GetByIdWithInclude(id, x => x.Include(x => x.SubCategories));
        }

        public async Task<List<MainCategory>> GetAll()
        {
            return await _repository.GetAllWithInclude(include: x => x.Include(x => x.SubCategories));
        }

        public async Task<bool> Update(MainCategory model)
        {
            var mainCategory = _repository.Find(x => x.Id == model.Id);
            if (mainCategory != null)
            {
                mainCategory.Name = model.Name;
                await _repository.Update(mainCategory);
                return true;
            }
            return false;
        }
    }
}
