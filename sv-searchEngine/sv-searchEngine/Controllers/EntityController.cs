using Microsoft.AspNetCore.Mvc;
using sv_searchEngine.Models;
using sv_searchEngine.Services;

namespace sv_searchEngine.Controllers
{
    public class EntityController : Controller
    {
        private readonly IDataService _dataService;
        private readonly ISearchService _searchService;
        private EntityModel? _allData = null;
        public EntityController(IDataService dataService, ISearchService searchService)
        {
            _dataService = dataService;
            _searchService = searchService;
        }
        public async Task<ActionResult> Index(string searchCriteria)
        {
            if (_allData == null)
            {
                _allData = await _dataService.GetAllData();
            }

            if (!string.IsNullOrEmpty(searchCriteria) && _allData != null)
            {
                var data = _searchService.GetSearchedResults(searchCriteria, _allData);


                return View(data);

            }
            else
            {
                return View(_allData);
            }
        }

    }
}
