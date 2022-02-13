# sv-searchEngine


This search engine will search all the entities with search criteria based on the assignment https://simonsvoss-homework.herokuapp.com/full-stack.html.

The application is implemented in the .net MVC framework.

The data is displayed in the Entity view. On the load of the entity page, all the data is displayed. When the search text is passed, it is supposed to return the entities matching the search text as per relevance. The search text is passed to the "EntityController", which calls the "SearchService", which again calls the generic class "SearchForType", with implements the following logic for searching with own properties as well as searching for transitive properties. It then returns all the search results to the "SearchService" that orders the searched data as per relevance weight. The search logic is implemented according to the following algorithm.

# Search with own searchable fields
1. All the entities have an extra field than the data fields called "weight". This field holds the name and value of a searchable property that has maximum weight for an entity. 
2. All the entity properties that are own searchable fields will have a custom attribute that will define the weight of the property. 
3. When we pass a search text, the algorithm will check all the entities' properties one by one and return the result in a search list. 
4. Full-text search: When it finds the custom attribute on a property, it will first match the search text with the property's data using the ".Equals()" function. As the full-text search has 10X relevance, if this condition is successful, the algorithm will take the entity and update the weight field with the property name and the value as 10 * weight of that property.
5. Then, it will check the search list is already having the entity or not. 
If it finds the entity in the search list, it will compare the weight value of the existing entity with the newly calculated weight value in step 4. If the new weight is more, it will update the existing entity in the search list or leave it as is.
If it doesn't find the entity in the search list, it will add the entity and its weight.
6. Partial-text search: If the full-text search is not successful, then the algo will implement partial-text search, which is the same as steps 4 and 5 except 
it will use the ".Contains()" function instead of ".Equals()".
And add the property weight as it is without 10 *.
7. At the end, the search list will be returned to the "SearchService". 

# Search with transitive searchable fields
1. All the entity properties that are transitive searchable fields will have custom attributes that will define the parent object, the property of the parent object on which it depends and the weight of the property. 
2. The search function will receive the entity data, the existing search result and the parent objects.
3. It will then check the entity properties for the transitive custom attribute. If a property has the attribute, it will then look for the parent objects whose "Id" matches the property data. 

In our example, for "Lock" entities, "BuildingId" is the transitive property that has two custom transitive attributes
     	2.1. "Building" is the parent object,  "BuildingName" (parent object name + dependent parent property name forms the dependent property name "Building" + "Name") as the property of the parent object on which it depends and weight as 8.
	2.2. "Building" is the parent object,  "BuidingShortCut" as the property of the parent object on which it depends and weight as 5.

So, in our example, the algorithm will check the parent object list, i.e. , searched building object list, that has been passed as parameter for the match where the building id of the lock will match the id of a building. 
4. It will then look for the name of the weight of that building whose Id matches. And as per the name, it will assign its weight. So , if the algorithm finds a building whose Id matches with the building Id of the lock, it will check that building object's weight. 
	4.1. In our example, if the building object's weight name is "Name", the algorithm will assign the "BuildingId" and 8 as the weight of the lock.
	4.2. In our example, if the building object's weight name is "ShortCut", the algorithm will assign the "BuildingId" and 5 as the weight of the lock.  
5. Then, it will check the search list is already having the entity or not. 
If it finds the entity in the search list, it will compare the weight value of the existing entity with the newly calculated weight value in step 4. If the new weight is more, it will update the existing entity in the search list or leave it as is.
If it doesn't find the entity in the search list, it will add the entity and its weight.
6. At the end, the search list will be returned to the "SearchService". 

The "SearchService" will return shorted (ordered by weight) search results to the controller to display.
