
using Models.DbEntities;
using Models.DTOs.Purchases;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IPurchaseService
    {
        Purchase Add(Purchase model);
        Task<List<Purchase>> GetAll();
        Task<List<Purchase>> GetAllWithFilters(PurchaseGetWithFilterRequest request);
        Task<Purchase> Get(int id);
        bool Delete(int id);
        Task<bool> Approve(int id);
        Task<bool> RevokeApproval(int id);
        Task<bool> Update(Purchase model);
        Task<bool> IsApproved(int id);
        Task<byte[]> GenerateReport(int id, string userName);
        Task<byte[]> GenerateReportFromTo(string userName, DateTime from, DateTime to, ReportType type);
        Task<byte[]> GenerateReportDaily(string userName, DateTime date, ReportType type);
        Task<byte[]> GenerateReportClient(string userName, DateTime from, DateTime to, int clientId);
    }
}
