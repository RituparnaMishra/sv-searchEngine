namespace sv_searchEngine.Models.Attribute
{
    [System.AttributeUsage(System.AttributeTargets.Property ,
                       AllowMultiple = true)  // multiuse attribute  
    ]
    public class TransitivePropertyWeightAttribute : System.Attribute
    {
        public Type ParentObject { get; set; }
        public string PropertyName { get; set; }
        public int Weightage { get; set; }

        public TransitivePropertyWeightAttribute(Type parentObject, string propertyName, int weightage)
        {
            ParentObject = parentObject;
            PropertyName = propertyName;
            Weightage = weightage;

        }
    }
}
