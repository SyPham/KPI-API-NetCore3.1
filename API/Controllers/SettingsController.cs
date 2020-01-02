
using Models.EF;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Threading.Tasks;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingService _settingService;

        public SettingsController(ISettingService settingService)
        {
            _settingService = settingService;
        }


        // GET: Settings
        [HttpPost]
        public async Task<IActionResult> Add([FromBody]Setting entity)
        {
            if (await _settingService.Add(entity))
                return Ok();
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> Update([FromBody]Setting entity)
        {
            if (await _settingService.Update(entity))
                return Ok();
            return BadRequest();
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            if (await _settingService.Remove(Id))
                return Ok();
            return BadRequest();
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> Detail(int Id)
        {
            return Ok(await _settingService.GetById(Id));
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _settingService.GetAll());

        }
        [HttpGet("{keyword}")]
        [HttpGet("{page}/{pageSize}/{keyword}")]
        public async Task<IActionResult> GetAllPaging(string keyword, int page = ConstantCommon.PAGE, int pageSize = ConstantCommon.PAGE_SIZE)
        {
            return Ok(await _settingService.GetAllPaging(keyword, page, pageSize));
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ShowInfo()
        {
            return Ok(await _settingService.ShowInfo());

        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Maintain()
        {
            return Ok(await _settingService.Maintain());

        }
    }
}
