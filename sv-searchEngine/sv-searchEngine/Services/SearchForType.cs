using sv_searchEngine.Models.Attribute;
using sv_searchEngine.Models.DTOS;

namespace sv_searchEngine.Services
{
    public class SearchForType<T> where T : class
    {
        // search for own properties
        public List<T> SearchForOwnProperties(IEnumerable<T> data, string searchCriteria)
        {
            List<T> returnData = new List<T>();
            if (data != null && data.Count() > 0)
            {
                var properties = typeof(T).GetProperties();
                foreach (var property in properties)
                {
                    var attributes = property.GetCustomAttributes(true);
                    if (attributes != null && attributes.Count() > 0)
                    {
                        foreach (var attribute in attributes.Where(a => a is OwnPropertyWeightAttribute))
                        {
                            var ownattribute = (OwnPropertyWeightAttribute)attribute;
                            var weightage = new Weightage()
                            {
                                Name = property.Name,
                                Value = ownattribute.Weightage
                            };
                            IEnumerable<T> searchedDataList;
                            // full text saerch
                            searchedDataList = data.Where(d => d!.GetType()!.GetProperty(property.Name)!.GetValue(d) != null &&
                                           d!.GetType()!.GetProperty(property!.Name)!.GetValue(d)!.ToString()!.Equals(searchCriteria, StringComparison.OrdinalIgnoreCase)); ;

                            if (searchedDataList.Any())
                            {
                                // 10 X more weitage for full test search
                                ownattribute.Weightage *= 10;
                            }
                            else
                            {
                                // partial search
                                searchedDataList = data.Where(d => d!.GetType()!.GetProperty(property.Name)!.GetValue(d) != null &&
                                d!.GetType()!.GetProperty(property.Name)!.GetValue(d)!.ToString()!.Contains(searchCriteria, StringComparison.OrdinalIgnoreCase)); ;
                            }



                            if (searchedDataList.Any())
                            {

                                searchedDataList.ToList().ForEach(searchedData =>
                                {
                                    // check if the data has a id field and if yes get the value

                                    if (searchedData!.GetType()!.GetProperty("Id")!.GetValue(searchedData) != null)
                                    {
                                        var idValue = searchedData.GetType()!.GetProperty("Id")!.GetValue(searchedData);
                                        T? existingData = null;

                                        if (returnData.Any())
                                        {
                                            // check if the data is already there

                                            existingData = returnData.FirstOrDefault(
                                                d => d!.GetType()!.GetProperty("Id")!.GetValue(d) != null
                                                            && d!.GetType()!.GetProperty("Id")!.GetValue(d)!.Equals(idValue))!;
                                        }
                                        if (existingData == null)
                                        {
                                            // add to the search list
                                            searchedData!.GetType()!.GetProperty("Weight")!.SetValue(searchedData, weightage);
                                            returnData.Add(searchedData);

                                        }
                                        // increase the weight if data is there with less weight
                                        else
                                        {
                                            if (existingData.GetType().GetProperty("Weight") != null
                                                 && existingData!.GetType()!.GetProperty("Weight")!.GetValue(existingData) != null)
                                            {
                                                var existingWeightage = (Weightage)existingData!.GetType()!.GetProperty("Weight")!.GetValue(existingData)!;
                                                if (existingWeightage.Value < weightage.Value)
                                                {
                                                    existingData!.GetType()!.GetProperty("Weight")!.SetValue(existingData, weightage);
                                                }
                                            }


                                        }

                                    }

                                });
                            }

                        }
                    }
                }
            }

            return returnData;
        }

        // search for transitive properties

        public List<T> SearchForTransitiveProperties(IEnumerable<T> data, IEnumerable<T> existingSearchResults, IEnumerable<Object> parentObjects)
        {
            List<T> returnData = new List<T>();
            if (existingSearchResults.Any())
            {
                returnData.AddRange(existingSearchResults.ToList());
            }

            var parentObjectsWithRelevantData = parentObjects.Where(
                po => po!.GetType()!.GetProperty("Id")!.GetValue(po) != null
                && po!.GetType()!.GetProperty("Weight")!.GetValue(po) != null);
            // check if parent object has Id field and weight field
            if (data != null && data.Count() > 0
                && parentObjectsWithRelevantData.Any())
            {
                var properties = typeof(T).GetProperties();
                foreach (var property in properties)
                {
                    var attributes = property.GetCustomAttributes(true);
                    if (attributes != null && attributes.Count(a => a is TransitivePropertyWeightAttribute) > 0)
                    {
                        foreach (var attribute in attributes.Where(a => a is TransitivePropertyWeightAttribute))
                        {
                            var transitiveAttribute = (TransitivePropertyWeightAttribute)attribute;
                            var weightage = new Weightage()
                            {
                                Name = property.Name,
                                Value = transitiveAttribute.Weightage

                            };


                            var dataListThatHasValueInTheTransitiveProperty = data.
                                Where(d => d.GetType()!.GetProperty(property.Name)!.GetValue(d) != null).ToList();

                            dataListThatHasValueInTheTransitiveProperty.ForEach(
                                searchedData =>
                                {
                                    var valueToCheck = searchedData.GetType()!.GetProperty(property.Name)!.GetValue(searchedData);
                                    if (valueToCheck != null)
                                    {
                                        // the transitive property attribute Parentobject is parent type and  transitive property attribute PropertyName = parenttype name + property name... 
                                        // e.g. for building name in locks the transitive property attribute ParentObject  will be building and transitive property attribute PropertyName = BuildingName
                                        if (parentObjectsWithRelevantData.Any(po =>
                                             po!.GetType()!.GetProperty("Id")!.GetValue(po)!.Equals(valueToCheck)
                                             && po!.GetType()!.GetProperty("Weight")!.GetValue(po) != null
                                             && po.GetType().Name.Equals(transitiveAttribute.ParentObject.Name)
                                             && (po.GetType().Name + ((Weightage)po!.GetType()!.GetProperty("Weight")!.GetValue(po)!).Name).Equals(transitiveAttribute.PropertyName)))
                                         {

                                            // Check for existing data
                                            T? existingData = null;
                                            if (returnData.Any(o => o.GetType().GetProperty("Id")!.GetValue(o) != null)
                                            && searchedData.GetType()!.GetProperty("Id")!.GetValue(searchedData) != null)
                                            {
                                                existingData = returnData.FirstOrDefault(o => o.GetType()!.GetProperty("Id")!.GetValue(o)!.Equals(
                                                    searchedData.GetType()!.GetProperty("Id")!.GetValue(searchedData)));
                                                
                                            }
                                            if (existingData == null)
                                            {
                                                searchedData!.GetType()!.GetProperty("Weight")!.SetValue(searchedData, weightage);
                                                returnData.Add(searchedData);

                                            }
                                            else
                                            {
                                                var existingWeightage = (Weightage)existingData!.GetType()!.GetProperty("Weight")!.GetValue(existingData)!;
                                                if (existingWeightage.Value < weightage.Value)
                                                {
                                                    existingData!.GetType()!.GetProperty("Weight")!.SetValue(existingData, weightage);
                                                }


                                            }

                                        }
                                    }
                                }

                                );
                        }
                    }
                }
            }

            return returnData;
        }

    }
}
