using sv_searchEngine.Models.Attribute;

namespace sv_searchEngine.Models.DTOS
{
    public class Lock
    {
        public string Id { get; set; }
        public Guid BuildingId { get; set; }
        [OwnPropertyWeightAttribute(3)]
        public string Type { get; set; }
        [OwnPropertyWeightAttribute(10)]
        public string Name { get; set; }
        [OwnPropertyWeightAttribute(8)]
        public string Description { get; set; }
        [OwnPropertyWeightAttribute(6)]
        public string SerialNumber { get; set; }
        [OwnPropertyWeightAttribute(6)]
        public string Floor { get; set; }
        [OwnPropertyWeightAttribute(6)]
        public string RoomNumber { get; set; }

        public int Weight { get; set; }
    }
}
