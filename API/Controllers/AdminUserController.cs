
using Models.EF;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Threading.Tasks;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]/[action]")]
    public class AdminUserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMenuService _menuService;

        public AdminUserController(IUserService userService, IMenuService menuService)
        {
            _userService = userService;
            _menuService = menuService;
        }


        // GET: Account
        [HttpPost]
        public async Task<IActionResult> Add([FromBody]User entity)
        {
            return Ok(await _userService.Add(entity));
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _userService.GetAll());
        }
        [AllowAnonymous]
        [HttpPost("{page}/{pageSize}")]
        [HttpPost("{page}/{pageSize}/{name}")]
        public async Task<IActionResult> LoadData(string name, int page = ConstantCommon.PAGE, int pageSize = ConstantCommon.PAGE_SIZE)
        {
            var model = await _userService.GetAllPaging(name, page, pageSize);
            return Ok(new
            {
                data = await _userService.GetAllPaging(name, page, pageSize),
                page = model.CurrentPage,
                pageSize = model.PageSize,
                pageCount = model.TotalPages,

            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _userService.Remove(id));
        }
        [HttpPost]
        public async Task<IActionResult> Update([FromBody]User entity)
        {
            return Ok(await _userService.Update(entity));
        }
        [HttpGet("{ID}")]
        public async Task<IActionResult> GetbyID(int ID)
        {
            return Ok(await _userService.GetById(ID));
        }
        [HttpGet("{ID}")]
        public async Task<IActionResult> LockUser(int ID)
        {
            return Ok(await _userService.LockUser(ID));
        }
        [HttpGet]
        public async Task<IActionResult> GetAllMenus()
        {
            return Ok(await _menuService.GetAll());
        }
        [HttpGet("{userid}")]
        public async Task<IActionResult> Checkpermisson(int userid)
        {
            return Ok(await _userService.Checkpermisson(userid));
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody]string username, string password)
        {
            return Ok(await _userService.ChangePassword(username, password));
        }

    }
}