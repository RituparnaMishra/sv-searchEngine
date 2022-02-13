using sv_searchEngine.Models.Attribute;
using System;

namespace sv_searchEngine.Models.DTOS
{
    public class Building
    {
        public Guid Id { get; set; }
        [OwnPropertyWeightAttribute(7)]
        public string ShortCut { get; set; }
        [OwnPropertyWeightAttribute(9)]
        public string Name { get; set; } = null!;
        [OwnPropertyWeightAttribute(5)]
        public string Description { get; set; }
        public Weightage Weight { get; set; }

    }
}
