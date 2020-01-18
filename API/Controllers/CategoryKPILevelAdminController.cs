

using Models.EF;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Threading.Tasks;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using API.Dto.CategoryKPILevelAdmin;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class CategoryKPILevelAdminController : ControllerBase
    {
        private readonly ICategoryKPILevelService _categoryKPILevelService;

        public CategoryKPILevelAdminController(ICategoryKPILevelService categoryKPILevelService)
        {
            _categoryKPILevelService = categoryKPILevelService;
        }

        [HttpGet("{level}/{OCID}")]
        [HttpGet("{level}/{OCID}/{page}/{pageSize}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCategories(int level, int OCID, int page = ConstantCommon.PAGE, int pageSize = ConstantCommon.PAGE_SIZE)
        {
            return Ok(await _categoryKPILevelService.GetCategoryByOC(page, pageSize, level, OCID));
        }

        [HttpGet("{page}/{pageSize}")]
        public async Task<IActionResult> GetAllKPIlevels(int page, int pageSize)
        {
            return Ok(await _categoryKPILevelService.GetAllKPIlevels(page, pageSize));
        }
        [HttpGet("{level}/{category}")]
        [HttpGet("{level}/{category}/{page}/{pageSize}")]
        [AllowAnonymous]
        public async Task<IActionResult> LoadDataKPILevel(int level, int category, int page = ConstantCommon.PAGE, int pageSize = ConstantCommon.PAGE_SIZE)
        {
            return Ok(await _categoryKPILevelService.LoadDataKPILevel(level, category, page, pageSize));
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody]CategoryKPILevel entity)
        {
            return Ok(await _categoryKPILevelService.Add(entity));
        }
        [HttpPost]
        public async Task<IActionResult> AddGeneral([FromBody] AddGeneralDto entity)
        {
            return Ok(await _categoryKPILevelService.AddGeneral(entity.kpilevel, entity.category, entity.pic, entity.owner, entity.manager, entity.sponsor, entity.participant));
        }
        [HttpGet("{KPILevelID}/{CategoryID}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserByCategoryIDAndKPILevelID(int KPILevelID, int CategoryID)
        {
            return Ok(await _categoryKPILevelService.GetUserByCategoryIDAndKPILevelID(CategoryID, KPILevelID));
        }
        [HttpPost]
        public async Task<IActionResult> RemoveCategoryKPILevel([FromBody]int KPILevelID, int CategoryID)
        {
            return Ok(await _categoryKPILevelService.RemoveCategoryKPILevel(CategoryID, KPILevelID));
        }

    }
}