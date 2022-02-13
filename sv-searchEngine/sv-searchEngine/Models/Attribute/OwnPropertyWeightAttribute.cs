namespace sv_searchEngine.Models.Attribute
{
    public class OwnPropertyWeightAttribute : System.Attribute
    {
        public int Weightage { get; set; }
        public OwnPropertyWeightAttribute(int weightage)
        {
            Weightage = weightage;

        }
    }
}
