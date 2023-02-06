using Models.DbEntities;
using Models.DTOs.Recycled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IRecycledService
    {
        Recycled Add(Recycled model);
        Task<List<Recycled>> GetAll();
        Task<List<Recycled>> GetAllWithFilters(RecycledGetWithFilterRequest request);
        Task<Recycled> Get(int id);
        bool Delete(int id);
        Task<bool> Approve(int id);
        Task<bool> RevokeApproval(int id);
        Task<bool> Update(Recycled model);
        Task<bool> IsApproved(int id);
        Task<byte[]> GenerateReport(int id, string userName);
        Task<byte[]> GenerateReportFromTo(string userName, DateTime from, DateTime to);
        Task<byte[]> GenerateReportDaily(string userName, DateTime date);
    }
}
