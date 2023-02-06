using AutoMapper;
using Caching.Interfaces;
using Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DbEntities;
using Models.DTOs.Employee;
using Models.ResponseModels;
using Services.Interfaces;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using WebApi.Attributes;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _service;
        private readonly IMapper _mapper;
        private readonly ICacheManager _cache;

        public EmployeeController(IEmployeeService service, IMapper mapper, ICacheManager cache)
        {
            _service = service;
            _mapper = mapper;
            _cache = cache;
        }

        [Cached(2)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<EmployeeDto>>))]
        public async Task<IActionResult> GetAll()
        {
            var data = _mapper
                .Map<List<Employee>, List<EmployeeDto>>(await _service.GetAll());
            return Ok(new BaseResponse<IEnumerable<EmployeeDto>>(data, $"Lista wszystkich pracowników"));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<EmployeeDto>))]
        public async Task<IActionResult> GetOne(int id)
        {
            var data = _mapper
                .Map<Employee, EmployeeDto>(await _service.GetById(id));
            if (data == null)
                throw new ApiException("Pracownik nie istnieje") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<EmployeeDto>(data, $"Pracownik o numerze id {id}"));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Add([FromBody] EmployeeRequest request)
        {
            _cache.Clear();
            var result = _service.Add(_mapper.Map<EmployeeRequest, Employee>(request));
            if (result == null)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie.") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Nowy pracownik został dodany"));
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Update([FromBody] EmployeeUpdateRequest request)
        {
            _cache.Clear();
            var result = await _service.Update(_mapper.Map<EmployeeUpdateRequest, Employee>(request));
            if (!result)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie.") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Pracownik został zaaktualizowany"));
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Delete(int id)
        {
            _cache.Clear();
            var result = _service.Delete(id);
            if (!result)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie.") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Pracownik został usunięty"));
        }
    }
}
