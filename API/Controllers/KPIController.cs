
using Models.EF;
using Models.ViewModels.KPILevel;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;

namespace API.Controllers
{
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
        public async Task<ActionResult> Period(string kpilevelcode, string period)
        {
            return Ok(await _KPIService.GetAllAjax(kpilevelcode, period));
        }
        public IActionResult GetListTreeClient(int id)
        {
            return Ok(_levelService.GetListTreeClient(id));
        }


        public IActionResult LoadDataKPILevel(int level, int category, int page, int pageSize)
        {
            return Ok(_kpiLevelService.LoadDataForUser(level, category, page, pageSize));
        }
        public IActionResult GetCategoryCode(Category entity)
        {
            return Ok(_categoryService.GetAll());
        }
        public IActionResult UpdateKPILevel(KPILevelForUpdate entity)
        {
            return Ok(_kpiLevelService.Update(entity));
        }

        //public IActionResult AddComment(Model.EF.Comment entity)
        //{
        //    var value = entity.KPILevelCode;
        //    entity.KPILevelCode = value.Substring(0, value.Length - 1);
        //    entity.Period = value.Substring(value.Length - 1, 1).ToUpper();
        //    return Ok(_KPIService.AddComment(entity));
        //}

        public IActionResult LoadDataComment(int dataid, int userid)
        {
            return Ok(_KPIService.ListComments(dataid, userid));
        }

        public IActionResult AddKPI(KPI entity)
        {
            return Ok(_KPIService.Add(entity));
        }
        public IActionResult GetAllKPILevel(int category, int page, int pageSize)
        {
            return Ok(_kpiLevelService.GetAllPaging(category, page, pageSize));
        }
    }
}