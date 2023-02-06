using AutoMapper;
using Caching.Interfaces;
using Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DbEntities;
using Models.DTOs.Category;
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
    public class MainCategoryController : ControllerBase
    {
        private readonly IMainCategoryService _service;
        private readonly IMapper _mapper;
        private readonly ICacheManager _cache;

        public MainCategoryController(IMainCategoryService service, IMapper mapper, ICacheManager cache)
        {
            _service = service;
            _mapper = mapper;
            _cache = cache;
        }

        [Cached(2)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<MainCategoryDto>>))]
        public async Task<IActionResult> GetAll()
        {
            var data = _mapper
                .Map<List<MainCategory>, List<MainCategoryDto>>(await _service.GetAll());
            return Ok(new BaseResponse<IEnumerable<MainCategoryDto>>(data, $"Lista głównych kategorii"));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<MainCategoryDto>))]
        public async Task<IActionResult> GetOne(int id)
        {
            var data = _mapper
                .Map<MainCategory, MainCategoryDto>(await _service.Get(id));
            if(data == null) 
                throw new ApiException("Kategoria nie istnieje") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<MainCategoryDto>(data, $"Kategoria o numerze id {id}"));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Add([FromBody] MainCategoryRequest request)
        {
            _cache.Clear();
            var result = _service.Add(_mapper.Map<MainCategoryRequest, MainCategory>(request));
            if (result == null)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie.") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Główna kategoria została dodana"));
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Update([FromBody] MainCategoryDto request)
        {
            _cache.Clear();
            var result = await _service.Update(_mapper.Map<MainCategoryDto, MainCategory>(request));
            if (!result)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie.") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Główna kategoria została zaaktualizowana"));
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Delete(int id)
        {
            _cache.Clear();
            var result = _service.Delete(id);
            if (!result)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie.") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Główna kategoria została usunięta"));
        }
    }
}
