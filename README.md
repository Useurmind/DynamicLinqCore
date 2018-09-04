# DynamicLinqCore

Create linq queries dynamically via strings and non generic IQueryable interface.

Integration for entity framework methods like Include.

Early beta.

## Installation

	nuget install DynamicLinqCore 
	nuget install DynamicLinqCore.EntityFramework
	
## Get Started

This project should provide a way to create entity framework core queries from strings.

It should solve the most common issues you have when querying the database for showing data in tables:

- Filtering with Where
- Loading relations with Include
- Loading a certain range of entities with Skip and Take
- Providing database ordering via OrderBy, ThenBy, etc.

Examples:

	# Filter by length of name and skip first and take one
	# include deep relation (works the same as in entity framework)
	# order by id ascending
	# uses strongly typed dbsets and returns stronly type collection
	dbContext.TestEntity1s.Where("x => x.Name.Length > 1")
						  .Include("ChildEntity.ChildChildEntities")
						  .OrderBy("x => x.Id")
                          .Skip(1)
                          .Take(1)
                          .ToArrayAsync();

    # Same query as above
	# uses just IQueryable interface
	# returns an object[]	
	var queryable = (IQueryable)dbContext.TestEntity1s;
	queryable.Where("x => x.Name.Length > 1")
			 .Include("ChildEntity.ChildChildEntities")
			 .OrderBy("x => x.Id")
             .Skip(1)
             .Take(1)
             .ToArrayAsync();