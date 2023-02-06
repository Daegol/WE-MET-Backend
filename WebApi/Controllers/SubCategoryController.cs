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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebApi.Attributes;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {
        private readonly ISubCategoryService _service;
        private readonly IMapper _mapper;
        private readonly ICacheManager _cache;

        public SubCategoryController(ISubCategoryService service, IMapper mapper, ICacheManager cache)
        {
            _service = service;
            _mapper = mapper;
            _cache = cache;
        }

        [Cached(2)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<SubCategoryDto>>))]
        public async Task<IActionResult> GetAll()
        {
            var data = _mapper
                .Map<List<SubCategory>, List<SubCategoryDto>>(await _service.GetAll());
            return Ok(new BaseResponse<IEnumerable<SubCategoryDto>>(data, $"Lista wszystkich podkategorii"));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<SubCategoryDto>))]
        public async Task<IActionResult> GetOne(int id)
        {
            var data = _mapper
                .Map<SubCategory, SubCategoryDto>(await _service.Get(id));
            if (data == null)
                throw new ApiException("Podkategoria nie istnieje") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<SubCategoryDto>(data, $"Podkategoria o id {id}"));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Add([FromBody] SubCategoryRequest request)
        {
            _cache.Clear();
            var result = _service.Add(_mapper.Map<SubCategoryRequest, SubCategory>(request));
            if (result == null)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Podkategoria została dodana"));
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Update([FromBody] SubCategoryDto request)
        {
            _cache.Clear();
            var result = await _service.Update(_mapper.Map<SubCategoryDto, SubCategory>(request));
            if (!result)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Podkategoria została zaaktualizowana"));
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Delete(int id)
        {
            _cache.Clear();
            var result = _service.Delete(id);
            if (!result)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Podkategoria została usunięta"));
        }
    }
}
