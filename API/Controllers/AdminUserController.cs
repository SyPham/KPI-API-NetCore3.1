
using Models.EF;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
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

        public async Task<IActionResult> Add(User entity)
        {
            return Ok(await _userService.Add(entity));
        }
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _userService.GetAll());
        }
        [HttpGet]
        public async Task<IActionResult> LoadData(string search, int page, int pageSize)
        {
            return Ok(await _userService.GetAllPaging(search, page, pageSize));
        }
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _userService.Remove(id));
        }
        public async Task<IActionResult> Update(User entity)
        {
            return Ok(await _userService.Update(entity));
        }
        public async Task<IActionResult> GetbyID(int ID)
        {
            return Ok(await _userService.GetById(ID));
        }
        public async Task<IActionResult> LockUser(int ID)
        {
            return Ok(await _userService.LockUser(ID));
        }
        //update kpiLevel
        //public async Task<IActionResult> UpdateKPILevel(KPILevelForUpDate entity)
        //{
        //    return Ok(await _kpileveldao.Update(entity));
        //}
        ////get all kpilevel
        //public async Task<IActionResult> GetAllKPILevel()
        //{
        //    return Ok(await _kpileveldao.GetAll());
        //}
        public async Task<IActionResult> GetListAllPermissions(int userid)
        {
            return Ok(await _userService.GetListAllPermissions(userid));
        }
        public async Task<IActionResult> GetAllMenus()
        {
            return Ok(await _menuService.GetAll());
        }
        public async Task<IActionResult> Checkpermisson(int userid)
        {
            return Ok(await _userService.Checkpermisson(userid));
        }
        public async Task<IActionResult> ChangePassword(string username, string password)
        {
            return Ok(await _userService.ChangePassword(username, password));
        }

    }
}