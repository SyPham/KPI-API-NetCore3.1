﻿using Models.EF;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;
using API.Helpers;

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

        [HttpPost]
        public async Task<IActionResult> Add(Category entity)
        {
            return Ok(await _categoryService.Add(entity));
        }
        [HttpPost]
        public async Task<IActionResult> Update(Category entity)
        {
            return Ok(await _categoryService.Update(entity));
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _categoryService.GetAll());
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _categoryService.Remove(id));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetbyID(int ID)
        {
            return Ok(await _categoryService.GetById(ID));
        }
        [HttpGet("{name}")]
        [HttpGet("{name}/{page}/{pageSize}")]
        public async Task<IActionResult> LoadData(string name, int page = ConstantCommon.PAGE, int pageSize = ConstantCommon.PAGE_SIZE)
        {
            return Ok(await _categoryService.GetAllPaging(name, page, pageSize));
        }
        //public async Task<IActionResult> Autocomplete(string name)
        //{
        //    return Ok(await _categoryService.Autocomplete(name));
        //}
    }
}