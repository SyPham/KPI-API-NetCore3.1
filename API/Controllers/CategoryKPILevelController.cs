
using Models.EF;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;
using API.Helpers;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CategoryKPILevelController : ControllerBase
    {
        private readonly ICategoryKPILevelService _categoryKPILevelService;
        private readonly IKPILevelService _kPILevelService;
        private readonly ICategoryService _categoryService;

        public CategoryKPILevelController(ICategoryKPILevelService categoryKPILevelService,
            IKPILevelService kPILevelService,
            ICategoryService categoryService)
        {
            _categoryKPILevelService = categoryKPILevelService;
            _kPILevelService = kPILevelService;
            _categoryService = categoryService;
        }
        // GET: CategoryKPILevelAdmin
        [HttpGet("{ocID}/{level}")]
        [HttpGet("{ocID}/{level}/{page}/{pageSize}")]
        public async Task<IActionResult> GetAllCategories(int level,int ocID, int page = ConstantCommon.PAGE, int pageSize = ConstantCommon.PAGE_SIZE)
        {
            return Ok(await _categoryService.GetAllByCategory(page, pageSize, level, ocID));
        }
        [HttpGet("{page}/{pageSize}")]
        [HttpGet]
        public async Task<IActionResult> GetAllKPIlevels(int page = ConstantCommon.PAGE, int pageSize = ConstantCommon.PAGE_SIZE)
        {
            return Ok(await _kPILevelService.GetAll(page, pageSize));
        }
        [HttpPost]
        public async Task<IActionResult> Add(CategoryKPILevel entity)
        {
            return Ok(await _categoryKPILevelService.Add(entity));
        }
        [HttpGet("{category}")]
        [HttpGet("{category}/{page}/{pageSize}")]
        public async Task<IActionResult> GetAllKPILevelByCategory(int category, int page = ConstantCommon.PAGE, int pageSize = ConstantCommon.PAGE_SIZE)
        {
            return Ok(await _categoryKPILevelService.LoadKPILevel(category, page, pageSize));

        }
    }
}