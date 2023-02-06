using AutoMapper;
using Caching.Interfaces;
using Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DbEntities;
using Models.DTOs.RecycledItems;
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
    public class RecycledItemController : ControllerBase
    {
        private readonly IRecycledItemService _service;
        private readonly IMapper _mapper;
        private readonly ICacheManager _cache;

        public RecycledItemController(IRecycledItemService service, IMapper mapper, ICacheManager cache)
        {
            _service = service;
            _mapper = mapper;
            _cache = cache;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<RecycledItemDto>>))]
        public async Task<IActionResult> GetAll()
        {
            var data = _mapper
                .Map<List<RecycledItem>, List<RecycledItemDto>>(await _service.GetAll());
            return Ok(new BaseResponse<IEnumerable<RecycledItemDto>>(data, $"Lista wszystkich przedmiotów"));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<RecycledItemDto>))]
        public async Task<IActionResult> GetOne(int id)
        {
            var data = _mapper
                .Map<RecycledItem, RecycledItemDto>(await _service.Get(id));
            if (data == null)
                throw new ApiException("Przedmiot nie istnieje") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<RecycledItemDto>(data, $"Przedmiot o numerze id {id}"));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Add([FromBody] RecycledItemInsertRequest request)
        {
            _cache.Clear();
            var result = _service.Add(_mapper.Map<RecycledItemInsertRequest, RecycledItem>(request));
            if (result == null)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie.") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Przedmiot został dodany"));
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Update([FromBody] RecycledItemUpdateRequest request)
        {
            _cache.Clear();
            var result = await _service.Update(_mapper.Map<RecycledItemUpdateRequest, RecycledItem>(request));
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
