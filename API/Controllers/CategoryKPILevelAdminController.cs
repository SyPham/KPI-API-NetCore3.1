

using Models.EF;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CategoryKPILevelAdminController : ControllerBase
    {
        private readonly ICategoryKPILevelService _categoryKPILevelService;

        public CategoryKPILevelAdminController(ICategoryKPILevelService categoryKPILevelService)
        {
            _categoryKPILevelService = categoryKPILevelService;
        }

        public async Task<IActionResult> GetAllCategories(int page, int pageSize, int level, int OCID)
        {
            return Ok(await _categoryKPILevelService.GetCategoryByOC(page, pageSize, level, OCID));
        }

        public async Task<IActionResult> GetAllKPIlevels(int page, int pageSize)
        {
            return Ok(await _categoryKPILevelService.GetAllKPIlevels(page, pageSize));
        }
        public async Task<IActionResult> LoadDataKPILevel(int level, int category, int page, int pageSize)
        {
            return Ok(await _categoryKPILevelService.LoadDataKPILevel(level, category, page, pageSize));
        }
        public async Task<IActionResult> Add(CategoryKPILevel entity)
        {
            return Ok(await _categoryKPILevelService.Add(entity));
        }
        public async Task<IActionResult> AddGeneral(int kpilevel, int category, string pic, string owner, string manager, string sponsor, string participant)
        {
            return Ok(await _categoryKPILevelService.AddGeneral(kpilevel, category, pic, owner, manager, sponsor, participant));
        }
        public async Task<IActionResult> GetUserByCategoryIDAndKPILevelID(int KPILevelID, int CategoryID)
        {
            return Ok(await _categoryKPILevelService.GetUserByCategoryIDAndKPILevelID(CategoryID, KPILevelID));
        }
        public async Task<IActionResult> RemoveCategoryKPILevel(int KPILevelID, int CategoryID)
        {
            return Ok(await _categoryKPILevelService.RemoveCategoryKPILevel(CategoryID, KPILevelID));
        }

    }
}