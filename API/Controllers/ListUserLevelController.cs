

using Models;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;
using API.Helpers;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ListUserLevelController : ControllerBase
    {
        private readonly DataContext _dbContext;
        private readonly IUserService _userService;

        public ListUserLevelController(DataContext dbContext,IUserService userService)
        {
            _dbContext = dbContext;
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