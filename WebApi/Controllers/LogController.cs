using AutoMapper;
using Data.Mongo.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Log;
using Models.ResponseModels;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Attributes;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILoginLogService _loginLogService;
        private readonly IMapper _mapper;

        public LogController(ILoginLogService loginLogService, IMapper mapper)
        {
            _loginLogService = loginLogService;
            _mapper = mapper;
        }

        [Cached(2)]
        [Authorize(Policy = "OnlyAdmins")]
        [HttpGet("get")]
        public async Task<IActionResult> GetUserAuthLogs(string userName)
        {
            var userList = await _loginLogService.Get(userName);
            var data = _mapper
                .Map<IReadOnlyList<LoginLog>, IReadOnlyList<LogDto>>(userList);

            return Ok(new BaseResponse<IReadOnlyList<LogDto>>(data, $"User Log List"));
        }
    }
}
