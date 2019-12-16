
using Models.EF;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Add(Setting entity)
        {
            if (await _settingService.Add(entity))
                return Ok();
            return BadRequest();
        }
        public async Task<IActionResult> Update(Setting entity)
        {
            if (await _settingService.Update(entity))
                return Ok();
            return BadRequest();
        }
        public async Task<IActionResult> Delete(int Id)
        {
            if (await _settingService.Remove(Id))
                return Ok();
            return BadRequest();
        }
        public async Task<IActionResult> Detail(int Id)
        {
            return Ok(await _settingService.GetById(Id));
        }
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _settingService.GetAll());

        }
        public async Task<IActionResult> GetAllPaging(string keyword, int page, int pageSize)
        {
            return Ok(await _settingService.GetAllPaging(keyword, page, pageSize));

        }
    }
}
