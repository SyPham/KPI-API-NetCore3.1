

using Models;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Threading.Tasks;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class ListUserLevelController : ControllerBase
    {
        private readonly IUserService _userService;

        public ListUserLevelController( IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{teamid}/{code}")]
        [HttpGet("{teamid}/{code}/{page}/{pageSize}")]
        public async Task<IActionResult> LoadDataUser(int teamid, string code, int page = ConstantCommon.PAGE, int pageSize = ConstantCommon.PAGE_SIZE)
        {
            return Ok(await _userService.LoadDataUser(teamid, code, page, pageSize));
        }
    }
}