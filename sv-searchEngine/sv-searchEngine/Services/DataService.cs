
using Newtonsoft.Json;
using sv_searchEngine.Models;
using sv_searchEngine.Models.Attribute;
using sv_searchEngine.Models.DTOS;
using System.Net;

namespace sv_searchEngine.Services
{
    public interface IDataService
    {
        Task<EntityModel?> GetDataWithBasicSearchCriteria(string searchCriteria);
        Task<EntityModel?> GetDataWithAdvancedSearchCriteria(string searchCriteria);
    }
    public class DataService : IDataService
    {
        private const string DataFileLink = "https://simonsvoss-homework.herokuapp.com/sv_lsm_data.json";
        private SearchService _searchService = new SearchService();
        EntityModel _data = new();

        private async Task ReadDataFromJsonFile()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = await client.GetAsync(DataFileLink);
                    if (result != null && result.StatusCode == HttpStatusCode.OK && result.Content != null)
                    {

                        var stringData = await result.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(stringData))
                        {
                            _data = JsonConvert.DeserializeObject<EntityModel>(stringData);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Cannot read the data file due to:{e.InnerException}");
            }
        }

        public async Task<EntityModel?> GetDataWithBasicSearchCriteria(string searchCriteria)
        {
            await ReadDataFromJsonFile();
            if (!string.IsNullOrEmpty(searchCriteria) && _data != null)
            {
                ////var searchedBuildings = _searchService.search(searchCriteria, _data.Buildings);
                ////_data.Buildings = searchedBuildings;
                //SearchForBuildings(searchCriteria);
                _data.Buildings = _data.Buildings.Where(b => b.Name.Contains(searchCriteria, StringComparison.OrdinalIgnoreCase)).ToList();
                if (_data.Buildings.Any())
                {
                    _data.Locks = _data.Locks.Where(l => _data.Buildings.Select(b => b.Id).Contains(l.BuildingId)).ToList();
                }
            }


            return _data;
        }


        public async Task<EntityModel?> GetDataWithAdvancedSearchCriteria(string searchCriteria)
        {
            await ReadDataFromJsonFile();
            if (!string.IsNullOrEmpty(searchCriteria) && _data != null)
            {
                SearchForBuildings(searchCriteria);
                SearchForLocks(searchCriteria);
                _data.Buildings = _data.Buildings.Where(b => b.Weight != 0).OrderBy(b => b.Weight).ToList();
                _data.Locks = _data.Locks.Where(b => b.Weight != 0).OrderBy(b => b.Weight).ToList();
            }
            return _data;
        }

        private void SearchForBuildings(string searchCriteria)
        {

            System.Type type = typeof(Building);
            System.Reflection.PropertyInfo[] properties = type.GetProperties();

            foreach (System.Reflection.PropertyInfo property in properties)
            {
                var weightAttribute = property.GetCustomAttributes(typeof(OwnPropertyWeightAttribute), true).FirstOrDefault();
                if (weightAttribute != null)
                {
                    var weight = ((OwnPropertyWeightAttribute)weightAttribute).Weightage;
                    var buildings = _data.Buildings.Where(b => b.GetType().GetProperty(property.Name).GetValue(b) != null &&
                     b.GetType().GetProperty(property.Name).GetValue(b).ToString().Contains(searchCriteria, StringComparison.OrdinalIgnoreCase));
                    if (buildings.Any())
                    {
                        buildings.ToList().ForEach(b =>
                        {
                            if (b.Weight < weight)
                                b.Weight = weight;
                        });
                    }
                }

            }
        }
        private void SearchForLocks(string searchCriteria)
        {

            System.Type type = typeof(Lock);
            System.Reflection.PropertyInfo[] properties = type.GetProperties();

            foreach (System.Reflection.PropertyInfo property in properties)
            {
                var weightAttribute = property.GetCustomAttributes(typeof(OwnPropertyWeightAttribute), true).FirstOrDefault();
                if (weightAttribute != null)
                {
                    var weight = ((OwnPropertyWeightAttribute)weightAttribute).Weightage;
                    var locks = _data.Locks.Where(l => l.GetType().GetProperty(property.Name).GetValue(l) != null &&
                    l.GetType().GetProperty(property.Name).GetValue(l).ToString()
                    .Contains(searchCriteria, StringComparison.OrdinalIgnoreCase));
                    if (locks.Any())
                    {
                        locks.ToList().ForEach(b =>
                        {
                            if (b.Weight < weight)
                                b.Weight = weight;
                        });
                    }
                }

            }
        }
    }
}
