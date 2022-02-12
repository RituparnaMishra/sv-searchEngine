
using Newtonsoft.Json;
using sv_searchEngine.Models;
using System.Net;

namespace sv_searchEngine.Services
{
    public interface IDataService
    {
        Task<EntityModel?> GetDataWithSearchCriteria(string searchCriteria);
    }
    public class DataService : IDataService
    {
        private const string DataFileLink = "https://simonsvoss-homework.herokuapp.com/sv_lsm_data.json";
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

        public async Task<EntityModel?> GetDataWithSearchCriteria(string searchCriteria)
        {
            await ReadDataFromJsonFile();
            if (!string.IsNullOrEmpty(searchCriteria) && _data != null)
            {
                _data.Buildings = _data.Buildings.Where(b => b.Name.Contains(searchCriteria, StringComparison.OrdinalIgnoreCase)).ToList();
                if (_data.Buildings.Any())
                {
                    _data.Locks = _data.Locks.Where(l => _data.Buildings.Select(b => b.Id).Contains(l.BuildingId)).ToList();
                }
            }


            return _data;
        }

    }
}
