using Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IClientService
    {
        Client Add(Client model);
        Task<List<Client>> GetAll();
        Task<List<Client>> GetAllIndividuals();
        Task<List<Client>> GetAllCompany();
        bool Delete(int id);
        Task<bool> Update(Client model);
    }
}
