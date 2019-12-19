

using API.Helpers;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Threading.Tasks;

namespace API.Controllers
{

    [ApiController]
    [Route("[controller]/[action]")]
    public class OCCategoriesController : ControllerBase
    {
        private readonly IOCCategoryService _OCCategoryService;
        private readonly ILevelService _levelService;
        private readonly ICategoryService _categoryService;

        public OCCategoriesController(IOCCategoryService OCCategoryService,
            ILevelService levelService,
            ICategoryService categoryService)
        {
            _OCCategoryService = OCCategoryService;
            _levelService = levelService;
            _categoryService = categoryService;
        }
     
        [HttpGet("{OCID}/{CategoryID}")]
        public async Task<IActionResult> AddOCCategory(int OCID, int CategoryID)
        {
            return Ok(await _OCCategoryService.AddOCCategory(OCID, CategoryID));
        }

        /// <summary>
        /// 
        /// Lấy danh sách Category theo OC. Theo quan hệ (n - n)
        /// SQL:    Select Categories.Name, Categories.ID, OCCategories.Status
        ///         From OCCategories 
        ///         Join Categories on OCCategories.CategoryID = Categories.ID
        ///         Join Levels on OCCategories.OCID = Levels.ID
        ///         Where Levels.LevelNumber = @level
        ///     
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="level">Cấp bậc của OC </param>
        /// <param name="ocID"></param>
        /// <returns></returns>
        [HttpGet("{level}/{ocID}")]
        [HttpGet("{level}/{ocID}/{page}/{pageSize}")]
        public async Task<IActionResult> GetCategoryByOC(int level, int ocID, int page = ConstantCommon.PAGE, int pageSize = ConstantCommon.PAGE_SIZE)
        {
            return Ok(await _categoryService.GetCategoryByOC(page, pageSize, level, ocID));
        }

        /// <summary>
        /// Lấy tất cả danh sách OC theo Tree View
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetListTree()
        {
            return Ok(await _levelService.GetListTree());
        }

    }
}