using Models.EF;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using API.Helpers;
using System.Linq;
using Service.Helpers;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class MenusController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenusController(IMenuService menuService)
        {
            _menuService = menuService;
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody]Menu entity)
        {
            if (await _menuService.Add(entity))
                return Ok();
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> Update([FromBody]Menu entity)
        {
            if (await _menuService.Update(entity))
                if (await _menuService.Update(entity))
                    return Ok();
            return BadRequest();
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            if (await _menuService.Remove(Id))
                return Ok();
            return BadRequest();
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(int Id)
        {

            return Ok(await _menuService.GetById(Id));
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            string token = Request.Headers["Authorization"];
            var roleId = Extensions.DecodeToken(token).FirstOrDefault(x => x.Type == "Role").Value.ToInt();
            return Ok(await _menuService.GetMenusAsync(roleId));
        }
        [HttpGet("{keyword}")]
        [HttpGet("{page}/{pageSize}/{keyword}")]
        public async Task<IActionResult> GetAllPaging(string keyword, int page = ConstantCommon.PAGE, int pageSize = ConstantCommon.PAGE_SIZE)
        {
            return Ok(await _menuService.GetAllPaging(keyword, page, pageSize));
        }

        [HttpGet("{langBy}")]
        public IActionResult ChangeLocale(string langBy)
        {
            string token = Request.Headers["Authorization"];
            var role = Extensions.DecodeToken(token).FirstOrDefault(x => x.Type == "Role").Value.ToInt();
            return Ok(role);

        }
    }
}
