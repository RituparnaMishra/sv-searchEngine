using sv_searchEngine.Models;
using sv_searchEngine.Models.Attribute;
using sv_searchEngine.Models.DTOS;

namespace sv_searchEngine.Services
{
    public interface ISearchService
    {
        public EntityModel? GetSearchedResults(string searchCriteria, EntityModel allData);
    }

    public class SearchService : ISearchService
    {

        private readonly EntityModel _data=new EntityModel();

        public EntityModel? GetSearchedResults(string searchCriteria, EntityModel allData)
        {
            var searchedBuildings = SearchBuildings(searchCriteria,allData);
            var searchedLocks = SearchLocks(searchCriteria,allData, searchedBuildings);
            var searchedGroups = SearchGroups(searchCriteria, allData);
            var searchMedia = SearchMedia(searchCriteria, allData, searchedGroups);

            _data.Buildings = searchedBuildings;
            _data.Locks = searchedLocks;
            _data.Groups = searchedGroups;
            _data.Media = searchMedia;

            if (_data.Buildings.Any() || _data.Locks.Any() || _data.Groups.Any() || _data.Media.Any())
            {
                return _data;
            }
            else
            {
                return null;
            }
        }

        private static List<Media> SearchMedia(string searchCriteria, EntityModel allData, IEnumerable<Group> searchedGroups)
        {
            var media = new List<Media>();
            var searchType = new SearchForType<Media>();
            media = searchType.SearchForOwnProperties(allData.Media, searchCriteria).ToList();
            media = searchType.SearchForTransitiveProperties(allData.Media, media, searchedGroups);

            if (media.Any())
            {
                _ = media.OrderBy(b => b.Weight.Value).ToList();
            }


            return media;

        }

        private static List<Group> SearchGroups(string searchCriteria, EntityModel allData)
        {
            var groups = new List<Group>();
            var searchType = new SearchForType<Group>();
            groups = searchType.SearchForOwnProperties(allData.Groups, searchCriteria).ToList();
            if (groups.Any())
            {
                _ = groups.OrderBy(b => b.Weight.Value).ToList();
            }
            return groups;
        }

        private static List<Lock> SearchLocks(string searchCriteria, EntityModel allData, IEnumerable<Building> searchedBuildings)
        {
            var locks = new List<Lock>();
            var searchType = new SearchForType<Lock>();
            locks = searchType.SearchForOwnProperties(allData.Locks, searchCriteria).ToList();
            locks = searchType.SearchForTransitiveProperties(allData.Locks, locks, searchedBuildings);

            if (locks.Any())
            {
                _ = locks.OrderBy(b => b.Weight.Value).ToList();
            }


            return locks;
        }

        private static List<Building> SearchBuildings(string searchCriteria, EntityModel allData)
        {
            var buildings = new List<Building>();
            var searchType = new SearchForType<Building>();
            buildings = searchType.SearchForOwnProperties(allData.Buildings,searchCriteria).ToList();
            if (buildings.Any())
            {
                _ = buildings.OrderBy(b => b.Weight.Value).ToList();
            }


            return buildings ;
        }

       
    }
}
