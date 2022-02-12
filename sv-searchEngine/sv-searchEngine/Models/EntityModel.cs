using sv_searchEngine.Models.DTOS;


namespace sv_searchEngine.Models
{
    public class EntityModel
    {
        //public DataCollection Entity { get; set; }

        public IEnumerable<Building> Buildings { get; set; } = new List<Building>();
        public IEnumerable<Lock> Locks { get; set; } = new List<Lock>();
        public IEnumerable<Group> Groups { get; set; } = new List<Group>();
        public IEnumerable<Media> Media { get; set; } = new List<Media>();


    }
}
