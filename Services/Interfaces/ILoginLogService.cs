using Data.Mongo.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ILoginLogService
    {
        Task Add(LoginLog model);
        Task<List<LoginLog>> Get(string email);
    }
}
