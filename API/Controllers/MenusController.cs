﻿using Models.EF;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class MenusController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenusController(IMenuService menuService)
        {
            _menuService = menuService;
        }
        [HttpPost]
        public async Task<IActionResult> Add(Menu entity)
        {
            if (await _menuService.Add(entity))
                return Ok();
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> Update(Menu entity)
        {
            if (await _menuService.Update(entity))
                if (await _menuService.Update(entity))
                    return Ok();
            return BadRequest();
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            if (await _menuService.Remove(Id))
                return Ok();
            return BadRequest();
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(int Id)
        {
            
            return Ok(await _menuService.GetById(Id));
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {

            return Ok(await _menuService.GetAll());

        }
        public async Task<IActionResult> GetAllPaging(string keyword, int page, int pageSize)
        {
            return Ok(await _menuService.GetAllPaging(keyword, page, pageSize));

        }

        public async Task<IActionResult> GetPermissions()
        {
            return Ok(await _menuService.GetPermissions());

        }

    }
}
