using AutoMapper;
using Caching.Interfaces;
using Identity.Models;
using Identity.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Account;
using Models.Enums;
using Models.ResponseModels;
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
    public class AdminController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        private readonly ICacheManager _cache;
        public AdminController(IAccountService accountService, IMapper mapper, ICacheManager cache)
        {
            _accountService = accountService;
            _mapper = mapper;
            _cache = cache;
        }

        [Cached(2)]
        [Authorize(Policy = "OnlyAdmins")]
        [HttpGet("alluser")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<UserDto>>))]
        public async Task<IActionResult> GetAllUser()
        {
            var userList = await _accountService.GetUsers();
            var data = _mapper
                .Map<IReadOnlyList<ApplicationUser>, IReadOnlyList<UserDto>>(userList);

            return Ok(new BaseResponse<IReadOnlyList<UserDto>>(data, $"User List"));
        }

        [Cached(2)]
        [Authorize(Policy = "OnlyAdmins")]
        [HttpGet("oneuser/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<UserDto>))]
        public async Task<IActionResult> GetOneUser(int id)
        {
            var user = await _accountService.GetOneUser(id);
            var data = _mapper
                .Map<ApplicationUser, UserDto>(user);

            return Ok(new BaseResponse<UserDto>(data, $"User"));
        }

        [Cached(2)]
        [Authorize(Policy = "OnlyAdmins")]
        [HttpGet("oneuserwithroles/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<UserDto>))]
        public async Task<IActionResult> GetOneUserWithRoles(int id)
        {
            var user = await _accountService.GetOneUserWithRoles(id);

            var result = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = user.UserRoles.ToList().Select(y => y.Role.Name.ToString()).ToList()
            };

            return Ok(new BaseResponse<UserDto>(result, $"User with roles"));
        }

        [Cached(1)]
        [Authorize(Policy = "OnlyAdmins")]
        [HttpGet("alluserwithroles")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<UserDto>>))]
        public async Task<IActionResult> GetAllUserWithRoles()
        {
            var userList = await _accountService.GetUsers();

            var result = userList.Select(x => new UserDto
            {
                Id = x.Id,
                Email = x.Email,
                UserName = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Roles = x.UserRoles.ToList().Select(y => y.Role.Name.ToString()).ToList()
            });

            return Ok(new BaseResponse<IEnumerable<UserDto>>(result, $"User List"));
        }

        [Cached(1)]
        [Authorize(Policy = "OnlyAdmins")]
        [HttpGet("allroles")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<string>>))]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = new List<string>();
            foreach(var role in Enum.GetNames(typeof(Roles)))
            {
                result.Add(role);
            }

            return Ok(new BaseResponse<IEnumerable<string>>(result, $"Lista ról"));
        }

        [Authorize(Policy = "OnlyAdmins")]
        [HttpDelete("deleteuser/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> DeleteUser(int id)
        {
            _cache.Clear();
            return Ok(await _accountService.DeleteUser(id));
        }

        [Authorize(Policy = "OnlyAdmins")]
        [HttpPost("updateUser")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<string>))]
        public async Task<IActionResult> UpdateUser(UpdateRequest request)
        {
            _cache.Clear();
            return Ok(await _accountService.UpdateUser(request));
        }


    }
}
