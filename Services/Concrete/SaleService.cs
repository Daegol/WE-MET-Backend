using Data.Repos;
using Models.DbEntities;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Models.DTOs.Sale;
using Microsoft.AspNetCore.Hosting;
using System.Data;
using System.IO;
using Microsoft.Reporting.NETCore;
using Models.ReportModels;
using Models.ResponseModels;
using Models.Enums;

namespace Services.Concrete
{
    public class SaleService : ISaleService
    {
        private readonly IGenericRepository<Sale> _repository;
        private readonly IGenericRepository<SaleItem> _itemRepository;
        private readonly IGenericRepository<Client> _clientRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public SaleService(IGenericRepository<Sale> repository, IGenericRepository<SaleItem> itemRepository, IGenericRepository<Client> clientRepository, IWebHostEnvironment webHostEnvironment)
        {
            _repository = repository;
            _clientRepository = clientRepository;
            _itemRepository = itemRepository;
            _webHostEnvironment = webHostEnvironment;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }
        public Sale Add(Sale model)
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
            var Sale = _repository.Find(x => x.Id == id);
            if (Sale != null)
            {
                if (_repository.Delete(Sale) > 0)
                    return true;
            }
            return false;
        }

        public async Task<bool> Approve(int id)
        {
            var Sale = _repository.Find(x => x.Id == id);
            if (Sale != null)
            {
                Sale.Approved = true;
                await _repository.Update(Sale);
                return true;
            }
            return false;
        }

        public async Task<bool> RevokeApproval(int id)
        {
            var Sale = _repository.Find(x => x.Id == id);
            if (Sale != null)
            {
                Sale.Approved = false;
                await _repository.Update(Sale);
                return true;
            }
            return false;
        }

        public async Task<Sale> Get(int id)
        {
            return await _repository.GetByIdWithInclude(id, x => x
            .Include(x => x.SaleItems)
            .ThenInclude(x => x.SubCategory)
            .Include(x => x.Client));
        }

        public async Task<List<Sale>> GetAll()
        {
            return await _repository.GetAllWithInclude(x => x
            .Include(x => x.SaleItems)
            .ThenInclude(x => x.SubCategory)
            .Include(x => x.Client));
        }

