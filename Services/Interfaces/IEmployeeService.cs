using Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IEmployeeService
    {
        Employee Add(Employee model);
        Task<List<Employee>> GetAll();
        Task<Employee> GetById(int id);
        bool Delete(int id);
        Task<bool> Update(Employee model);
    }
}
