using Data.Mongo.Collections;
using Data.Mongo.Repo;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete
{
    public class LoginLogService : ILoginLogService
    {
        private readonly IMongoRepository<LoginLog> _repository;
        public LoginLogService(IMongoRepository<LoginLog> repository)
        {
            _repository = repository;
        }
        public async Task Add(LoginLog model)
        {
            await _repository.AddAsync(model);
        }

        public async Task<List<LoginLog>> Get(string userName)
        {
            var content = await _repository.FindAsync(x => x.UserName.Equals(userName));
            return content.ToList();
        }
    }
}
