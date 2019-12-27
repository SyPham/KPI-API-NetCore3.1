
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class CompareController : ControllerBase
    {
        private readonly IDataService _dataService;


        public CompareController(IDataService dataService)
        {
            _dataService = dataService;
        }

        // GET: Compare

        [HttpGet("{obj}")]
        public IActionResult Compare(string obj)
        {
       
            if (obj == null)
                return BadRequest();
            //var value = obj.Split(';')[1].Split(',');
            //var standard = value[0].ToInt();
            //var unit = value[1].ToString();
            var comp = obj.Split(';')[0].ToString();

          
            var chartVM2s = _dataService.Compare2(comp);
            //var compare2 = new DataChartDAO().Compare2(comp);

            //ViewBag.ChartVM2s = chartVM2s;
            //if (chartVM2s[0].period == "W") { ViewBag.PeriodText = "Weekly"; ViewBag.Period = chartVM2s[0].period; };
            //if (chartVM2s[0].period == "M") { ViewBag.PeriodText = "Monthly"; ViewBag.Period = chartVM2s[0].period; }
            //if (chartVM2s[0].period == "Q") { ViewBag.PeriodText = "Quarterly"; ViewBag.Period = chartVM2s[0].period; }
            //if (chartVM2s[0].period == "Y") { ViewBag.PeriodText = "Yearly"; ViewBag.Period = chartVM2s[0].period; }
            //ViewBag.Standard = standard;
            //ViewBag.Unit = unit;

            return Ok(chartVM2s);
        }
    }
}