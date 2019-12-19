
using API.Helpers;
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
        [HttpPost]
        public async Task<IActionResult> AddUserToLevel(int id, int levelid)
        {
            return Ok(await _userService.AddUserToLevel(id, levelid));
        }
        [HttpGet("{levelid}")]
        [HttpGet("{levelid}/{code}/{page}/{pageSize}")]
        public async Task<IActionResult> LoadDataUser(int levelid, string code, int page = ConstantCommon.PAGE, int pageSize = ConstantCommon.PAGE_SIZE)
        {
            
            return Ok(await _userService.LoadDataUser(levelid, code, page, pageSize));
        }
    }
}