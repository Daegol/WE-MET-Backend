using Data.Repos;
using Models.DbEntities;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Threading.Tasks;
using Models.DTOs.Purchases;
using Microsoft.AspNetCore.Hosting;
using System.Data;
using Microsoft.Reporting.NETCore;
using System.Reflection;
using Models.ResponseModels;
using System.IO;
using Models.Enums;

namespace Services.Concrete
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IGenericRepository<Purchase> _repository;
        private readonly IGenericRepository<Client> _clientRepository;
        private readonly IGenericRepository<PurchaseItem> _itemRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public PurchaseService(IGenericRepository<Purchase> repository, IGenericRepository<PurchaseItem> itemRepository, IGenericRepository<Client> clientRepository, IWebHostEnvironment webHostEnvironment)
        {
            _repository = repository;
            _itemRepository = itemRepository;
            _clientRepository = clientRepository;
            _webHostEnvironment = webHostEnvironment;
        }
        public Purchase Add(Purchase model)
        {
            if (_clientRepository.GetById(model.Client.Id) != null)
            {
                model.ClientId = model.Client.Id;
                model.Client = null;
            }
            return _repository.Insert(model);
        }

        public bool Delete(int id)
        {
            var purchase = _repository.Find(x => x.Id == id);
            if (purchase != null)
            {
                if (_repository.Delete(purchase) > 0)
                    return true;
            }
            return false;
        }

        public async Task<bool> Approve(int id)
        {
            var purchase = _repository.Find(x => x.Id == id);
            if (purchase != null)
            {
                purchase.Approved = true;
                await _repository.Update(purchase);
                return true;
            }
            return false;
        }

        public async Task<bool> RevokeApproval(int id)
        {
            var purchase = _repository.Find(x => x.Id == id);
            if (purchase != null)
            {
                purchase.Approved = false;
                await _repository.Update(purchase);
                return true;
            }
            return false;
        }

        public async Task<Purchase> Get(int id)
        {
            return await _repository.GetByIdWithInclude(id, x => x
            .Include(x => x.PurchaseItems)
            .ThenInclude(x => x.SubCategory)
            .Include(x => x.Client));
        }

        public async Task<List<Purchase>> GetAll()
        {
            return await _repository.GetAllWithInclude(x => x
            .Include(x => x.PurchaseItems)
            .ThenInclude(x => x.SubCategory)
            .Include(x => x.Client));
        }

        public async Task<List<Purchase>> GetAllWithFilters(PurchaseGetWithFilterRequest request)
        {
            var purchases = await _repository.GetAllWithInclude(x => x
            .Include(x => x.PurchaseItems)
            .ThenInclude(x => x.SubCategory)
            .Include(x => x.Client));
            if (request.StartDate != null && request.EndDate != null)
            {
                return purchases.Where(x => x.Date >= request.StartDate && x.Date < request.EndDate.Value.AddDays(1)).ToList();
            }

            if (request.StartDate != null && request.EndDate == null)
            {
                return purchases.Where(x => x.Date >= request.StartDate).ToList();
            }

            return purchases;
        }

        public async Task<bool> Update(Purchase model)
        {
            var purchase = await _repository.GetByIdWithInclude(model.Id,x => x.Include(x => x.PurchaseItems));
            if (purchase != null)
            {
                foreach(var item in purchase.PurchaseItems)
                {
                    _itemRepository.Delete(item);
                }
                purchase.PurchaseItems = model.PurchaseItems;
                if(_clientRepository.GetById(model.Client.Id) == null)
                {
                    var newClient = _clientRepository.Insert(model.Client);
                    purchase.ClientId = newClient.Id;
                }
                else
                {
                    purchase.ClientId = model.Client.Id;
                }
                await _repository.Update(purchase);
                return true;
            }
            return false;
        }

        public async Task<bool> IsApproved(int id)
        {
            var result = _repository.GetById(id);
            return result.Approved;
        }

        public async Task<byte[]> GenerateReport(int id, string userName)
        {
            var purchase = await _repository.GetByIdWithInclude(id, x => x
                .Include(x => x.PurchaseItems)
                .ThenInclude(x => x.SubCategory)
                .Include(x => x.Client));

            if (purchase == null || purchase?.PurchaseItems?.Count == 0)
            {
                var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\BlankReport.rdlc";
                using StreamReader stream = new StreamReader(File.OpenRead(path));
                LocalReport localReport = new LocalReport();
                localReport.LoadReportDefinition(stream);
                var parameters = new[]
                {
                new ReportParameter("Place", ""),
                new ReportParameter("Date", purchase.Date.ToString()),
                new ReportParameter("UserName", userName),
                new ReportParameter("Summary", "0 kg"),
                new ReportParameter("ClientName", purchase.Client.CompanyName ?? purchase.Client.FirstName + " " + purchase.Client.LastName),
                new ReportParameter("Name", "ZAKUP " + "PZ/" + purchase.Id.ToString() + "/" + purchase.Date.Month.ToString() + "/" + purchase.Date.Year.ToString())
                };

                var purchaseItems = new PurchaseReportItem[1];
                purchaseItems[0] = new PurchaseReportItem();

                localReport.DataSources.Add(new ReportDataSource("PurchaseItem", purchaseItems));

                localReport.SetParameters(parameters);

                var result = localReport.Render("PDF");

                return result;

            }
            else
            {
                var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\PurchaseReport.rdlc";
                using StreamReader stream = new StreamReader(File.OpenRead(path));
                LocalReport localReport = new LocalReport();
                localReport.LoadReportDefinition(stream);
                var parameters = new[]
                {
                new ReportParameter("Place", ""),
                new ReportParameter("Date", purchase.Date.ToString()),
                new ReportParameter("UserName", userName),
                new ReportParameter("Summary", Math.Round(purchase.PurchaseItems.Sum(x => x.Amount - x.Contamination),2).ToString() + " kg"),
                new ReportParameter("ClientName", purchase.Client.CompanyName ?? purchase.Client.FirstName + " " + purchase.Client.LastName),
                new ReportParameter("ReportNumber","PZ/" + purchase.Id.ToString() + "/" + purchase.Date.Month.ToString() + "/" + purchase.Date.Year.ToString())
                };
                var purchaseItems = new PurchaseReportItem[purchase.PurchaseItems.Count];

                int idx = 0;

                foreach (var item in purchase.PurchaseItems)
                {
                    purchaseItems[idx] = new PurchaseReportItem();
                    purchaseItems[idx].SubCategoryName = item.SubCategory.Name;
                    purchaseItems[idx].Amount = (item.Amount - item.Contamination) + " kg";
                    purchaseItems[idx].Contamination = item.Contamination + " kg";

                    idx++;
                }

                localReport.DataSources.Add(new ReportDataSource("PurchaseItem", purchaseItems));

                localReport.SetParameters(parameters);

                var result = localReport.Render("PDF");

                return result;
            }
        }

        public async Task<byte[]> GenerateReportFromTo(string userName, DateTime from, DateTime to, ReportType type)
        {
            string clientType = "Klienci: Wszyscy";

            var purchases = await _repository.FindAllAsyncWithInclude(x => x.Date >= from && x.Date < to.AddDays(1), y => y
                .Include(x => x.PurchaseItems)
                .ThenInclude(x => x.SubCategory)
                .Include(x => x.Client));

            if (type == ReportType.Company)
            {
                purchases = purchases.Where(x => x.Client.CompanyName != null).ToList();
                clientType = "Klienci: Firmy";
            }

            if (type == ReportType.Individual)
            {
                purchases = purchases.Where(x => x.Client.CompanyName == null).ToList();
                clientType = "Klienci: Indywidualni";
            }

            List<PurchaseItem> purchaseItems = purchases.SelectMany(x => x.PurchaseItems).Distinct().ToList();

            if (purchaseItems?.Count == 0 || purchases == null)
            {
                var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\BlankReportInTime.rdlc";
                using StreamReader stream = new StreamReader(File.OpenRead(path));
                LocalReport localReport = new LocalReport();
                localReport.LoadReportDefinition(stream);
                var parameters = new[]
                {
                new ReportParameter("DateFrom", from.ToString("d")),
                new ReportParameter("DateTo", to.ToString("d")),
                new ReportParameter("UserName", userName),
                new ReportParameter("Summary", "0 kg"),
                new ReportParameter("Name", "ZAKUPÓW"),
                new ReportParameter("ClientType", clientType)
                };

                var purchaseItemsRep = new PurchaseReportItem[1];
                purchaseItemsRep[0] = new PurchaseReportItem();

                localReport.DataSources.Add(new ReportDataSource("PurchaseItem", purchaseItemsRep));
                localReport.SetParameters(parameters);

                var result = localReport.Render("PDF");

                return result;
            }
            else
            {

                var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\PurchaseReportInTime.rdlc";
                using StreamReader stream = new StreamReader(File.OpenRead(path));
                LocalReport localReport = new LocalReport();
                localReport.LoadReportDefinition(stream);
                var parameters = new[]
                {
                new ReportParameter("DateFrom", from.ToString("d")),
                new ReportParameter("DateTo", to.ToString("d")),
                new ReportParameter("UserName", userName),
                new ReportParameter("Summary", Math.Round(purchaseItems.Sum(x => x.Amount - x.Contamination),2).ToString() + " kg"),
                new ReportParameter("ClientType", clientType)
                };

                var groupedPurchaseItems = from s in purchaseItems group s by s.SubCategoryId;

                var purchaseItemsRep = new PurchaseReportItem[groupedPurchaseItems.Count()];

                var idx = 0;

                foreach (var item in groupedPurchaseItems)
                {
                    purchaseItemsRep[idx] = new PurchaseReportItem();
                    purchaseItemsRep[idx].SubCategoryName = item.ElementAt(0).SubCategory.Name;
                    purchaseItemsRep[idx].Amount = item.Sum(x => x.Amount - x.Contamination) + " kg";
                    purchaseItemsRep[idx].Contamination = item.Sum(x => x.Contamination) + " kg";
                    idx++;
                }

                localReport.DataSources.Add(new ReportDataSource("PurchaseItem", purchaseItemsRep));

                localReport.SetParameters(parameters);

                var result = localReport.Render("PDF");

                return result;
            }
        }

        public async Task<byte[]> GenerateReportDaily(string userName, DateTime date, ReportType type)
        {
            string clientType = "Klienci: Wszyscy";

            var sales = await _repository.FindAllAsyncWithInclude(x => x.Date >= date && x.Date < date.AddDays(1), y => y
                .Include(x => x.PurchaseItems)
                .ThenInclude(x => x.SubCategory)
                .Include(x => x.Client));

            if (type == ReportType.Company)
            {
                sales = sales.Where(x => x.Client.CompanyName != null).ToList();
                clientType = "Klienci: Firmy";
            }

            if (type == ReportType.Individual)
            {
                sales = sales.Where(x => x.Client.CompanyName == null).ToList();
                clientType = "Klienci: Indywidualni";
            }

            List<PurchaseItem> purchaseItems = sales.SelectMany(x => x.PurchaseItems).Distinct().ToList();

            if (purchaseItems?.Count == 0 || sales == null)
            {
                var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\BlankReportDaily.rdlc";
                using StreamReader stream = new StreamReader(File.OpenRead(path));
                LocalReport localReport = new LocalReport();
                localReport.LoadReportDefinition(stream);

                var parameters = new[]
                {
                new ReportParameter("Date", date.ToString("d")),
                new ReportParameter("UserName", userName),
                new ReportParameter("Summary", "0 kg"),
                new ReportParameter("Name", "ZAKUPÓW"),
                new ReportParameter("ClientType", clientType)
                };

                localReport.SetParameters(parameters);

                var purchaseItemsRep = new PurchaseReportItem[1];
                purchaseItemsRep[0] = new PurchaseReportItem();

                localReport.DataSources.Add(new ReportDataSource("PurchaseItem", purchaseItemsRep));

                var result = localReport.Render("PDF");

                return result;
            }
            else
            {

                var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\PurchaseReportDaily.rdlc";
                using StreamReader stream = new StreamReader(File.OpenRead(path));
                LocalReport localReport = new LocalReport();
                localReport.LoadReportDefinition(stream);

                var parameters = new[]
                {
                new ReportParameter("Date", date.ToString("d")),
                new ReportParameter("UserName", userName),
                new ReportParameter("Summary", Math.Round(purchaseItems.Sum(x => x.Amount),2).ToString() + " kg"),
                new ReportParameter("ClientType", clientType)
            };

                var groupedPurchaseItems = from s in purchaseItems group s by s.SubCategoryId;

                var purchaseItemsRep = new PurchaseReportItem[groupedPurchaseItems.Count()];

                var idx = 0;

                foreach (var item in groupedPurchaseItems)
                {
                    purchaseItemsRep[idx] = new PurchaseReportItem();
                    purchaseItemsRep[idx].SubCategoryName = item.ElementAt(0).SubCategory.Name;
                    purchaseItemsRep[idx].Amount = item.Sum(x => x.Amount - x.Contamination) + " kg";
                    purchaseItemsRep[idx].Contamination = item.Sum(x => x.Contamination) + " kg";
                    idx++;
                }

                localReport.DataSources.Add(new ReportDataSource("PurchaseItem", purchaseItemsRep));

                localReport.SetParameters(parameters);

                var result = localReport.Render("PDF");

                return result;
            }
        }

        public async Task<byte[]> GenerateReportClient(string userName, DateTime from, DateTime to, int clientId)
        {
            var purchases = await _repository.FindAllAsyncWithInclude(x => x.Date >= from && x.Date < to.AddDays(1) && x.Client.Id == clientId, y => y
                .Include(x => x.PurchaseItems)
                .ThenInclude(x => x.SubCategory)
                .Include(x => x.Client));

            List<PurchaseItem> purchaseItems = purchases.SelectMany(x => x.PurchaseItems).Distinct().ToList();

            var client = _clientRepository.Find(x => x.Id == clientId);

            if (purchaseItems?.Count == 0 || purchases == null)
            {
                var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\BlankReportClient.rdlc";
                using StreamReader stream = new StreamReader(File.OpenRead(path));
                LocalReport localReport = new LocalReport();
                localReport.LoadReportDefinition(stream);
                var parameters = new[]
                {
                new ReportParameter("DateFrom", from.ToString("d")),
                new ReportParameter("DateTo", to.ToString("d")),
                new ReportParameter("UserName", userName),
                new ReportParameter("Client", client.CompanyName ?? client.FirstName + " " + client.LastName),
                new ReportParameter("Summary", "0 kg"),
                new ReportParameter("Name", "ZAKUPÓW")
                };

                var purchaseItemsRep = new PurchaseReportItem[1];
                purchaseItemsRep[0] = new PurchaseReportItem();

                localReport.DataSources.Add(new ReportDataSource("PurchaseItem", purchaseItemsRep));
                localReport.SetParameters(parameters);

                var result = localReport.Render("PDF");

                return result;
            }
            else
            {

                var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\PurchaseReportClient.rdlc";
                using StreamReader stream = new StreamReader(File.OpenRead(path));
                LocalReport localReport = new LocalReport();
                localReport.LoadReportDefinition(stream);
                var parameters = new[]
                {
                new ReportParameter("DateFrom", from.ToString("d")),
                new ReportParameter("DateTo", to.ToString("d")),
                new ReportParameter("UserName", userName),
                new ReportParameter("Client", client.CompanyName ?? client.FirstName + " " + client.LastName),
                new ReportParameter("Summary", Math.Round(purchaseItems.Sum(x => x.Amount),2).ToString() + " kg"),
                };

                var groupedPurchaseItems = from s in purchaseItems group s by s.SubCategoryId;

                var purchaseItemsRep = new PurchaseReportItem[groupedPurchaseItems.Count()];

                var idx = 0;

                foreach (var item in groupedPurchaseItems)
                {
                    purchaseItemsRep[idx] = new PurchaseReportItem();
                    purchaseItemsRep[idx].SubCategoryName = item.ElementAt(0).SubCategory.Name;
                    purchaseItemsRep[idx].Amount = item.Sum(x => x.Amount - x.Contamination) + " kg";
                    purchaseItemsRep[idx].Contamination = item.Sum(x => x.Contamination) + " kg";
                    idx++;
                }

                localReport.DataSources.Add(new ReportDataSource("PurchaseItem", purchaseItemsRep));

                localReport.SetParameters(parameters);

                var result = localReport.Render("PDF");

                return result;
            }
        }
    }
}
