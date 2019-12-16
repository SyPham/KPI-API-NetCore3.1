
using Models.EF;
using Models.ViewModels.KPILevel;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class AdminKPILevelController : ControllerBase
    {
        private readonly IKPILevelService _KPILevelService;
        private readonly ILevelService _levelService;

        public AdminKPILevelController(IKPILevelService KPILevelService, ILevelService levelService)
        {
            _KPILevelService = KPILevelService;
            _levelService = levelService;
        }
        // GET: AdminKPILevel
        /// <summary>
        /// Lấy ra danh sách OC theo tree view
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetListTree()
        {
            return Ok(await _levelService.GetListTree());
        }

        public async Task<IActionResult> LoadDataKPILevel(int level, int category, int page, int pageSize)
        {
            return Ok(await _KPILevelService.LoadData(level, category, page, pageSize));
        }

        public async Task<IActionResult> GetAll(int level, int category, int page, int pageSize)
        {
            return Ok(await _KPILevelService.GetAllPaging(level, category, page, pageSize));
        }
        //public async Task<IActionResult> GetCategoryCode(Category entity)
        //{
        //    return Ok(await _KPILevelService.GetAllCategory());
        //}
        //update kpiLevel
        public async Task<IActionResult> UpdateKPILevel(KPILevelForUpdate entity)
        {
            return Ok(await _KPILevelService.Update(entity));
        }

        //update kpiLevel
        [HttpPost]
        public async Task<IActionResult> Update(KPILevelForUpdate entity)
        {
            return Ok(await _KPILevelService.UpdateKPILevel(entity));
        }
        public async Task<IActionResult> GetbyID(int ID)
        {
            return Ok(await _KPILevelService.GetById(ID));
        }
        //public async Task<IActionResult> GetListAllUser()
        //{
        //    return Ok(await new UserAdminDAO().GetListAllUser());
        //}
    }
}