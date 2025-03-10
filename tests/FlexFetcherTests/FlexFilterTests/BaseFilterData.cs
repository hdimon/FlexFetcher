﻿using FlexFetcher;
using FlexFetcher.Exceptions;
using FlexFetcher.Models.FlexFetcherOptions;
using FlexFetcher.Models.Queries;
using FlexFetcher.Serialization.NewtonsoftJson;
using FlexFetcher.Serialization.SystemTextJson;
using FlexFetcherTests.Stubs.CustomFields;
using FlexFetcherTests.Stubs.CustomFilters;
using FlexFetcherTests.Stubs.FlexFetcherContexts;
using FlexFetcherTests.Stubs.FlexFilters;
using Newtonsoft.Json;
using System.Globalization;
using TestData.Database;

namespace FlexFetcherTests.FlexFilterTests;

public abstract class BaseFilterData
{
    protected void SimpleFilterTest(Func<DataFilter, List<PeopleEntity>> fetcher)
    {
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

        var result = fetcher(filter);

        Assert.That(result.Count, Is.EqualTo(5));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 1, 3, 5, 7, 9 }));
    }

    protected void SimpleContainsFilterTest(Func<DataFilter, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new()
                {
                    Field = "Name",
                    Operator = DataFilterOperator.Contains,
                    Value = "an"
                }
            }
        };

        var result = fetcher(filter);

        Assert.That(result.Count, Is.EqualTo(5));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 2, 4, 6, 8, 10 }));
    }

    protected void SimpleStartsWithFilterTest(Func<DataFilter, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new()
                {
                    Field = "Name",
                    Operator = DataFilterOperator.StartsWith,
                    Value = "Ja"
                }
            }
        };

        var result = fetcher(filter);

        Assert.That(result.Count, Is.EqualTo(5));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 2, 4, 6, 8, 10 }));
    }

    protected void SimpleEndsWithFilterTest(Func<DataFilter, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new()
                {
                    Field = "Name",
                    Operator = DataFilterOperator.EndsWith,
                    Value = "ne"
                }
            }
        };

        var result = fetcher(filter);

        Assert.That(result.Count, Is.EqualTo(5));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 2, 4, 6, 8, 10 }));
    }

    protected void SimpleInArrayFilterTest(Func<DataFilter, List<PeopleEntity>> fetcher)
    {
        var ids = new List<int> { 1, 3, 5, 7 };
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new()
                {
                    Field = "Id",
                    Operator = DataFilterOperator.In,
                    Value = ids
                }
            }
        };

        var filterJson = JsonConvert.SerializeObject(filter, NewtonsoftHelper.GetSerializerSettings());
        var filter1 = JsonConvert.DeserializeObject<DataFilter>(filterJson, NewtonsoftHelper.GetSerializerSettings())!;

        var result = fetcher(filter1);

        Assert.That(result.Count, Is.EqualTo(4));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(ids));

        filterJson = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter2 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(filterJson, SystemTextJsonHelper.GetSerializerSettings())!;

        result = fetcher(filter2);

        Assert.That(result.Count, Is.EqualTo(4));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(ids));
    }

    protected void SimpleFilterWithCustomFilterTest(Func<DataFilter, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new()
                {
                    Field = "FullName",
                    Operator = DataFilterOperator.Equal,
                    Value = "John Smith"
                }
            }
        };

        var result = fetcher(filter);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 3 }));
    }

    protected void SimpleFilterWithCustomFilterWithContextTest(Func<DataFilter, List<PeopleEntity>> fetcher)
    {
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

        var result = fetcher(filter);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 4 }));
    }

    protected void SimpleFilterWithNestedCustomFilterTest(Func<FlexFilter<PeopleEntity>, DataFilter, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilter
        {
            Logic = DataFilterLogic.And,
            Filters = new List<DataFilter>
            {
                new()
                {
                    Field = "Address",
                    Operator = DataFilterOperator.NotEqual,
                    Value = null
                },
                new()
                {
                    Field = "Address.Location",
                    Operator = DataFilterOperator.Equal,
                    Value = "Chicago, IL"
                }
            }
        };

        var customFilter = new AddressLocationCustomFilter();
        var addressFilter = new SimpleAddressFilterWithNestedCustomFilter(customFilter);
        var options = new FlexFilterOptions<PeopleEntity>();
        options.AddNestedFlexFilter(addressFilter);
        var flexFilter = new FlexFilter<PeopleEntity>(options);

        var result = fetcher(flexFilter, filter);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 3 }));
    }

    protected void SimpleFilterWithNestedWithCustomFilterWithContextTest(
        Func<FlexFilter<PeopleEntity>, DataFilter, CustomContext, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilter
        {
            Logic = DataFilterLogic.And,
            Filters = new List<DataFilter>
            {
                new()
                {
                    Field = "Address",
                    Operator = DataFilterOperator.NotEqual,
                    Value = null
                },
                new()
                {
                    Field = "Address.Country",
                    Operator = DataFilterOperator.Equal,
                    Value = "Deutschland"
                }
            }
        };

        var customFilter = new AddressCountryCustomField();
        var addressFilterOptions = new FlexFilterOptions<AddressEntity>();
        addressFilterOptions.AddCustomField(customFilter);
        var addressFilter = new FlexFilter<AddressEntity>(addressFilterOptions);

        var options = new FlexFilterOptions<PeopleEntity>();
        options.AddNestedFlexFilter(addressFilter);
        var flexFilter = new FlexFilter<PeopleEntity>(options);

        var context = new CustomContext
        {
            Culture = new CultureInfo("de-DE")
        };

        var result = fetcher(flexFilter, filter, context);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 4 }));
    }

    protected void SimpleFilterWithFieldAliasTest(Func<DataFilter, FlexFilterOptions<PeopleEntity>, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new()
                {
                    Field = "FirstName",
                    Operator = DataFilterOperator.Equal,
                    Value = "John"
                }
            }
        };

        var options = new FlexFilterOptions<PeopleEntity>();
        options.Field(x => x.Name).Map("FirstName");

        var result = fetcher(filter, options);

        Assert.That(result.Count, Is.EqualTo(5));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 1, 3, 5, 7, 9 }));
    }

    protected void SimpleValueObjectFilterWithFieldAliasTest(Func<DataFilter, FlexFilterOptions<PeopleEntity>, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new()
                {
                    Field = "FirstName",
                    Operator = DataFilterOperator.Equal,
                    Value = "John"
                }
            }
        };

        var options = new FlexFilterOptions<PeopleEntity>();
        options.Field(x => x.PeopleName).CastTo<string>().Map("FirstName");

        var result = fetcher(filter, options);

        Assert.That(result.Count, Is.EqualTo(5));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 1, 3, 5, 7, 9 }));
    }

    protected void SimpleValueObjectFilterWithTheSameFieldAliasTest(Func<DataFilter, FlexFilterOptions<PeopleEntity>, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new()
                {
                    Field = "PeopleName",
                    Operator = DataFilterOperator.Equal,
                    Value = "John"
                }
            }
        };

        var options = new FlexFilterOptions<PeopleEntity>();
        options.Field(x => x.PeopleName).CastTo<string>().Map("PeopleName");

        var result = fetcher(filter, options);

        Assert.That(result.Count, Is.EqualTo(5));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 1, 3, 5, 7, 9 }));
    }

    protected void SimpleNestedEntityFilterWithFieldAliasTest(Func<FlexFilter<PeopleEntity>, DataFilter, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilter
        {
            Logic = DataFilterLogic.And,
            Filters = new List<DataFilter>
            {
                new()
                {
                    Field = "Residence",
                    Operator = DataFilterOperator.NotEqual,
                    Value = null
                },
                new()
                {
                    Field = "Residence.Town",
                    Operator = DataFilterOperator.Equal,
                    Value = "New York"
                }
            }
        };

        var addressOption = new FlexFilterOptions<AddressEntity>();
        addressOption.Field(x => x.City).Map("Town");
        var addressFilter = new FlexFilter<AddressEntity>(addressOption);
        var peopleOption = new FlexFilterOptions<PeopleEntity>();
        peopleOption.AddNestedFlexFilter(addressFilter);
        peopleOption.Field(x => x.Address).Map("Residence");
        var peopleFilter = new FlexFilter<PeopleEntity>(peopleOption);

        var result = fetcher(peopleFilter, filter);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 1 }));
    }

    protected void SimpleNestedEntityFilterWithTheSameFieldAliasTest(Func<FlexFilter<PeopleEntity>, DataFilter, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilter
        {
            Logic = DataFilterLogic.And,
            Filters = new List<DataFilter>
            {
                new()
                {
                    Field = "Address",
                    Operator = DataFilterOperator.NotEqual,
                    Value = null
                },
                new()
                {
                    Field = "Address.City",
                    Operator = DataFilterOperator.Equal,
                    Value = "New York"
                }
            }
        };

        var addressOption = new FlexFilterOptions<AddressEntity>();
        addressOption.Field(x => x.City).Map("City");
        var addressFilter = new FlexFilter<AddressEntity>(addressOption);
        var peopleOption = new FlexFilterOptions<PeopleEntity>();
        peopleOption.AddNestedFlexFilter(addressFilter);
        peopleOption.Field(x => x.Address).Map("Address");
        var peopleFilter = new FlexFilter<PeopleEntity>(peopleOption);

        var result = fetcher(peopleFilter, filter);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 1 }));
    }

    protected void SimpleNestedEntityFilterWithFieldAliasByFlexFilterTest(Func<SimpleNestedEntityPeopleFilterWithFieldAlias, DataFilter, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilter
        {
            Logic = DataFilterLogic.And,
            Filters = new List<DataFilter>
            {
                new()
                {
                    Field = "Residence",
                    Operator = DataFilterOperator.NotEqual,
                    Value = null
                },
                new()
                {
                    Field = "Residence.Town",
                    Operator = DataFilterOperator.Equal,
                    Value = "New York"
                }
            }
        };

        var peopleFilter = new SimpleNestedEntityPeopleFilterWithFieldAlias(new SimpleNestedEntityAddressFilterWithFieldAlias());
        var result = fetcher(peopleFilter, filter);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 1 }));
    }

    protected void SimpleFilterWithDefaultAndLogicTest(Func<DataFilter, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilter
        {
            //Logic = DataFilterLogic.And, // Default logic is And
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
                    Field = "Age",
                    Operator = DataFilterOperator.GreaterThanOrEqual,
                    Value = 45
                }
            }
        };

        var result = fetcher(filter);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 7, 9 }));
    }

    protected void SimpleFilterWithAndLogicTest(Func<DataFilter, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilter
        {
            Logic = DataFilterLogic.And,
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
                    Field = "Age",
                    Operator = DataFilterOperator.GreaterThanOrEqual,
                    Value = 45
                }
            }
        };

        var result = fetcher(filter);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 7, 9 }));
    }

    protected void SimpleFilterWithOrLogicTest(Func<DataFilter, List<PeopleEntity>> fetcher)
    {
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
                    Field = "Age",
                    Operator = DataFilterOperator.LessThan,
                    Value = 30
                }
            }
        };

        var result = fetcher(filter);

        Assert.That(result.Count, Is.EqualTo(6));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 1, 2, 3, 5, 7, 9 }));
    }

    protected void FilterWithOrLogicAndNestedAndFiltersTest(Func<DataFilter, List<PeopleEntity>> fetcher)
    {
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

        var result = fetcher(filter);

        Assert.That(result.Count, Is.EqualTo(6));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 1, 3, 5, 7, 9, 10 }));
    }

    protected void FilterWithAndLogicAndNestedOrFiltersTest(Func<DataFilter, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilter
        {
            Logic = DataFilterLogic.And,
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
                    Logic = DataFilterLogic.Or,
                    Filters = new List<DataFilter>
                    {
                        new()
                        {
                            Field = "Age",
                            Operator = DataFilterOperator.LessThanOrEqual,
                            Value = 25
                        },
                        new()
                        {
                            Field = "Age",
                            Operator = DataFilterOperator.GreaterThanOrEqual,
                            Value = 55
                        }
                    }
                }
            }
        };

        var result = fetcher(filter);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 1, 9 }));
    }

    protected void FilterWithNestedEntitiesOfTheSameTypeTest(Func<FlexFilter<PeopleEntity>, DataFilter, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilter
        {
            Logic = DataFilterLogic.And,
            Filters = new List<DataFilter>
            {
                new()
                {
                    Field = "Creator",
                    Operator = DataFilterOperator.NotEqual,
                    Value = null
                },
                new()
                {
                    Field = "Creator.CreatorFullName",
                    Operator = DataFilterOperator.Equal,
                    Value = "John Doe"
                },
                new()
                {
                    Field = "Updater",
                    Operator = DataFilterOperator.NotEqual,
                    Value = null
                },
                new()
                {
                    Field = "Updater.UpdaterFullName",
                    Operator = DataFilterOperator.Equal,
                    Value = "Jane Doe"
                }
            }
        };

        var userFullNameCustomFilter = new UserFullNameCustomFilter();
        var userFilter1 = new UserFilterWithNestedEntitiesOfTheSameType1(userFullNameCustomFilter);
        var peopleFilter1 = new PeopleFilterWithNestedEntitiesOfTheSameType1(userFilter1);

        var result1 = fetcher(peopleFilter1, filter);

        Assert.That(result1.Count, Is.EqualTo(1));
        Assert.That(result1.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 3 }));

        var userFilter2 = new UserFilterWithNestedEntitiesOfTheSameType2(userFullNameCustomFilter);
        var peopleFilter2 = new PeopleFilterWithNestedEntitiesOfTheSameType2(userFilter2);

        var result2 = fetcher(peopleFilter2, filter);

        Assert.That(result2.Count, Is.EqualTo(1));
        Assert.That(result2.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 3 }));
    }

    protected void FilterWithNestedManyToManyEntitiesTest(Func<FlexFilter<PeopleEntity>, DataFilter, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilter
        {
            Logic = DataFilterLogic.And,
            Filters = new List<DataFilter>
            {
                new()
                {
                    Field = "PeopleGroups",
                    Operator = "AnyGroup",
                    Value = "Group 1"
                }
            }
        };

        var customFilter = new PeopleWithManyToManyGroupsCustomFilter();
        var peopleFilter = new PeopleWithManyToManyGroups(customFilter);

        var result = fetcher(peopleFilter, filter);

        Assert.That(result.Count, Is.EqualTo(4));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 1, 3, 5, 6 }));
    }

    protected void SimpleFilterWithHiddenFieldTest(Func<DataFilter, List<PeopleEntity>> filter)
    {
        var filters = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "CreatedByUserId",
                    Operator = DataFilterOperator.Equal,
                    Value = 1
                }
            }
        };

        Assert.Throws<FieldNotFoundException>(() =>
        {
            var _ = filter(filters);
        });
    }

    protected void SimpleFilterWithHiddenFieldAndTheSameMappingTest(Func<DataFilter, List<PeopleEntity>> filter)
    {
        var filters = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "CreatedByUserId",
                    Operator = DataFilterOperator.Equal,
                    Value = 1
                }
            }
        };

        var result = filter(filters);

        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 1, 2, 3 }));
    }

    protected void SimpleValueObjectFilterWithHiddenFieldAndTheSameMappingTest(Func<DataFilter, List<PeopleEntity>> filter)
    {
        var filters = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "PeopleCreatedByUserId",
                    Operator = DataFilterOperator.Equal,
                    Value = 1
                }
            }
        };

        var result = filter(filters);

        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 1, 2, 3 }));
    }

    protected void SimpleFilterWithNotFoundFieldTest(Func<DataFilter, List<PeopleEntity>> filter)
    {
        var filters = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Field",
                    Operator = DataFilterOperator.Equal,
                    Value = 1
                }
            }
        };

        Assert.Throws<FieldNotFoundException>(() =>
        {
            var _ = filter(filters);
        });
    }
}