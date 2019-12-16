using Microsoft.AspNetCore.Mvc;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ActionPlanController : ControllerBase
    {
        private readonly IActionPlanService _actionPlanService;
        private readonly ICategoryService _categoryService;

        public ActionPlanController(IActionPlanService actionPlanService, ICategoryService categoryService)
        {
            _actionPlanService = actionPlanService;
            _categoryService = categoryService;
        }

        // GET: ActionPlan/GetActionPlanByCategory
        public async Task<IActionResult> GetAllById(int? catid)
        {

            return Ok(await _actionPlanService.GetAllById(catid ?? 0));
        }

        public async Task<IActionResult> GetAllCategory()
        {

            return Ok(await _categoryService.GetAll());
        }
    }
}