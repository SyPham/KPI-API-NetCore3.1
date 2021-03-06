﻿
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System;
using System.Threading.Tasks;


namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class DatasetController : ControllerBase
    {
        private readonly IDataService _dataService;

        public DatasetController(IDataService dataService)
        {
            _dataService = dataService;
        }

        // GET: Dataset
        [HttpGet("{catid}/{period}/{start}/{end}/{year}")]
        public async Task<ActionResult> GetAllDataByCategory(int catid, string period, int? start, int? end, int? year)
        {
            year ??= DateTime.Now.Year;
            return Ok(await _dataService.GetAllDataByCategory(catid, period, start, end, year));
        }
    }
}