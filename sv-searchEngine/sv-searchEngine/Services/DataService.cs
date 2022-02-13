
using Newtonsoft.Json;
using sv_searchEngine.Models;
using sv_searchEngine.Models.Attribute;
using sv_searchEngine.Models.DTOS;
using System.Collections.Generic;
using System.Net;

namespace sv_searchEngine.Services
{
    public interface IDataService
    {
        Task<EntityModel?> GetAllData();
    }
    public class DataService : IDataService
    {
        private const string DataFileLink = "https://simonsvoss-homework.herokuapp.com/sv_lsm_data.json";
        EntityModel? _allData = null;

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
                            _allData = JsonConvert.DeserializeObject<EntityModel>(stringData);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Cannot read the data file due to:{e.InnerException}");
            }
        }

        public async Task<EntityModel?> GetAllData()
        {
            await ReadDataFromJsonFile();
            return _allData;
        }
    }
}
