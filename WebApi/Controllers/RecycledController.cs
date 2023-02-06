using AutoMapper;
using Caching.Interfaces;
using Core.Exceptions;
using Identity.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DbEntities;
using Models.DTOs.Recycled;
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
    public class RecycledController : ControllerBase
    {

        private readonly IRecycledService _service;
        private readonly IMapper _mapper;
        private readonly ICacheManager _cache;
        private readonly IAccountService _accountService;

        public RecycledController(IRecycledService service, IMapper mapper, ICacheManager cache, IAccountService accountService)
        {
            _service = service;
            _mapper = mapper;
            _cache = cache;
            _accountService = accountService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<RecycledDto>>))]
        public async Task<IActionResult> GetAll()
        {
            var data = _mapper
                .Map<List<Recycled>, List<RecycledDto>>(await _service.GetAll());
            return Ok(new BaseResponse<IEnumerable<RecycledDto>>(data, $"Lista wszystkich wysortów"));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<RecycledDto>))]
        public async Task<IActionResult> GetOne(int id)
        {
            var data = _mapper
                .Map<Recycled, RecycledDto>(await _service.Get(id));
            if (data == null)
                throw new ApiException("Wysort nie istnieje") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<RecycledDto>(data, $"Wysort o numerze id {id}"));
        }

        [HttpGet("getAllWihtFilters")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<RecycledDto>>))]
        public async Task<IActionResult> GetAllWithFilters([FromQuery] RecycledGetWithFilterRequest request)
        {
            var data = _mapper
                .Map<List<Recycled>, List<RecycledDto>>(await _service.GetAllWithFilters(request));
            return Ok(new BaseResponse<IEnumerable<RecycledDto>>(data, $"Lista wszystkich wysortów"));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Add([FromBody] RecycledRequest request)
        {
            _cache.Clear();
            var userId = User.FindFirstValue("uid");
            var currentUser = await _accountService.GetOneUser(int.Parse(userId));
            var data = _mapper.Map<RecycledRequest, Recycled>(request);
            data.CreatedBy = currentUser.FirstName + " " + currentUser.LastName;
            var result = _service.Add(data);
            if (result == null)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie.") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Wysort został dodany"));
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Update([FromBody] RecycledUpdateRequest request)
        {
            _cache.Clear();
            var isApproved = await _service.IsApproved(request.Id);
            if (isApproved == true && !HttpContext.User.IsInRole("Admin"))
            {
                throw new ApiException("Tylko administrator może edytować zatwierdzone dokumenty") { StatusCode = (int)HttpStatusCode.Unauthorized };
            }
            var result = await _service.Update(_mapper.Map<RecycledUpdateRequest, Recycled>(request));
            if (!result)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie.") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Wysort został zaaktualizowany"));
        }

        [HttpPut("approve/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Approve(int id)
        {
            _cache.Clear();
            var result = await _service.Approve(id);
            if (!result)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie.") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Wysort został zatwierdzony"));
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
            return Ok(new BaseResponse<string>("Wysort został usunięty"));
        }

        [HttpGet("getReport/{id}")]
        public async Task<IActionResult> GetReport(int id)
        {
            var data = await _service.Get(id);
            var report = await _service.GenerateReport(id, data.CreatedBy == "0" ? "Brak danych" : data.CreatedBy);
            return File(report, "application/pdf", "recycled_report.pdf");
        }

        [HttpGet("getDailyReport")]
        public async Task<IActionResult> GetDailyReport([FromQuery] RecycledDailyReportRequest request)
        {
            var userId = User.FindFirstValue("uid");
            var currentUser = await _accountService.GetOneUser(int.Parse(userId));
            var report = await _service.GenerateReportDaily(currentUser.FirstName + " " + currentUser.LastName, DateTime.Parse(request.Day));
            return File(report, "application/pdf", "recycledDailyReport.pdf");
        }

        [HttpGet("getMonthlyReport")]
        public async Task<IActionResult> GetMonthlyReport([FromQuery] RecycledMonthlyReportRequest request)
        {
            var userId = User.FindFirstValue("uid");
            var currentUser = await _accountService.GetOneUser(int.Parse(userId));
            var report = await _service.GenerateReportFromTo(currentUser.FirstName + " " + currentUser.LastName, DateTime.Parse(request.Month), DateTime.Parse(request.Month).AddMonths(1).AddDays(-1));
            return File(report, "application/pdf", "recycledMonthlyReport.pdf");
        }

        [HttpGet("getCustomReport")]
        public async Task<IActionResult> GetCustomReport([FromQuery] RecycledCustomReportRequest request)
        {
            var userId = User.FindFirstValue("uid");
            var currentUser = await _accountService.GetOneUser(int.Parse(userId));
            var report = await _service.GenerateReportFromTo(currentUser.FirstName + " " + currentUser.LastName, DateTime.Parse(request.DateFrom), DateTime.Parse(request.DateTo));
            return File(report, "application/pdf", "recycledCustomReport.pdf");
        }
    }
}
