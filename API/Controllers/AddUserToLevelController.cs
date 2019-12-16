
using Microsoft.AspNetCore.Mvc;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AddUserToLevelController : ControllerBase
    {
        private readonly IUserService _userService;

        public AddUserToLevelController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: AddUserToLevel
        public async Task<IActionResult> AddUserToLevel(int id, int levelid)
        {
            return Ok(await _userService.AddUserToLevel(id, levelid));
        }
        public async Task<IActionResult> LoadDataUser(int levelid, string code, int page, int pageSize)
        {
            return Ok(await _userService.LoadDataUser(levelid, code, page, pageSize));
        }
    }
}