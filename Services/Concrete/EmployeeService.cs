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
    public class EmployeeService : IEmployeeService
    {
        private readonly IGenericRepository<Employee> _repository;
        public EmployeeService(IGenericRepository<Employee> repository)
        {
            _repository = repository;
        }
        public Employee Add(Employee model)
        {
            return _repository.Insert(model);
        }

        public bool Delete(int id)
        {
            var employee = _repository.Find(x => x.Id == id);
            if (employee != null)
            {
                if (_repository.Delete(employee) > 0)
                    return true;
            }
            return false;
        }

        public async Task<List<Employee>> GetAll()
        {
            return await _repository.GetAllWithInclude();
        }

        public async Task<Employee> GetById(int id)
        {
            return await _repository.GetByIdWithInclude(id);
        }

        public async Task<bool> Update(Employee model)
        {
            var employee = _repository.Find(x => x.Id == model.Id);
            if (employee != null)
            {
                employee.FirstName = model.FirstName;
                employee.LastName = model.LastName;
                await _repository.Update(employee);
                return true;
            }
            return false;
        }
    }
}
