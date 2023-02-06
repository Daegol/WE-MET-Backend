using Data.Repos;
using Microsoft.AspNetCore.Hosting;
using Models.DbEntities;
using Models.DTOs.Recycled;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Reporting.NETCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Models.ReportModels;
using Models.ResponseModels;

namespace Services.Concrete
{
    public class RecycledService : IRecycledService
    {
        private readonly IGenericRepository<Recycled> _repository;
        private readonly IGenericRepository<RecycledItem> _itemRepository;
        private readonly IGenericRepository<Employee> _employeeRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public RecycledService(IGenericRepository<Recycled> repository, IGenericRepository<RecycledItem> itemRepository, IGenericRepository<Employee> employeeRepository, IWebHostEnvironment webHostEnvironment)
        {
            _repository = repository;
            _employeeRepository = employeeRepository;
            _itemRepository = itemRepository;
            _webHostEnvironment = webHostEnvironment;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }
        public Recycled Add(Recycled model)
        {
            if (_employeeRepository.GetById(model.Employee.Id) != null)
            {
                model.EmployeeId = model.Employee.Id;
                model.Employee = null;
            }
            return _repository.Insert(model);
        }

        public bool Delete(int id)
        {
            var recycled = _repository.Find(x => x.Id == id);
            if (recycled != null)
            {
                if (_repository.Delete(recycled) > 0)
                    return true;
            }
            return false;
        }

        public async Task<bool> Approve(int id)
        {
            var recycled = _repository.Find(x => x.Id == id);
            if (recycled != null)
            {
                recycled.Approved = true;
                await _repository.Update(recycled);
                return true;
            }
            return false;
        }

        public async Task<bool> RevokeApproval(int id)
        {
            var recycled = _repository.Find(x => x.Id == id);
            if (recycled != null)
            {
                recycled.Approved = false;
                await _repository.Update(recycled);
                return true;
            }
            return false;
        }

        public async Task<Recycled> Get(int id)
        {
            return await _repository.GetByIdWithInclude(id, x => x
            .Include(x => x.RecycledItems)
            .ThenInclude(x => x.SubCategory)
            .Include(x => x.Employee));
        }

        public async Task<List<Recycled>> GetAll()
        {
            return await _repository.GetAllWithInclude(x => x
            .Include(x => x.RecycledItems)
            .ThenInclude(x => x.SubCategory)
            .Include(x => x.Employee));
        }

