using AutoMapper;
using Caching.Interfaces;
using Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DbEntities;
using Models.DTOs.SaleItem;
using Models.ResponseModels;
using Services.Interfaces;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;


namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SaleItemController : ControllerBase
    {
        private readonly ISaleItemService _service;
        private readonly IMapper _mapper;
        private readonly ICacheManager _cache;

        public SaleItemController(ISaleItemService service, IMapper mapper, ICacheManager cache)
        {
            _service = service;
            _mapper = mapper;
            _cache = cache;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<SaleItemDto>>))]
        public async Task<IActionResult> GetAll()
        {
            var data = _mapper
                .Map<List<SaleItem>, List<SaleItemDto>>(await _service.GetAll());
            return Ok(new BaseResponse<IEnumerable<SaleItemDto>>(data, $"Lista wszystkich przedmiotów"));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<SaleItemDto>))]
        public async Task<IActionResult> GetOne(int id)
        {
            var data = _mapper
                .Map<SaleItem, SaleItemDto>(await _service.Get(id));
            if (data == null)
                throw new ApiException("Przedmiot nie istnieje") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<SaleItemDto>(data, $"Przedmiot o numerze id {id}"));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Add([FromBody] SaleItemInsertRequest request)
        {
            _cache.Clear();
            var result = _service.Add(_mapper.Map<SaleItemInsertRequest, SaleItem>(request));
            if (result == null)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie.") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Przedmiot został dodany"));
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Update([FromBody] SaleItemUpdateRequest request)
        {
            _cache.Clear();
            var result = await _service.Update(_mapper.Map<SaleItemUpdateRequest, SaleItem>(request));
            if (!result)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie.") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Przedmiot został zaaktualizowany"));
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Delete(int id)
        {
            _cache.Clear();
            var result = _service.Delete(id);
            if (!result)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie.") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Przedmiot został usunięty"));
        }
    }
}
