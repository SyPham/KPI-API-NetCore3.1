using Models.EF;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using API.Helpers;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class AdminKPIController : ControllerBase
    {
        private readonly IKPIService _KPIService;

        public AdminKPIController(IKPIService KPIService)
        {
            _KPIService = KPIService;
        }
        /// <summary>
        /// Thêm KPI
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>True or False</returns>
        [HttpPost]
        public async Task<IActionResult> Add(KPI entity)
        {
            return Ok(await _KPIService.Add(entity));
        }
        public async Task<IActionResult> AddKPILevel(KPILevel entity)
        {
            return Ok(await _KPIService.AddKPILevel(entity));
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _KPIService.GetAll());
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _KPIService.Delete(id));
        }
        [HttpPost]
        public async Task<IActionResult> Update(KPI entity)
        {
            return Ok(await _KPIService.Update(entity));
        }
        [HttpGet]
        [Route("{ID}")]
        public async Task<IActionResult> GetbyID(int ID)
        {
            return Ok(await _KPIService.GetById(ID));
        }
        /// <summary>
        /// Lấy danh sách theo phân trang
        /// </summary>
        /// <param name="catID">Khóa chỉnh của bảng category</param>
        /// <param name="name">Keyword để lọc dữ liệu</param>
        /// <param name="page">Số trang</param>
        /// <param name="pageSize">Số dòng trên 1 trang</param>
        /// <returns>Trả về danh sách dữ liệu đã được phân trang</returns>
        /// 
        [HttpGet("{catID}")]
        [HttpGet("{catID}/{name}")]
        [HttpGet("{name}/{page}/{pageSize}")]
        [HttpGet("{catID}/{page}/{pageSize}")]
        [HttpGet("{catID}/{name}/{page}/{pageSize}")]
        public async Task<IActionResult> LoadData(int? catID, string name = "", int page = ConstantCommon.PAGE, int pageSize = ConstantCommon.PAGE_SIZE)
        {
            return Ok(await _KPIService.GetAllPaging(catID, name, page, pageSize));
        }
        [HttpGet("{name}")]
        public async Task<IActionResult> Autocomplete(string name)
        {
            return Ok(await _KPIService.Autocomplete(name));
        }
    }
}