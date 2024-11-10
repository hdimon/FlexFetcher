# FlexFetcher

FlexFetcher is a .NET library for filtering, sorting, paging data. 
It is designed based on OOP principles and can be used in any .NET project: Web, Desktop, Mobile, etc.
FlexFetcher is inspired by [Telerik Kendo UI](https://www.telerik.com/kendo-ui) grid objects format.

## Table of contents
- [Why FlexFetcher](#why-flexfetcher)
- [Supported platforms](#supported-platforms)
- [Supported data types](#supported-data-types)
- [Installation](#installation)
- [Basic usage](#basic-usage)
- [Advanced usage](#advanced-usage)
- [Dependency injection](#dependency-injection)
- [FlexFilter with filtering nested objects](#flexfilter-with-filtering-nested-objects)
- [FlexFilter logic](#flexfilter-logic)
- [FlexFilter operators](#flexfilter-operators)
- [Extending FlexFilter operators](#extending-flexfilter-operators)
- [Field mapping](#field-mapping)
- [Custom fields](#custom-fields)
- [Custom filter fields](#custom-filter-fields)
- [FlexFetcher Context](#flexfetcher-context)
- [Field hiding](#field-hiding)
- [Serialization and deserialization](#serialization-and-deserialization)
  - [Newtonsoft.Json](#newtonsoftjson)
  - [System.Text.Json](#systemtextjson)
  - [Succinct format](#succinct-format)
- [ASP.NET integration](#asp.net-integration)
  - [ASP.NET Core](#asp.net-core)
  - [ASP.NET Framework](#asp.net-framework)
- [Samples](#samples)

## Why FlexFetcher
There are a lot of great libraries for filtering, sorting, paging data in .NET: 
[Sieve](https://github.com/Biarity/Sieve), [QueryKit](https://github.com/pdevito3/QueryKit), [LightQuery](https://github.com/GeorgDangl/LightQuery), etc. 
But most of them are not designed based on OOP principles. 
They have query syntaxes which are intended to be human-readable, like (all examples are taken from those libraries documentation):
```
sorts=LikeCount,CommentCount,-created&filters=LikeCount>10,Title@=awesome title,&page=1&pageSize=10
```
or
```
""(Age ^^ [20, 30, 40]) && (BirthMonth ^^* ["January", "February", "March"]) || (Id ^^ ["6d623e92-d2cf-4496-a2df-f49fa77328ee"])""
```
or
```
`?sort=country&thenSort=email desc`
```
but at the same time:
- own syntaxes might be not flexible enough to cover all possible cases
- they are not easy to extend
- they are hard for machine-to-machine communication
- they are hard for humans to read and understand because it's required to learn a new syntax

FlexFetcher is not better or worse than those libraries, it's just different. 
It accepts queries in a format of objects, which can be serialized/deserialized to/from JSON like this:
```
?Filters={"Logic":"And","Filters":[{"Operator":"Eq","Field":"Address.Town","Value":"New York"}]}
```
or, if you prefer more compact format, even like this:
```
?Filter={"L":"And","Fs":[{"O":"Eq","F":"Address.Town","V":"New York"}]}
```
or, if you want to use POST request (RPC style), just like this:
```json
{
  "Filters": {
    "Logic": "and",
    "Filters": [
      {
        "Operator": "eq",
        "Field": "Address.Town",
        "Value": "New York"
      }
    ]
  }
}
```

This format is easy to read and understand for humans, easy to extend, easy to serialize/deserialize, and easy to use in machine-to-machine communication.

## Supported platforms
FlexFetcher is build for next platforms:
- .NET 6.0
- .NET 7.0
- .NET 8.0
- .NET Standard 2.0

It means that you can use it in both .NET Core (see ```samples/WebApiSample```) and .NET Framework (see ```samples/WebApiSample.Framework48```) projects.

## Supported data types
FlexFetcher supports next data types for fields which are used in filters and sorters:
- string
- int
- long
- double
- float
- decimal
- bool
- DateOnly (.NET 6.0 or greater)
- DateTime
- DateTimeOffset
- TimeSpan
- TimeOnly (.NET 6.0 or greater)
- Guid
- enum (both as string and as integer)

All values might be nullable.

## Installation
You can install FlexFetcher ([NuGet](https://www.nuget.org/packages/FlexFetcher)) via NuGet Package Manager Console by running next command:
```
dotnet add package FlexFetcher
```
To install FlexFetcher.DependencyInjection.Microsoft ([NuGet](https://www.nuget.org/packages/FlexFetcher.DependencyInjection.Microsoft)):
```
dotnet add package FlexFetcher.DependencyInjection.Microsoft
```
To install FlexFetcher.Serialization.NewtonsoftJson ([NuGet](https://www.nuget.org/packages/FlexFetcher.Serialization.NewtonsoftJson)):
```
dotnet add package FlexFetcher.Serialization.NewtonsoftJson
```
To install FlexFetcher.Serialization.SystemTextJson ([NuGet](https://www.nuget.org/packages/FlexFetcher.Serialization.SystemTextJson)):
```
dotnet add package FlexFetcher.Serialization.SystemTextJson
```

## Basic usage

> All examples are done on test dataset, which is defined in ```tests/TestData/InMemoryDataHelper.cs``` 
and consists of ```tests/TestData/Database/PeopleEntity.cs``` and ```tests/TestData/Database/AddressEntity.cs``` entities.

The simplest way to utilize FlexFetcher is to use extension methods for ```IQueryable<T>``` and ```IEnumerable<T>``` interfaces.

### Filter
```csharp
var filter = new DataFilter
{
    Filters = new List<DataFilter>
    {
        new()
        {
            Field = "Name",
            Operator = DataFilterOperator.Equal,
            Value = "John"
        }
    }
};
var result = _ctx.People.FilterData(filter).ToList();
```
### Sorter
```csharp
var sorter = new DataSorters
{
    Sorters = new List<DataSorter>
    {
        new DataSorter
        {
            Field = "Surname",
            Direction = DataSorterDirection.Asc
        }
    }
};
var result = _ctx.People.SortData(sorter).ToList();
```
### Pager
```csharp
var pager = new DataPager { PageSize = 3, Page = 1 }; // Numeration starts from 1 for Page number
OR
var pager = new DataPager { Skip = 3, Take = 3 };

var result = _ctx.People.PageData(pager).ToList();
```

## Advanced usage
Extensions methods are good for simple cases, but their usage is very limited.

For more complex cases you can use ```FlexFetcher``` classes: 
```FlexFilter<TEntity>``` and ```FlexFilter<TEntity, TModel>```, 
```FlexSorter<TEntity>``` and ```FlexSorter<TEntity, TModel>```,
```FlexPager<TEntity>``` and ```FlexPager<TEntity, TModel>```,
```FlexFetcher<TEntity>``` and ```FlexFetcher<TEntity, TModel>```.

Even though it's not recommended way, but for clarity, let's see how to create instances of classes manually.
We will look at dependency injection in corresponding section later.

### Filter
```csharp
var filter = new DataFilter
{
    Filters = new List<DataFilter>
    {
        new()
        {
            Field = "Name",
            Operator = DataFilterOperator.Equal,
            Value = "John"
        }
    }
};
var flexFilter = new FlexFilter<PeopleEntity>();
var result = flexFilter.FilterData(_ctx.People, filter).ToList();
```

### Sorter
```csharp
var sorters = new DataSorters
{
    Sorters = new List<DataSorter>
    {
        new DataSorter
        {
            Field = "Surname",
            Direction = DataSorterDirection.Asc
        }
    }
};
var flexSorter = new FlexSorter<PeopleEntity>();
var result = flexSorter.SortData(_ctx.People, sorters).ToList();
```

### Pager
```csharp
var pager = new DataPager { PageSize = 3, Page = 1 };
var flexPager = new FlexPager<PeopleEntity>();
var result = flexPager.PageData(_ctx.People, pager).ToList();
```

### Fetcher
```csharp
var pager = new DataPager { PageSize = 3, Page = 1 };
var flexFetcher = new FlexFetcher<PeopleEntity>();
var result = flexFetcher.FetchData(_ctx.People, null, null, pager).ToList(); // Filter, sorter and pager parameters are optional
```

## Dependency injection
In order to use FlexFetcher with dependency injection, don't forget to install ```FlexFetcher.DependencyInjection.Microsoft``` package.

It's possible to inject main FlexFetcher class as well as its components: FlexFilter, FlexSorter, FlexPager.
The basic usage is:
```csharp
services.AddSingleton<FlexFetcher<PeopleEntity>>();
```
> FlexFetcher classses don't have any state, so it's safe to use them as singletons.
> But if you want to use them as scoped or transient, it's up to you.

More advanced scenarious fill be shown next. 
Also see more examples in ```tests/FlexFetcherTests/DependencyInjectionTests/ServiceProviderTests.cs```, 
```samples/WebApiSample```) and ```samples/WebApiSample.Framework48```.

## FlexFilter with filtering nested objects
In our example data model we have nested objects: PeopleEntity has Address property.
Let's see how to filter by nested object:
```csharp
var filter = new DataFilter
{
    Filters = new List<DataFilter>
    {
        new()
        {
            Field = "Address.City", // Address is a nested object, so we use dot notation
            Operator = DataFilterOperator.Equal,
            Value = "New York"
        }
    }
};
var flexAddressFilter = new FlexFilter<AddressEntity>();
var peopleFilterOptions = new FlexFilterOptions<PeopleEntity>();
peopleFilterOptions.AddNestedFlexFilter(flexAddressFilter);
var flexPeopleFilter = new FlexFilter<PeopleEntity>(peopleFilterOptions);
var result = flexPeopleFilter.FilterData(_ctx.People, filter).ToList();
```
The same thing can be done with dependency injection:
```csharp
Services.AddSingleton<FlexFilter<AddressEntity>>();
Services.AddSingletonFlexOptions<FlexFilterOptions<PeopleEntity>>((provider, options) =>
{
    options.AddNestedFlexFilter(provider.GetRequiredService<FlexFilter<AddressEntity>>());
});
Services.AddSingleton<FlexFilter<PeopleEntity>>();
```
The same principle can be applied to FlexSorter, FlexPager and FlexFetcher.

See more examples in ```tests/FlexFetcherTests/DependencyInjectionTests/ServiceProviderTests.cs``` and ```samples/WebApiSample```.

## FlexFilter logic
In previous examples we used only one filter, but it's possible to use multiple filters with OR or AND logics like this:
```csharp
var filter = new DataFilter
{
    Logic = DataFilterLogic.Or,
    Filters = new List<DataFilter>
    {
        new()
        {
            Field = "Name",
            Operator = DataFilterOperator.Equal,
            Value = "John"
        },
        new()
        {
            Logic = DataFilterLogic.And,
            Filters = new List<DataFilter>
            {
                new()
                {
                    Field = "Name",
                    Operator = DataFilterOperator.Equal,
                    Value = "Jane"
                },
                new()
                {
                    Field = "Age",
                    Operator = DataFilterOperator.GreaterThan,
                    Value = 55
                }
            }
        }
    }
};
```
FlexFilter supports nested filters with any depth.

> If Logic is not specified, it will be set to AND by default.

> Filter can contain either (Filters and Logic) or (Field, Operator and Value) properties, but not both.

## FlexFilter operators
We have already seen examples with Equal, GreaterThan operators, but there are more operators available:
- Equal
```csharp
new DataFilter { Field = "Name", Operator = DataFilterOperator.Equal, Value = "John" }
```
- NotEqual
```csharp
new DataFilter { Field = "Name", Operator = DataFilterOperator.NotEqual, Value = "John" }
```
- GreaterThan
```csharp
new DataFilter { Field = "Age", Operator = DataFilterOperator.GreaterThan, Value = 55 }
```
- GreaterThanOrEqual
```csharp
new DataFilter { Field = "Age", Operator = DataFilterOperator.GreaterThanOrEqual, Value = 55 }
```
- LessThan
```csharp
new DataFilter { Field = "Age", Operator = DataFilterOperator.LessThan, Value = 55 }
```
- LessThanOrEqual
```csharp
new DataFilter { Field = "Age", Operator = DataFilterOperator.LessThanOrEqual, Value = 55 }
```
- Contains
```csharp
new DataFilter { Field = "Name", Operator = DataFilterOperator.Contains, Value = "Jo" }
```
- StartsWith
```csharp
new DataFilter { Field = "Name", Operator = DataFilterOperator.StartsWith, Value = "Jo" }
```
- EndsWith
```csharp
new DataFilter { Field = "Name", Operator = DataFilterOperator.EndsWith, Value = "hn" }
```
- In
```csharp
new DataFilter { Field = "Age", Operator = DataFilterOperator.In, Value = new List<int> { 55, 56, 57 } }
```

## Extending FlexFilter operators
In order to create own filter operator it's needed: 
- create custom FilterExpressionHandler inherrited from ```BaseFilterExpressionHandler```
- create custom FilterExpressionBuilder inherrited from ```FilterExpressionBuilder```
- override ```AddCustomExpressionHandlers``` method in custom FilterExpressionBuilder class
- pass custom FilterExpressionBuilder to FlexFilterOptions constructor

It might look like this (see ```tests/FlexFetcherTests/FlexFilterTests/CustomFilterExpressionBuilderTests.cs```):
```csharp
class CustomExpressionBuilderWithValueTest : FilterExpressionBuilder<PeopleEntity>
{
    protected override void AddCustomExpressionHandlers(List<IFilterExpressionHandler> handlers)
    {
        handlers.Add(new ModuleFilterExpressionHandler());
    }

    private class ModuleFilterExpressionHandler : BaseFilterExpressionHandler
    {
        public override string Operator => "MODULE";

        public override Expression BuildExpression(Expression property, DataFilter filter)
        {
            var value = BuildValueExpression(filter);
            return Expression.Equal(Expression.Modulo(property, value), Expression.Constant(0));
        }
    }
}

var customExpressionBuilder = new CustomExpressionBuilderWithValueTest();
var options = new FlexFilterOptions<PeopleEntity>(customExpressionBuilder);
var flexFilter = new FlexFilter<PeopleEntity>(options);

var filter = new DataFilter
{
    Logic = DataFilterLogic.And,
    Filters = new List<DataFilter>
    {
        new()
        {
            Field = "Age",
            Operator = "Module",
            Value = 15
        }
    }
};

var result = flexFilter.FilterData(_ctx.People, filter).ToList();
```
Also it's possible to create custom filter operator which does need Value at all:
```csharp
class CustomExpressionBuilderWithoutValueTest : FilterExpressionBuilder<PeopleEntity>
{
    protected override void AddCustomExpressionHandlers(List<IFilterExpressionHandler> handlers)
    {
        handlers.Add(new EvenNumberFilterExpressionHandler());
    }

    private class EvenNumberFilterExpressionHandler : BaseFilterExpressionHandler
    {
        public override string Operator => "EVEN";

        public override Expression BuildExpression(Expression property, DataFilter filter)
        {
            return Expression.Equal(Expression.Modulo(property, Expression.Constant(2, property.Type)), Expression.Constant(0));
        }
    }
}

var customExpressionBuilder = new CustomExpressionBuilderWithoutValueTest();
var options = new FlexFilterOptions<PeopleEntity>(customExpressionBuilder);
var flexFilter = new FlexFilter<PeopleEntity>(options);

var filter = new DataFilter
{
    Logic = DataFilterLogic.And,
    Filters = new List<DataFilter>
    {
        new()
        {
            Field = "Age",
            Operator = "Even",
            Value = null
        }
    }
};

var result = flexFilter.FilterData(_ctx.People, filter).ToList();
```

Usage with dependency injection might look like this 
(see ```tests/FlexFetcherTests/DependencyInjectionTests/ServiceProviderTests.cs```, ```GenericFlexFilterWithExpressionBuilderUsage()``` method):
```csharp
class CustomExpressionBuilder : FilterExpressionBuilder<PeopleEntity>;

class GenericFlexFilterService(FlexFilter<PeopleEntity> flexFilter)
{
    public FlexFilter<PeopleEntity> FlexFilter { get; } = flexFilter;
}

var serviceCollection = new ServiceCollection();
serviceCollection.AddSingleton<FilterExpressionBuilder<PeopleEntity>, CustomExpressionBuilder>();
serviceCollection.AddSingleton<FlexFilterOptions<PeopleEntity>>();
serviceCollection.AddSingleton<FlexFilter<PeopleEntity>>();
serviceCollection.AddTransient<GenericFlexFilterService>();

var serviceProvider = serviceCollection.BuildServiceProvider();
var testInstance = serviceProvider.GetRequiredService<GenericFlexFilterService>();
```
Here ```CustomExpressionBuilder``` is a custom filter expression builder, which is passed to FlexFilterOptions constructor.
Then FlexFilterOptions is passed to FlexFilter constructor.
Finally, FlexFilter is injected into ```GenericFlexFilterService``` class.

It might seem a bit complicated, but it's quite powerful. FlexFetcher is very flexible.

## Field mapping
In previous examples we used field names as they are in entities, but it's possible to use custom field names.
There are two ways to do it:
- define field alias as string
- define field alias as property of Model class

The simplest way is to define field alias as string:
```csharp
// Manual creation of FlexFilter
var addressFilterOptions = new FlexFilterOptions<AddressEntity>();
addressFilterOptions.Field(entity => entity.City).Map("Town");
FlexFilter<AddressEntity> addressFilter = new FlexFilter<AddressEntity>(addressFilterOptions);

// Dependency injection of FlexFetcher
Services.AddSingletonFlexOptions<FlexFetcherOptions<AddressEntity>>(options =>
{
    options.Field(entity => entity.City).Map("Town");
});
Services.AddSingleton<FlexFetcher<AddressEntity>>();
```
In this example we map AddressEntity.City property to "Town" field, 
i.e. we can use "Town" in filters and sorters instead of "City".

The same thing can be done with property of Model class:
```csharp
public class AddressModel
{
    public string Town { get; set; }
}

// Manual creation of FlexFilter
var addressFilterOptions = new FlexFilterOptions<AddressEntity, AddressModel>();
addressFilterOptions.Field(entity => entity.City).Map(model => model.Town);
FlexFilter<AddressEntity, AddressModel> addressFilter = new FlexFilter<AddressEntity, AddressModel>(addressFilterOptions);

// Dependency injection of FlexFetcher
Services.AddSingletonFlexOptions<FlexFetcherOptions<AddressEntity, AddressModel>>(options =>
{
    options.Field(entity => entity.City).Map(model => model.Town);
});
Services.AddSingleton<FlexFetcher<AddressEntity, AddressModel>>();
```
In this example we map AddressEntity.City property to AddressModel.Town property.
In general it gives possibility to map view models to entities and have different mappings for different view models.

> Multiple mappings are allowed, so it's possible to map one entity field to multiple aliases, for example:
> ```csharp
> options.Field(entity => entity.City).Map("Town").Map("CityName");
> ```

> It's allowed to map to view model properties and to string values at the same time:
> ```csharp
> options.Field(x => x.City).Map(model => model.Town).Map("CityName");
> ```

> Mapping to custom fields (see below) is also allowed:
> ```csharp
> var customField = new PeopleFullNameCustomField();
> var options = new FlexSorterOptions<PeopleEntity>();
> options.AddCustomField(customField).Map("Title");
> var flexSorter = new FlexSorter<PeopleEntity>(options);
> ```

## Custom fields
In previous examples we used only properties of entities, but it's possible to use custom fields with custom expressions.

They might be used in FlexSorters, FlexFilters, FlexFetchers.

Let's say we want to add custom field "FullName" to PeopleEntity:
```csharp
public class PeopleFullNameCustomField : BaseFlexCustomField<PeopleEntity, string> // string is a type of field
{
    public override string Field => "FullName";

    protected override Expression<Func<PeopleEntity, string>> BuildFieldExpression(IFlexFetcherContext? context = null)
    {
        return p => p.Surname + " " + p.Name;
    }
}

// Manual creation of FlexSorter
var customField = new PeopleFullNameCustomField();
var options = new FlexSorterOptions<PeopleEntity>();
options.AddCustomField(customField).Map("Title");
var flexSorter = new FlexSorter<PeopleEntity>(options);

// Dependency injection of FlexFetcher
Services.AddSingletonFlexOptions<FlexFetcherOptions<PeopleEntity>>(options =>
{
	options.AddCustomField(new PeopleFullNameCustomField()).Map("Title"); // Map is optional
});
Services.AddSingleton<FlexFetcher<PeopleEntity>>();
```

## Custom filter fields
Probably in the most cases custom fields which we have seen in previous section are enough, but sometimes it's needed to have custom fields with custom expressions in filters for more complex filter logic.

Let's say we want to add custom field "PeopleGroups" to PeopleEntity and apply "AnyGroup" filter to it:
```csharp
public class PeopleWithManyToManyGroupsCustomFilter : BaseFlexCustomFieldFilter<PeopleEntity>
{
    public override string Field => "PeopleGroups";

    protected override Expression<Func<PeopleEntity, bool>> BuildFilterExpression(string filterOperator, object? filterValue,
        IFlexFetcherContext? context = null)
    {
        string value = (string)filterValue!;

        return filterOperator switch
        {
            "AnyGroup" => p => p.PeopleGroups.Any(pg => pg.Group!.Name == value),
            _ => throw new NotSupportedException($"Invalid filter operator: {filterOperator}")
        };
    }
}

// Another way to create extended FlexFilter - custom class inherrited from FlexFilter
public class PeopleWithManyToManyGroups : FlexFilter<PeopleEntity>
{
    public PeopleWithManyToManyGroups(PeopleWithManyToManyGroupsCustomFilter customFilter)
    {
        Options.AddCustomField(customFilter);
    }
}

var customFilter = new PeopleWithManyToManyGroupsCustomFilter();
var peopleFilter = new PeopleWithManyToManyGroups(customFilter);
```

## FlexFetcher Context
FlexFetcher context is a special object which is passed to custom fields and custom filters.

It can contain any data which is needed for custom fields and custom filters.

Let's say we need to pick different entity fields depending on culture:
```csharp
public class CustomContext : IFlexFetcherContext
{
    public CultureInfo Culture { get; set; } = null!;
}

public class PeopleOriginCountryCustomField : BaseFlexCustomField<PeopleEntity, string?>
{
    public override string Field => "Country";

    protected override Expression<Func<PeopleEntity, string?>> BuildFieldExpression(IFlexFetcherContext? context = null)
    {
        if (context is not CustomContext customContext)
        {
            throw new NotSupportedException("Invalid context type");
        }

        if (customContext.Culture.Name == "de-DE")
        {
            return entity => entity.OriginCountryDe;
        }

        return entity => entity.OriginCountryEn;
    }
}

var customExpressionFilter = new PeopleOriginCountryCustomField();
var options = new FlexFilterOptions<PeopleEntity>();
options.AddCustomField(customExpressionFilter);
var flexFilter = new FlexFilter<PeopleEntity>(options);

var filter = new DataFilter
{
    Filters = new List<DataFilter>
    {
        new()
        {
            Field = "Country",
            Operator = DataFilterOperator.Equal,
            Value = "Deutschland"
        }
    }
};

var context = new CustomContext
{
    Culture = new CultureInfo("de-DE")
};

var result = flexFilter.FilterData(_ctx.People, filter, context).ToList();
```

## Field hiding
In previous sections we have seen how to work with fields and how to map them. By default all fields are visible and can be used in filters and sorters.
But in some cases it's needed to hide some fields from being used in filters and sorters.

There are two ways to hide fields:
- define field as hidden
- define all entity fields as hidden that might be useful when mapping to view models

The way to define field as hidden:
```csharp
var options = new FlexFilterOptions<PeopleEntity>();
options.Field(x => x.CreatedByUserId).Hide(); // CreatedByUserId is a hidden field, it can't be used in filters and sorters
var flexFilter = new FlexFilter<PeopleEntity>(options);
```

The way to define all entity fields as hidden:
```csharp
var options = new FlexFilterOptions<PeopleEntity>();
options.HideOriginalFields();
var flexFilter = new FlexFilter<PeopleEntity>(options);
```

> Field aliases are not affected by hiding, so they can be used in filters and sorters even if original fields are hidden.

Custom fields are also might be hidden:
```csharp
class SimplePeopleSorterWithCustomSorter : FlexSorter<PeopleEntity>
{
    public SimplePeopleSorterWithCustomSorter()
    {
        Options.AddCustomField(new PeopleFullNameCustomField()).Map("Title").Hide();
    }
}
```

> Custom fields are not hidden automatically when HideOriginalFields() is used so they should be hidden manually if needed.

## Serialization and deserialization
FlexFetcher is designed to be used in machine-to-machine communication, so it's important to be able to serialize and deserialize queries.

In the simple cases any JSON serializer can be used but as soos as you need to use fields with TimeOnly type or you need to serialize/deserialize arrays to use ```In``` operator, you need to apply settings and custom JSON converters from ```FlexFetcher.Serialization.NewtonsoftJson``` or ```FlexFetcher.Serialization.SystemTextJson``` packages.

### Newtonsoft.Json
Install ```FlexFetcher.Serialization.NewtonsoftJson``` package and get settings from helper class:
```csharp
var jsonSettings = NewtonsoftHelper.GetSerializerSettings();
var json = JsonConvert.SerializeObject(filter, jsonSettings);
var deserializedFilter = JsonConvert.DeserializeObject<DataFilter>(json, jsonSettings);
```

### System.Text.Json
Install ```FlexFetcher.Serialization.SystemTextJson``` package and get settings from helper class:
```csharp
var jsonSettings = SystemTextJsonHelper.GetSerializerOptions();
var json = JsonSerializer.Serialize(filter, jsonSettings);
var deserializedFilter = JsonSerializer.Deserialize<DataFilter>(json, jsonSettings);
```

### Succinct format
There are also JSON converters which allow to serialize/deserialize objects to/from JSON in succinct format like this (DataFilter example):
```json
{
  "L": "And",
  "Fs": [
    {
      "O": "Eq",
      "F": "Address.Town",
      "V": "New York"
    }
  ]
}
```

See more examples of succinct format in ```tests/FlexFetcherTests/SerializationTests/NewtonsoftTests.cs``` and ```tests/FlexFetcherTests/SerializationTests/SystemTextJsonTests.cs``` 
and also in sample projects ```samples/WebApiSample``` and ```samples/WebApiSample.Framework48```.

To support succinct format it's needed to use next converters from ```FlexFetcher.Serialization.NewtonsoftJson``` and ```FlexFetcher.Serialization.SystemTextJson``` accordingly:
- ```FlexFetcherDataSorterConverter```
- ```FlexFetcherDataSortersConverter```
- ```FlexFetcherDataPagerConverter```
- ```FlexFetcherDataFilterConverter```
- ```FlexFetcherDataQueryConverter```

All converters have ```readOnlyShortForm``` constructor parameter. If it's set to true, then only succinct format will be allowed for deserialization.
Otherwise both full and succinct formats will be allowed.

In general there is no need to use ```readOnlyShortForm``` in most cases, but if you want to restrict deserialization to succinct format only, you can set it to true.

## ASP.NET integration
FlexFetcher can be used in ASP.NET projects, both Core and Framework, but Serializers configuration is different.

### ASP.NET Core
Next steps are needed to configure serialization in ASP.NET Core projects (example for System.Text.Json):
- create ```FlexFetcherModelBinder```
- create ```FlexFetcherModelBinderProvider```
- setup JSON serialization settings in ```AddControllers``` method in ```Program.cs``` like this:
```csharp
builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new FlexFetcherModelBinderProvider());
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new GenericConverter());
    // Next converters are optional, but they are needed to support succinct format
    options.JsonSerializerOptions.Converters.Add(new FlexFetcherDataSorterConverter());
    options.JsonSerializerOptions.Converters.Add(new FlexFetcherDataSortersConverter());
    options.JsonSerializerOptions.Converters.Add(new FlexFetcherDataPagerConverter());
    options.JsonSerializerOptions.Converters.Add(new FlexFetcherDataFilterConverter());
    options.JsonSerializerOptions.Converters.Add(new FlexFetcherDataQueryConverter());
});
```
See code in ```samples/WebApiSample```.

### ASP.NET Framework
Next steps are needed to configure serialization in ASP.NET Framework projects (example for Newtonsoft.Json):
- create ```FlexFetcherModelBinder```
- create ```FlexFetcherModelBinderProvider```
- setup JSON serialization settings in ```WebApiConfig.cs``` like this:
```csharp
var jsonSettings = NewtonsoftHelper.GetSerializerSettings();
config.Formatters.JsonFormatter.SerializerSettings = jsonSettings;
// Next converters are optional, but they are needed to support succinct format
config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new FlexFetcherDataFilterConverter());
config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new FlexFetcherDataSortersConverter());
config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new FlexFetcherDataSorterConverter());
config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new FlexFetcherDataPagerConverter());
config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new FlexFetcherDataQueryConverter());

config.Services.Insert(typeof(ModelBinderProvider), 0, new FlexFetcherModelBinderProvider());
```
See code in ```samples/WebApiSample.Framework48```.

## Samples
There are two sample projects in the repository:
- ```samples/WebApiSample``` - ASP.NET Core Web API project
- ```samples/WebApiSample.Framework48``` - ASP.NET Framework 4.8 Web API project

Both projects have the same functionality and demonstrate how to use FlexFetcher in ASP.NET projects.

Just build and run them to see how FlexFetcher works in real projects. Use your favorite REST client to send requests to the API.
Examples of requests can be found in ```samples/WebApiSample/ReadMe.md``` and ```samples/WebApiSample.Framework48/ReadMe.md```.

Also there are tests in ```tests/FlexFetcherTests``` folder which demonstrate how to use FlexFetcher in different scenarios.