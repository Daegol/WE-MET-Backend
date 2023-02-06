using AutoMapper;
using Caching.Interfaces;
using Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DbEntities;
using Models.DTOs.Client;
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
    public class ClientController : ControllerBase
    {
        private readonly IClientService _service;
        private readonly IMapper _mapper;
        private readonly ICacheManager _cache;

        public ClientController(IClientService service, IMapper mapper, ICacheManager cache)
        {
            _service = service;
            _mapper = mapper;
            _cache = cache;
        }

        [Cached(2)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<ClientDto>>))]
        public async Task<IActionResult> GetAll()
        {
            var data = _mapper
                .Map<List<Client>, List<ClientDto>>(await _service.GetAll());
            return Ok(new BaseResponse<IEnumerable<ClientDto>>(data, $"Lista wszystkich klientów"));
        }

        [Cached(2)]
        [HttpGet("getAllIndividualClients")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<IndividualClientDto>>))]
        public async Task<IActionResult> GetAllIndividualClients()
        {
            var data = _mapper
                .Map<List<Client>, List<IndividualClientDto>>(await _service.GetAllIndividuals());
            return Ok(new BaseResponse<IEnumerable<IndividualClientDto>>(data, $"Lista wszystkich klientów indywidualnych"));
        }

        [Cached(2)]
        [HttpGet("getAllCompanyClients")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<CompanyClientDto>>))]
        public async Task<IActionResult> GetAllCompanylClients()
        {
            var data = _mapper
                .Map<List<Client>, List<CompanyClientDto>>(await _service.GetAllCompany());
            return Ok(new BaseResponse<IEnumerable<CompanyClientDto>>(data, $"Lista wszystkich klientów firmowych"));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Add([FromBody] ClientRequest request)
        {
            _cache.Clear();
            if (request.CompanyName != null && (request.FirstName != null || request.LastName != null))
            {
                throw new ApiException("Nie można podać jednocześnie nazwy firmy oraz imienia i nazwiska") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            var result = _service.Add(_mapper.Map<ClientRequest, Client>(request));
            if (result == null)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie.") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Nowy klient został dodany"));
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Update([FromBody] ClientUpdateRequest request)
        {
            _cache.Clear();
            if (request.CompanyName != null && (request.FirstName != null || request.LastName != null))
            {
                throw new ApiException("Nie można podać jednocześnie nazwy firmy oraz imienia i nazwiska") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            var result = await _service.Update(_mapper.Map<ClientUpdateRequest, Client>(request));
            if (!result)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie.") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Klient został zaaktualizowany"));
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> Delete(int id)
        {
            _cache.Clear();
            var result = _service.Delete(id);
            if (!result)
                throw new ApiException("Coś poszło nie tak. Spróbuj ponownie.") { StatusCode = (int)HttpStatusCode.BadRequest };
            return Ok(new BaseResponse<string>("Klient został usunięty"));
        }
    }
}
