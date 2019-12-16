
using Models.EF;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;

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
         [HttpGet]
        public async Task<IActionResult> GetAllCategories(int page, int pageSize, int level,int ocID)
        {
            return Ok(await _categoryService.GetAllByCategory(page, pageSize, level, ocID));
        }
        [HttpGet]
        public async Task<IActionResult> GetAllKPIlevels(int page, int pageSize)
        {
            return Ok(await _kPILevelService.GetAll(page, pageSize));
        }
        [HttpPost]
        public async Task<IActionResult> Add(CategoryKPILevel entity)
        {
            return Ok(await _categoryKPILevelService.Add(entity));
        }
        [HttpGet]
        public async Task<IActionResult> GetAllKPILevelByCategory(int category, int page, int pageSize)
        {
            return Ok(await _categoryKPILevelService.LoadKPILevel(category, page, pageSize));

        }
    }
}