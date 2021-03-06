﻿

using Models.EF;
using Models.ViewModels.Level;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class AdminLevelController : ControllerBase
    {

        private readonly ILevelService _levelService;

        public AdminLevelController(
            ILevelService levelService )
        {
            _levelService = levelService;
        }
        /// <summary>
        /// Lấy ra danh sách oc theo tree view. URL: /AdminLevel/GetListTree
        /// </summary>
        /// <returns>Tất cả danh sách</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetListTree()
        {
            return Ok(await _levelService.GetListTree());
        }
       
        [HttpPost]
        public async Task<IActionResult> AddOrUpdate([FromBody]Level entity)
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
        public async Task<IActionResult> Rename([FromBody]TreeViewModel level)
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
        public async Task<IActionResult> Add([FromBody]Level level)
        {
            return Ok(await _levelService.Add(level));
        }
    }
}