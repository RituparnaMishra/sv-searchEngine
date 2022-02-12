using Microsoft.AspNetCore.Mvc;
using sv_searchEngine.Services;

namespace sv_searchEngine.Controllers
{
    public class EntityController : Controller
    {
        private readonly IDataService _dataService;
        public EntityController(IDataService dataService)
        {
            _dataService = dataService;
        }
        public async Task<ActionResult> Index(string searchCriteria)
        {
            var data = await _dataService.GetDataWithSearchCriteria(searchCriteria);
            return View(data);
        }
    }
}
