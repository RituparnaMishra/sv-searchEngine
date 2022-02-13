using sv_searchEngine.Models;
using sv_searchEngine.Models.Attribute;
using sv_searchEngine.Models.DTOS;

namespace sv_searchEngine.Services
{
    public interface ISearchService
    {
        public EntityModel GetSearchedResults(string searchCriteria, EntityModel allData);
    }

    public class SearchService : ISearchService
    {

        private EntityModel _data=new EntityModel();

        public EntityModel GetSearchedResults(string searchCriteria, EntityModel allData)
        {
            var searchedBuildings = SearchBuildings(searchCriteria,allData);
            var searchedLocks = SearchLocks(searchCriteria,allData, searchedBuildings);

            _data.Buildings = searchedBuildings;
            _data.Locks = searchedLocks;
            return _data;
        }

        private static List<Lock> SearchLocks(string searchCriteria, EntityModel allData, IEnumerable<Building> searchedBuildings)
        {
            var locks = new List<Lock>();
            System.Type type = typeof(Lock);
            System.Reflection.PropertyInfo[] properties = type.GetProperties();

            foreach (System.Reflection.PropertyInfo property in properties)
            {
                var weightAttributes = property.GetCustomAttributes(true);
                foreach (var weightAttribute in weightAttributes)
                {
                    if (weightAttribute is OwnPropertyWeightAttribute)
                    {
                        if (weightAttribute != null)
                        {
                            var weight = ((OwnPropertyWeightAttribute)weightAttribute).Weightage;
                            var searchedLocks = allData.Locks.Where(l => l.GetType().GetProperty(property.Name).GetValue(l) != null &&
                            l.GetType().GetProperty(property.Name).GetValue(l).ToString()
                            .Contains(searchCriteria, StringComparison.OrdinalIgnoreCase));
                            if (searchedLocks.Any())
                            {

                                searchedLocks.ToList().ForEach(l =>
                                {
                                    // check if the data is already there
                                    var dataAlreadyinSearchedList = locks.FirstOrDefault(lck => lck.Id.Equals(l.Id));
                                    // increase the weight if data is there with less weight
                                    if (dataAlreadyinSearchedList != null)
                                    {
                                        if (dataAlreadyinSearchedList.Weight.Item2 < weight)
                                        {
                                            dataAlreadyinSearchedList.Weight = new Tuple<string, int>(property.Name, weight);
                                        }
                                    }
                                    else
                                    {
                                        // add to the search list
                                        l.Weight = new Tuple<string, int>(property.Name, weight);
                                        locks.Add(l);

                                    }
                                });
                            }
                        }
                    }
                    if (weightAttribute is TransitivePropertyWeightAttribute && searchedBuildings.Any())
                    {
                        var transitiveAttribute = ((TransitivePropertyWeightAttribute)weightAttribute);
                        foreach (var building in searchedBuildings.Where(b => transitiveAttribute.PropertyName.Contains(b.Weight.Item1)))
                        {
                            var searchedlocks = allData.Locks.Where(l => l.BuildingId == building.Id);
                            if (searchedlocks.Any())
                            {
                                searchedlocks.ToList().ForEach(l =>
                                {
                                    // check if the data is already there
                                    var dataAlreadyinSearchedList = locks.FirstOrDefault(lck => lck.Id.Equals(l.Id));
                                    // increase the weight if data is there with less weight
                                    if (dataAlreadyinSearchedList != null)
                                    {
                                        if (dataAlreadyinSearchedList.Weight.Item2 < transitiveAttribute.Weightage)
                                        {
                                            dataAlreadyinSearchedList.Weight = new Tuple<string, int>(property.Name, transitiveAttribute.Weightage);
                                        }
                                    }
                                    else
                                    {
                                        // add to the search list
                                        l.Weight = new Tuple<string, int>(property.Name, transitiveAttribute.Weightage);
                                        locks.Add(l);

                                    }
                                });
                            }
                        }
                        
                    }
                }
            }

            return locks.OrderBy(l => l.Weight.Item2).ToList();
        }

        private static List<Building> SearchBuildings(string searchCriteria, EntityModel allData)
        {
            var buildings = new List<Building>();
            System.Type type = typeof(Building);
            System.Reflection.PropertyInfo[] properties = type.GetProperties();

            foreach (System.Reflection.PropertyInfo property in properties)
            {
                var weightAttribute = property.GetCustomAttributes(typeof(OwnPropertyWeightAttribute), true).FirstOrDefault();
                if (weightAttribute != null)
                {
                    var weight = ((OwnPropertyWeightAttribute)weightAttribute).Weightage;
                    var searchedBuildings = allData.Buildings.Where(b => b.GetType().GetProperty(property!.Name).GetValue(b) != null &&
                     b.GetType().GetProperty(property.Name).GetValue(b).ToString().Contains(searchCriteria, StringComparison.OrdinalIgnoreCase));
                    if (searchedBuildings.Any())
                    {
                        searchedBuildings.ToList().ForEach(b =>
                        {
                            // check if the data is already there
                            var dataAlreadyinSearchedList = buildings.FirstOrDefault(building => building.Id.Equals(b.Id));
                            // increase the weight if data is there with less weight
                            if(dataAlreadyinSearchedList != null )
                            {
                                if( dataAlreadyinSearchedList.Weight.Item2 < weight)
                                {
                                    dataAlreadyinSearchedList.Weight = new Tuple<string, int>(property.Name, weight);
                                }
                            }
                            else 
                            { 
                                // add to the search list
                                b.Weight = new Tuple<string, int>(property.Name, weight);
                                buildings.Add(b);

                            }
                        });

                    }
                }

            }

            return buildings.OrderBy(b => b.Weight.Item2).ToList() ;
        }

       
    }
}