        public async Task<List<Sale>> GetAllWithFilters(SaleGetWithFilterRequest request)
        {
            var purchases = await _repository.GetAllWithInclude(x => x
            .Include(x => x.SaleItems)
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

        public async Task<bool> Update(Sale model)
        {
            var sale = await _repository.GetByIdWithInclude(model.Id, x => x.Include(x => x.SaleItems));
            if (sale != null)
            {
                foreach (var item in sale.SaleItems)
                {
                    _itemRepository.Delete(item);
                }
                sale.SaleItems = model.SaleItems;
                if (_clientRepository.GetById(model.Client.Id) == null)
                {
                    var newClient = _clientRepository.Insert(model.Client);
                    sale.ClientId = newClient.Id;
                }
                else
                {
                    sale.ClientId = model.Client.Id;
                }
                await _repository.Update(sale);
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
            var sale = await _repository.GetByIdWithInclude(id, x => x
                .Include(x => x.SaleItems)
                .ThenInclude(x => x.SubCategory)
                .Include(x => x.Client));

            if (sale == null || sale?.SaleItems?.Count == 0)
            {
                var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\BlankReport.rdlc";
                using StreamReader stream = new StreamReader(File.OpenRead(path));
                LocalReport localReport = new LocalReport();
                localReport.LoadReportDefinition(stream);
                var parameters = new[]
                {
                new ReportParameter("Place", ""),
                new ReportParameter("Date", sale.Date.ToString()),
                new ReportParameter("UserName", userName),
                new ReportParameter("Summary", "0 kg"),
                new ReportParameter("ClientName", sale?.Client?.CompanyName ?? sale?.Client?.FirstName + " " + sale?.Client?.LastName),
                new ReportParameter("Name", "SPRZEDAŻ " + "WZ/" + sale.Id.ToString() + "/" + sale.Date.Month.ToString() + "/" + sale.Date.Year.ToString())
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

                var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\SaleReport.rdlc";
                using StreamReader stream = new StreamReader(File.OpenRead(path));
                LocalReport localReport = new LocalReport();
                localReport.LoadReportDefinition(stream);

                var parameters = new[]
                {
                new ReportParameter("Place", ""),
                new ReportParameter("Date", sale.Date.ToString()),
                new ReportParameter("UserName", userName),
                new ReportParameter("Summary", Math.Round(sale.SaleItems.Sum(x => x.Amount),2).ToString() + " kg"),
                new ReportParameter("ClientName", sale.Client.CompanyName ?? sale.Client.FirstName + " " + sale.Client.LastName),
                new ReportParameter("ReportNumber","WZ/" + sale.Id.ToString() + "/" + sale.Date.Month.ToString() + "/" + sale.Date.Year.ToString())
                };
                var saleItems = new SaleReportItem[sale.SaleItems.Count];

                int idx = 0;

                foreach (var item in sale.SaleItems)
                {
                    saleItems[idx] = new SaleReportItem();
                    saleItems[idx].SubCategoryName = item.SubCategory.Name;
                    saleItems[idx].Amount = item.Amount + " kg";

                    idx++;
                }

                localReport.DataSources.Add(new ReportDataSource("SaleItems", saleItems));

                localReport.SetParameters(parameters);

                var result = localReport.Render("PDF");

                return result;
            }
        }

        public async Task<byte[]> GenerateReportFromTo(string userName, DateTime from, DateTime to, ReportType type)
        {
            string clientType = "Klienci: Wszyscy";

            var sales = await _repository.FindAllAsyncWithInclude(x => x.Date >= from && x.Date < to.AddDays(1), y => y
                .Include(x => x.SaleItems)
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

            List<SaleItem> saleItems = sales.SelectMany(x => x.SaleItems).Distinct().ToList();

            if (saleItems.Count == 0 || sales == null)
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
                new ReportParameter("Name", "SPRZEDAŻY"),
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
                var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\SaleReportInTime.rdlc";
                using StreamReader stream = new StreamReader(File.OpenRead(path));
                LocalReport localReport = new LocalReport();
                localReport.LoadReportDefinition(stream);

                var parameters = new[]
                {
                new ReportParameter("DateFrom", from.ToString("d")),
                new ReportParameter("DateTo", to.ToString("d")),
                new ReportParameter("UserName", userName),
                new ReportParameter("Summary", Math.Round(saleItems.Sum(x => x.Amount),2).ToString() + " kg"),
                new ReportParameter("ClientType", clientType)
                };

                var groupedSaleItems = from s in saleItems group s by s.SubCategoryId;
                var saleItemsRep = new SaleReportItem[groupedSaleItems.Count()];

                int idx = 0;

                foreach (var item in groupedSaleItems)
                {
                    saleItemsRep[idx] = new SaleReportItem();
                    saleItemsRep[idx].SubCategoryName = item.ElementAt(0).SubCategory.Name;
                    saleItemsRep[idx].Amount = item.Sum(x => x.Amount) + " kg";

                    idx++;
                }

                localReport.DataSources.Add(new ReportDataSource("SaleItems", saleItemsRep));

                localReport.SetParameters(parameters);

                var result = localReport.Render("PDF");

                return result;
            }
        }

        public async Task<byte[]> GenerateReportDaily(string userName, DateTime date, ReportType type)
        {
            string clientType = "Klienci: Wszyscy";

            var sales = await _repository.FindAllAsyncWithInclude(x => x.Date >= date && x.Date < date.AddDays(1), y => y
                .Include(x => x.SaleItems)
                .ThenInclude(x => x.SubCategory)
                .Include(x => x.Client));

            if(type == ReportType.Company)
            {
                sales = sales.Where(x => x.Client.CompanyName != null).ToList();
                clientType = "Klienci: Firmy";
            }

            if(type == ReportType.Individual)
            {
                sales = sales.Where(x => x.Client.CompanyName == null).ToList();
                clientType = "Klienci: Indywidualni";
            }

            List<SaleItem> saleItems = sales.SelectMany(x => x.SaleItems).Distinct().ToList();

            if (saleItems?.Count == 0 || sales == null)
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
                new ReportParameter("Name", "SPRZEDAŻY"),
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
                var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\SaleReportDaily.rdlc";
                using StreamReader stream = new StreamReader(File.OpenRead(path));
                LocalReport localReport = new LocalReport();
                localReport.LoadReportDefinition(stream);

                var parameters = new[]
               {
                new ReportParameter("Date", date.ToString("d")),
                new ReportParameter("UserName", userName),
                new ReportParameter("Summary", Math.Round(saleItems.Sum(x => x.Amount),2).ToString() + " kg"),
                new ReportParameter("ClientType", clientType)
                };

                var groupedSaleItems = from s in saleItems group s by s.SubCategoryId;

                var saleItemsRep = new SaleReportItem[groupedSaleItems.Count()];

                int idx = 0;

                foreach (var item in groupedSaleItems)
                {
                    saleItemsRep[idx] = new SaleReportItem();
                    saleItemsRep[idx].SubCategoryName = item.ElementAt(0).SubCategory.Name;
                    saleItemsRep[idx].Amount = item.Sum(x => x.Amount) + " kg";

                    idx++;
                }

                localReport.DataSources.Add(new ReportDataSource("SaleItems", saleItemsRep));

                localReport.SetParameters(parameters);

                var result = localReport.Render("PDF");

                return result;
            }
        }

        public async Task<byte[]> GenerateReportClient(string userName, DateTime from, DateTime to, int clientId)
        {
            var sales = await _repository.FindAllAsyncWithInclude(x => x.Date >= from && x.Date < to.AddDays(1) && x.Client.Id == clientId, y => y
                .Include(x => x.SaleItems)
                .ThenInclude(x => x.SubCategory)
                .Include(x => x.Client));

            var client = _clientRepository.Find(x => x.Id == clientId);

            List<SaleItem> saleItems = sales.SelectMany(x => x.SaleItems).Distinct().ToList();

            if (saleItems.Count == 0 || sales == null)
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
                new ReportParameter("Name", "SPRZEDAŻY")
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
                var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\SaleReportClient.rdlc";
                using StreamReader stream = new StreamReader(File.OpenRead(path));
                LocalReport localReport = new LocalReport();
                localReport.LoadReportDefinition(stream);

                var parameters = new[]
                {
                new ReportParameter("DateFrom", from.ToString("d")),
                new ReportParameter("DateTo", to.ToString("d")),
                new ReportParameter("UserName", userName),
                new ReportParameter("Client", client.CompanyName ?? client.FirstName + " " + client.LastName),
                new ReportParameter("Summary", Math.Round(saleItems.Sum(x => x.Amount),2).ToString() + " kg"),
                };

                var groupedSaleItems = from s in saleItems group s by s.SubCategoryId;
                var saleItemsRep = new SaleReportItem[groupedSaleItems.Count()];

                int idx = 0;

                foreach (var item in groupedSaleItems)
                {
                    saleItemsRep[idx] = new SaleReportItem();
                    saleItemsRep[idx].SubCategoryName = item.ElementAt(0).SubCategory.Name;
                    saleItemsRep[idx].Amount = item.Sum(x => x.Amount) + " kg";

                    idx++;
                }

                localReport.DataSources.Add(new ReportDataSource("SaleItems", saleItemsRep));

                localReport.SetParameters(parameters);

                var result = localReport.Render("PDF");

                return result;
            }
        }

    }
}
