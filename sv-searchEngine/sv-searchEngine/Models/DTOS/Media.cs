using sv_searchEngine.Models.Attribute;

namespace sv_searchEngine.Models.DTOS
{
    public class Media
    {
        public Guid Id { get; set; }
        [TransitivePropertyWeightAttribute(typeof(Group), "GroupName", 8)]
        public Guid GroupId { get; set; }
        
        [OwnPropertyWeightAttribute(3)]
        public string Type { get; set; }
        [OwnPropertyWeightAttribute(10)]
        public string Owner { get; set; }
        [OwnPropertyWeightAttribute(8)]
        public object Description { get; set; }
        [OwnPropertyWeightAttribute(6)]
        public string SerialNumber { get; set; }
        public Weightage Weight { get; set; }

    }
}