        public async Task<List<Recycled>> GetAllWithFilters(RecycledGetWithFilterRequest request)
        {
            var purchases = await _repository.GetAllWithInclude(x => x
            .Include(x => x.RecycledItems)
            .ThenInclude(x => x.SubCategory)
            .Include(x => x.Employee));
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

        public async Task<bool> Update(Recycled model)
        {
            var recycled = await _repository.GetByIdWithInclude(model.Id, x => x.Include(x => x.RecycledItems));
            if (recycled != null)
            {
                foreach (var item in recycled.RecycledItems)
                {
                    _itemRepository.Delete(item);
                }
                recycled.RecycledItems = model.RecycledItems;
                if (_employeeRepository.GetById(model.Employee.Id) == null)
                {
                    var newEmployee = _employeeRepository.Insert(model.Employee);
                    recycled.EmployeeId = newEmployee.Id;
                }
                else
                {
                    recycled.EmployeeId = model.Employee.Id;
                }
                await _repository.Update(recycled);
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
            var recycled = await _repository.GetByIdWithInclude(id, x => x
                .Include(x => x.RecycledItems)
                .ThenInclude(x => x.SubCategory)
                .Include(x => x.Employee));

            if (recycled == null || recycled?.RecycledItems?.Count == 0)
            {
                var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\BlankReport.rdlc";
                using StreamReader stream = new StreamReader(File.OpenRead(path));
                LocalReport localReport = new LocalReport();
                localReport.LoadReportDefinition(stream);
                var parameters = new[]
                {
                new ReportParameter("Place", ""),
                new ReportParameter("Date", DateTime.Now.ToString("g")),
                new ReportParameter("UserName", userName),
                new ReportParameter("Summary", "0 kg"),
                new ReportParameter("ClientName", recycled?.Employee?.FirstName + " " + recycled?.Employee?.LastName),
                new ReportParameter("Name", "WYSORT " + "WS/" + recycled.Id.ToString() + "/" + recycled.Date.Month.ToString() + "/" + recycled.Date.Year.ToString())
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
                var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\RecycledReport.rdlc";
                using StreamReader stream = new StreamReader(File.OpenRead(path));
                LocalReport localReport = new LocalReport();
                localReport.LoadReportDefinition(stream);

                var parameters = new[]
                {
                new ReportParameter("Place", ""),
                new ReportParameter("Date", DateTime.Now.ToString("g")),
                new ReportParameter("UserName", userName),
                new ReportParameter("Summary", Math.Round(recycled.RecycledItems.Sum(x => x.Amount),2).ToString() + " kg"),
                new ReportParameter("ClientName", recycled.Employee.FirstName + " " + recycled.Employee.LastName),
                new ReportParameter("ReportNumber","WS/" + recycled.Id.ToString() + "/" + recycled.Date.Month.ToString() + "/" + recycled.Date.Year.ToString())
            };
                var recycledItems = new RecycledReportItem[recycled.RecycledItems.Count];

                int idx = 0;

                foreach (var item in recycled.RecycledItems)
                {
                    recycledItems[idx] = new RecycledReportItem();
                    recycledItems[idx].SubCategoryName = item.SubCategory.Name;
                    recycledItems[idx].Amount = item.Amount + " kg";

                    idx++;
                }

                localReport.DataSources.Add(new ReportDataSource("RecycledItems", recycledItems));

                localReport.SetParameters(parameters);

                var result = localReport.Render("PDF");

                return result;
            }
        }

        public async Task<byte[]> GenerateReportFromTo(string userName, DateTime from, DateTime to)
        {
            var recycleds = await _repository.FindAllAsyncWithInclude(x => x.Date >= from && x.Date < to.AddDays(1), y => y
                .Include(x => x.RecycledItems)
                .ThenInclude(x => x.SubCategory)
                .Include(x => x.Employee));

            List<RecycledItem> recycledItems = recycleds.SelectMany(x => x.RecycledItems).Distinct().ToList();

            if (recycledItems.Count == 0 || recycleds == null)
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
                new ReportParameter("Name", "WYSORTÓW")
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
                var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\RecycledReportInTime.rdlc";
                using StreamReader stream = new StreamReader(File.OpenRead(path));
                LocalReport localReport = new LocalReport();
                localReport.LoadReportDefinition(stream);

                var parameters = new[]
                {
                new ReportParameter("DateFrom", from.ToString("d")),
                new ReportParameter("DateTo", to.ToString("d")),
                new ReportParameter("UserName", userName),
                new ReportParameter("Summary", Math.Round(recycledItems.Sum(x => x.Amount),2).ToString() + " kg"),
                };

                var groupedRecycledItems = from s in recycledItems group s by s.SubCategoryId;
                var recycledItemsRep = new RecycledReportItem[groupedRecycledItems.Count()];

                int idx = 0;

                foreach (var item in groupedRecycledItems)
                {
                    recycledItemsRep[idx] = new RecycledReportItem();
                    recycledItemsRep[idx].SubCategoryName = item.ElementAt(0).SubCategory.Name;
                    recycledItemsRep[idx].Amount = item.Sum(x => x.Amount) + " kg";

                    idx++;
                }

                localReport.DataSources.Add(new ReportDataSource("RecycledItems", recycledItemsRep));

                localReport.SetParameters(parameters);

                var result = localReport.Render("PDF");

                return result;
            }
        }

        public async Task<byte[]> GenerateReportDaily(string userName, DateTime date)
        {
            var recycleds = await _repository.FindAllAsyncWithInclude(x => x.Date >= date && x.Date < date.AddDays(1), y => y
                .Include(x => x.RecycledItems)
                .ThenInclude(x => x.SubCategory)
                .Include(x => x.Employee));

            List<RecycledItem> recycledItems = recycleds.SelectMany(x => x.RecycledItems).Distinct().ToList();

            if (recycledItems?.Count == 0 || recycleds == null)
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
                new ReportParameter("Name", "WYSORTÓW")
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
                var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\RecycledReportDaily.rdlc";
                using StreamReader stream = new StreamReader(File.OpenRead(path));
                LocalReport localReport = new LocalReport();
                localReport.LoadReportDefinition(stream);

                var parameters = new[]
               {
                new ReportParameter("Date", date.ToString("d")),
                new ReportParameter("UserName", userName),
                new ReportParameter("Summary", Math.Round(recycledItems.Sum(x => x.Amount),2).ToString() + " kg"),
                };

                var groupedRecycledItems = from s in recycledItems group s by s.SubCategoryId;

                var recycledItemsRep = new RecycledReportItem[groupedRecycledItems.Count()];

                int idx = 0;

                foreach (var item in groupedRecycledItems)
                {
                    recycledItemsRep[idx] = new RecycledReportItem();
                    recycledItemsRep[idx].SubCategoryName = item.ElementAt(0).SubCategory.Name;
                    recycledItemsRep[idx].Amount = item.Sum(x => x.Amount) + " kg";

                    idx++;
                }

                localReport.DataSources.Add(new ReportDataSource("RecycledItems", recycledItemsRep));

                localReport.SetParameters(parameters);

                var result = localReport.Render("PDF");

                return result;
            }
        }
    }
}
