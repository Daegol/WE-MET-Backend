
using Models.DbEntities;
using Models.DTOs.Sale;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ISaleService
    {
        Sale Add(Sale model);
        Task<List<Sale>> GetAll();
        Task<List<Sale>> GetAllWithFilters(SaleGetWithFilterRequest request);
        Task<Sale> Get(int id);
        bool Delete(int id);
        Task<bool> Approve(int id);
        Task<bool> RevokeApproval(int id);
        Task<bool> Update(Sale model);
        Task<bool> IsApproved(int id);
        Task<byte[]> GenerateReport(int id, string userName);
        Task<byte[]> GenerateReportFromTo(string userName, DateTime from, DateTime to, ReportType type);
        Task<byte[]> GenerateReportDaily(string userName, DateTime date, ReportType type);
        Task<byte[]> GenerateReportClient(string userName, DateTime from, DateTime to, int clientId);
    }
}
