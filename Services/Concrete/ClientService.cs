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
    public class ClientService : IClientService
    {
        private readonly IGenericRepository<Client> _repository;
        public ClientService(IGenericRepository<Client> repository)
        {
            _repository = repository;
        }
        public Client Add(Client model)
        {
            return _repository.Insert(model);
        }

        public bool Delete(int id)
        {
            var client = _repository.Find(x => x.Id == id);
            if (client != null)
            {
                if (_repository.Delete(client) > 0)
                    return true;
            }
            return false;
        }

        public async Task<List<Client>> GetAll()
        {
            return await _repository.GetAllWithInclude();
        }

        public async Task<List<Client>> GetAllCompany()
        {
            var result = await _repository.GetAllWithInclude();
            return result.FindAll(x => x.CompanyName != null);
        }

        public async Task<List<Client>> GetAllIndividuals()
        {
            var result = await _repository.GetAllWithInclude();
            return result.FindAll(x => x.CompanyName == null);
        }

        public async Task<bool> Update(Client model)
        {
            var client = _repository.Find(x => x.Id == model.Id);
            if (client != null)
            {
                client.FirstName = model.FirstName;
                client.LastName = model.LastName;
                client.CompanyName = model.CompanyName;
                await _repository.Update(client);
                return true;
            }
            return false;
        }
    }
}
