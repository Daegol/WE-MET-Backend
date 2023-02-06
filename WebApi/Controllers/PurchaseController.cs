using AutoMapper;
using Caching.Interfaces;
using Core.Exceptions;
using Identity.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DbEntities;
using Models.DTOs.Purchases;
using Models.Enums;
using Models.ResponseModels;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;


namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _service;
        private readonly IMapper _mapper;
        private readonly ICacheManager _cache;
        private readonly IAccountService _accountService;

        public PurchaseController(IPurchaseService service, IMapper mapper, ICacheManager cache, IAccountService accountService)
        {
            _service = service;
            _mapper = mapper;
            _cache = cache;
            _accountService = accountService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<PurchaseDto>>))]
        public async Task<IActionResult> GetAll()
        {
            var data = _mapper
                .Map<List<Purchase>, List<PurchaseDto>>(await _service.GetAll());
            return Ok(new BaseResponse<IEnumerable<PurchaseDto>>(data, $"Lista wszystkich zakupów"));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<PurchaseDto>))]
        public async Task<IActionResult> GetOne(int id)
        {
            var data = _mapper
                .Map<Purchase, PurchaseDto>(await _service.Get(id));
            if (data == null)
                throw new ApiException("Zakup nie istnieje") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<PurchaseDto>(data, $"Zakup o numerze id {id}"));
        }

        [HttpGet("getAllWihtFilters")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<PurchaseDto>>))]
        public async Task<IActionResult> GetAllWithFilters([FromQuery] PurchaseGetWithFilterRequest request)
        {
            var data = _mapper
                .Map<List<Purchase>, List<PurchaseDto>>(await _service.GetAllWithFilters(request));
            return Ok(new BaseResponse<IEnumerable<PurchaseDto>>(data, $"Lista wszystkich sprzedaży"));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Add([FromBody] PurchaseRequest request)
        {
            _cache.Clear();
            var userId = User.FindFirstValue("uid");
            var currentUser = await _accountService.GetOneUser(int.Parse(userId));
            var data = _mapper.Map<PurchaseRequest, Purchase>(request);
            data.CreatedBy = currentUser.FirstName + " " + currentUser.LastName;
            var result = _service.Add(data);
            if (result == null)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie.") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Zakup został dodany"));
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Update([FromBody] PurchaseUpdateRequest request)
        {
            _cache.Clear();
            var isApproved = await _service.IsApproved(request.Id);
            if(isApproved == true && !HttpContext.User.IsInRole("Admin"))
            {
                throw new ApiException("Tylko administrator może edytować zatwierdzone dokumenty") { StatusCode = (int)HttpStatusCode.Unauthorized };
            }
            var result = await _service.Update(_mapper.Map<PurchaseUpdateRequest, Purchase>(request));
            if (!result)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie.") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Zakup został zaaktualizowany"));
        }

        [HttpPut("approve/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Approve(int id)
        {
            _cache.Clear();
            var result = await _service.Approve(id);
            if (!result)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie.") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Zakup został zatwierdzony"));
        }
        [Authorize(Policy = "OnlyAdmins")]
        [HttpPut("revokeApproval/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> RevokeApproval(int id)
        {
            _cache.Clear();
            var result = await _service.RevokeApproval(id);
            if (!result)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie.") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Wycofano zatwierdzenie dokumentu"));
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Delete(int id)
        {
            _cache.Clear();
            var result = _service.Delete(id);
            if (!result)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie.") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Zakup został usunięty"));
        }

        [HttpGet("getReport/{id}")]
        public async Task<IActionResult> GetReport(int id)
        {
            var data = await _service.Get(id);
            var report = await _service.GenerateReport(id, data.CreatedBy == "0"? "Brak danych":data.CreatedBy);
            return File(report, "application/pdf", "purchase_report.pdf");
        }

        [HttpGet("getMonthlyReportEveryone")]
        public async Task<IActionResult> GetMonthlyReportEveryone([FromQuery] PurchaseMonthlyReportRequest request)
        {
            var userId = User.FindFirstValue("uid");
            var currentUser = await _accountService.GetOneUser(int.Parse(userId));
            var report = await _service.GenerateReportFromTo(currentUser.FirstName + " " + currentUser.LastName, DateTime.Parse(request.Month), DateTime.Parse(request.Month).AddMonths(1).AddDays(-1),ReportType.Everyone);
            return File(report, "application/pdf", "purchaseMonthlyReport.pdf");
        }

        [HttpGet("getMonthlyReportIndividual")]
        public async Task<IActionResult> GetMonthlyReportIndividual([FromQuery] PurchaseMonthlyReportRequest request)
        {
            var userId = User.FindFirstValue("uid");
            var currentUser = await _accountService.GetOneUser(int.Parse(userId));
            var report = await _service.GenerateReportFromTo(currentUser.FirstName + " " + currentUser.LastName, DateTime.Parse(request.Month), DateTime.Parse(request.Month).AddMonths(1).AddDays(-1), ReportType.Individual);
            return File(report, "application/pdf", "purchaseMonthlyReport.pdf");
        }

        [HttpGet("getMonthlyReportCompany")]
        public async Task<IActionResult> GetMonthlyReportCompany([FromQuery] PurchaseMonthlyReportRequest request)
        {
            var userId = User.FindFirstValue("uid");
            var currentUser = await _accountService.GetOneUser(int.Parse(userId));
            var report = await _service.GenerateReportFromTo(currentUser.FirstName + " " + currentUser.LastName, DateTime.Parse(request.Month), DateTime.Parse(request.Month).AddMonths(1).AddDays(-1), ReportType.Company);
            return File(report, "application/pdf", "purchaseMonthlyReport.pdf");
        }

        [HttpGet("getCustomReportEveryone")]
        public async Task<IActionResult> GetCustomReportEveryone([FromQuery] PurchaseCustomReportRequest request)
        {
            var userId = User.FindFirstValue("uid");
            var currentUser = await _accountService.GetOneUser(int.Parse(userId));
            var report = await _service.GenerateReportFromTo(currentUser.FirstName + " " + currentUser.LastName, DateTime.Parse(request.DateFrom), DateTime.Parse(request.DateTo), ReportType.Everyone);
            return File(report, "application/pdf", "purchaseCustomReport.pdf");
        }

        [HttpGet("getCustomReportIndividual")]
        public async Task<IActionResult> GetCustomReportIndividual([FromQuery] PurchaseCustomReportRequest request)
        {
            var userId = User.FindFirstValue("uid");
            var currentUser = await _accountService.GetOneUser(int.Parse(userId));
            var report = await _service.GenerateReportFromTo(currentUser.FirstName + " " + currentUser.LastName, DateTime.Parse(request.DateFrom), DateTime.Parse(request.DateTo), ReportType.Individual);
            return File(report, "application/pdf", "purchaseCustomReport.pdf");
        }

        [HttpGet("getCustomReportCompany")]
        public async Task<IActionResult> GetCustomReportCompany([FromQuery] PurchaseCustomReportRequest request)
        {
            var userId = User.FindFirstValue("uid");
            var currentUser = await _accountService.GetOneUser(int.Parse(userId));
            var report = await _service.GenerateReportFromTo(currentUser.FirstName + " " + currentUser.LastName, DateTime.Parse(request.DateFrom), DateTime.Parse(request.DateTo), ReportType.Company);
            return File(report, "application/pdf", "purchaseCustomReport.pdf");
        }

        [HttpGet("getDailyReportEveryone")]
        public async Task<IActionResult> GetDailyReportEveryone([FromQuery] PurchaseDailyReportRequest request)
        {
            var userId = User.FindFirstValue("uid");
            var currentUser = await _accountService.GetOneUser(int.Parse(userId));
            var report = await _service.GenerateReportDaily(currentUser.FirstName + " " + currentUser.LastName, DateTime.Parse(request.Day), ReportType.Everyone);
            return File(report, "application/pdf", "purchaseDailyReport.pdf");
        }

        [HttpGet("getDailyReportIndividual")]
        public async Task<IActionResult> GetDailyReportIndividual([FromQuery] PurchaseDailyReportRequest request)
        {
            var userId = User.FindFirstValue("uid");
            var currentUser = await _accountService.GetOneUser(int.Parse(userId));
            var report = await _service.GenerateReportDaily(currentUser.FirstName + " " + currentUser.LastName, DateTime.Parse(request.Day), ReportType.Individual);
            return File(report, "application/pdf", "purchaseDailyReport.pdf");
        }

        [HttpGet("getDailyReportCompany")]
        public async Task<IActionResult> GetDailyReportCompany([FromQuery] PurchaseDailyReportRequest request)
        {
            var userId = User.FindFirstValue("uid");
            var currentUser = await _accountService.GetOneUser(int.Parse(userId));
            var report = await _service.GenerateReportDaily(currentUser.FirstName + " " + currentUser.LastName, DateTime.Parse(request.Day), ReportType.Company);
            return File(report, "application/pdf", "purchaseDailyReport.pdf");
        }

        [HttpGet("getClientReport")]
        public async Task<IActionResult> GetClientReport([FromQuery] PurchaseClientReportRequest request)
        {
            var userId = User.FindFirstValue("uid");
            var currentUser = await _accountService.GetOneUser(int.Parse(userId));
            var report = await _service.GenerateReportClient(currentUser.FirstName + " " + currentUser.LastName, DateTime.Parse(request.DateFrom), DateTime.Parse(request.DateTo), request.ClientId);
            return File(report, "application/pdf", "purchaseClientReport.pdf");
        }
    }
}
