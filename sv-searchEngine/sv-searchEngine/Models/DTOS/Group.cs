using sv_searchEngine.Models.Attribute;

namespace sv_searchEngine.Models.DTOS
{
    public class Group
    {
        public Guid Id { get; set; }
        [OwnPropertyWeightAttribute(9)]
        public string Name { get; set; }
        [OwnPropertyWeightAttribute(5)]
        public string Description { get; set; }
        public Weightage Weight { get; set; }

    }
}
