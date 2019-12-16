using Models.EF;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;

namespace API.Controllers
{
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

        public async Task<IActionResult> Add(Category entity)
        {
            return Ok(await _categoryService.Add(entity));
        }
        public async Task<IActionResult> Update(Category entity)
        {
            return Ok(await _categoryService.Update(entity));
        }
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _categoryService.GetAll());
        }
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _categoryService.Remove(id));
        }

        public async Task<IActionResult> GetbyID(int ID)
        {
            return Ok(await _categoryService.GetById(ID));
        }
        public async Task<IActionResult> LoadData(string name, int page, int pageSize)
        {
            return Ok(await _categoryService.GetAllPaging(name, page, pageSize));
        }
        //public async Task<IActionResult> Autocomplete(string name)
        //{
        //    return Ok(await _categoryService.Autocomplete(name));
        //}
    }
}