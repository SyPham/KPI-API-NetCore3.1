using Models.EF;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Threading.Tasks;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using API.Dto.Category;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class AdminCategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public AdminCategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        // GET: Category
        [HttpPost]
        public async Task<IActionResult> Add( [FromBody]Category entity)
        {
            return Ok(await _categoryService.Add(entity));
        }
        [HttpPost]
        public async Task<IActionResult> Update([FromBody]Category entity)
        {
            return Ok(await _categoryService.Update(entity));
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _categoryService.GetAll());
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _categoryService.Remove(id));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetbyID(int ID)
        {
            return Ok(await _categoryService.GetById(ID));
        }
        [AllowAnonymous]
        [HttpGet("{name}")]
        [HttpGet("{page}/{pageSize}/{name}")]
        public async Task<IActionResult> LoadData(string name, int page = ConstantCommon.PAGE, int pageSize = ConstantCommon.PAGE_SIZE)
        {
            var model = await _categoryService.GetAllPaging(name, page, pageSize);
            return Ok(new
            {
                data = await _categoryService.GetAllPaging(name, page, pageSize),
                page = model.CurrentPage,
                pageSize = model.PageSize,
                pageCount = model.TotalPages,

            });
        }
        [AllowAnonymous]
        [HttpPost("{page}/{pageSize}")]
        [HttpPost("{page}/{pageSize}/{name}")]
        public async Task<IActionResult> LoadData2(string name, int page = ConstantCommon.PAGE, int pageSize = ConstantCommon.PAGE_SIZE)
        {
            var model = await _categoryService.GetAllPaging(name, page, pageSize);
            return Ok( new {
                data = await _categoryService.GetAllPaging(name, page, pageSize),
                page = model.CurrentPage,
                pageSize = model.PageSize,
                pageCount = model.TotalPages,

            });
        }
        //public async Task<IActionResult> Autocomplete(string name)
        //{
        //    return Ok(await _categoryService.Autocomplete(name));
        //}
    }
}