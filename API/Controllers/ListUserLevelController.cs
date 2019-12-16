

using Models;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;

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

        public async Task<IActionResult> LoadDataUser(int teamid, string code, int page, int pageSize)
        {
            return Ok(await _userService.LoadDataUser(teamid, code, page, pageSize));
        }
    }
}