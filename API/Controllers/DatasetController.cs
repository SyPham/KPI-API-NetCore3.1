
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;


namespace API.Controllers
{
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
   
        public async Task<ActionResult> GetAllDataByCategory(int catid, string period, int? start = 0, int? end = 0, int? year = 0)
        {
           
            var datasets =await _dataService.GetAllDataByCategory(catid, period, start, end, year);
            
           
            return Ok(datasets);
        }
    }
}