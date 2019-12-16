
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;


namespace API.Controllers
{
  
    public class FavouriteController : ControllerBase
    {


        private readonly IFavouriteService _favouriteService;


        public FavouriteController(IFavouriteService favouriteService)
        {
            _favouriteService = favouriteService;
        }

        [HttpGet]
        public async Task<IActionResult> LoadData(int userid, int page, int pageSize)
        {
            return Ok(await _favouriteService.GetAllPaging(userid, page, pageSize));
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _favouriteService.Remove(id));
        }

    }
}