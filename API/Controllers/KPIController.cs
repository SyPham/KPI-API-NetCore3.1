
using Models.EF;
using Models.ViewModels.KPILevel;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Threading.Tasks;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class KPIController : ControllerBase
    {
        private readonly IKPIService _KPIService;
        private readonly ILevelService _levelService;
        private readonly IKPILevelService _kpiLevelService;
        private readonly ICategoryService _categoryService;

        public KPIController(IKPIService KPIService,
            ILevelService levelService,
            IKPILevelService kpiLevelService,
            ICategoryService categoryService)
        {
            _KPIService = KPIService;
            _levelService = levelService;
            _kpiLevelService = kpiLevelService;
            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _levelService.GetListTree());
        }
        [HttpGet("{kpilevelcode}/{period}")]
        public async Task<ActionResult> Period(string kpilevelcode, string period)
        {
            return Ok(await _KPIService.GetAllAjax(kpilevelcode, period));
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetListTreeClient(int id)
        {
            return Ok(_levelService.GetListTreeClient(id));
        }


        [HttpGet("{level}/{category}")]
        [HttpGet("{level}/{category}/{page}/{pageSize}")]
        public IActionResult LoadDataKPILevel(int level, int category, int page = ConstantCommon.PAGE, int pageSize = ConstantCommon.PAGE_SIZE)
        {
            return Ok(_kpiLevelService.LoadDataForUser(level, category, page, pageSize));
        }
        [HttpGet]
        public IActionResult GetCategoryCode()
        {
            return Ok(_categoryService.GetAll());
        }
        [HttpPost]
        public IActionResult UpdateKPILevel([FromBody]KPILevelForUpdate entity)
        {
            return Ok(_kpiLevelService.Update(entity));
        }
        [HttpGet("{dataid}/{userid}")]
        public IActionResult LoadDataComment(int dataid, int userid)
        {
            return Ok(_KPIService.ListComments(dataid, userid));
        }

        [HttpPost]
        public IActionResult AddKPI([FromBody]KPI entity)
        {
            return Ok(_KPIService.Add(entity));
        }
        [HttpGet("{category}")]
        [HttpGet("{category}/{page}/{pageSize}")]
        public IActionResult GetAllKPILevel(int category, int page = ConstantCommon.PAGE, int pageSize = ConstantCommon.PAGE_SIZE)
        {
            return Ok(_kpiLevelService.GetAllPaging(category, page, pageSize));
        }
    }
}