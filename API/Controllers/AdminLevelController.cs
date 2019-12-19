

using Models.EF;
using Models.ViewModels.Level;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AdminLevelController : ControllerBase
    {

        private readonly ILevelService _levelService;
        private readonly ILogger<AdminLevelController> _logger;

        public AdminLevelController(
            ILevelService levelService,
            ILogger<AdminLevelController> logger)
        {
            _levelService = levelService;
            _logger = logger;


        }
        /// <summary>
        /// Lấy ra danh sách oc theo tree view. URL: /AdminLevel/GetListTree
        /// </summary>
        /// <returns>Tất cả danh sách</returns>
        [HttpGet]
        public async Task<IActionResult> GetListTree()
        {
            return Ok(await _levelService.GetListTree());
        }
       
        [HttpPost]
        public async Task<IActionResult> AddOrUpdate(Level entity)
        {
            return Ok(await _levelService.AddOrUpdate(entity));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            //string Code = OkConvert.SerializeObject(code);
            return Ok(await _levelService.GetById(id));
        }
        /// <summary>
        /// Chỉnh sửa trên fancytree
        /// </summary>
        /// <param name="level"></param>
        /// <returns>True or False</returns>
        [HttpPost]
        public async Task<IActionResult> Rename(TreeViewModel level)
        {
            //string Code = OkConvert.SerializeObject(code);
            return Ok(await _levelService.Rename(level));
        }
        /// <summary>
        /// Xóa dữ liệu
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True or False</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            //string Code = OkConvert.SerializeObject(code);
            return Ok(await _levelService.Remove(id));
        }
        [HttpPost]
        public async Task<IActionResult> Add(Level level)
        {
            return Ok(await _levelService.Add(level));
        }
    }
}