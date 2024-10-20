﻿using FlexFetcher;
using FlexFetcher.Exceptions;
using FlexFetcher.Models.FlexFetcherOptions;
using FlexFetcher.Models.Queries;
using FlexFetcher.Serialization.NewtonsoftJson;
using FlexFetcher.Serialization.SystemTextJson;
using FlexFetcherTests.Stubs.CustomFilters;
using FlexFetcherTests.Stubs.FlexFilters;
using Newtonsoft.Json;
using TestData.Database;

namespace FlexFetcherTests.FlexFilterTests;

public abstract class BaseFilterData
{
    protected void SimpleFilterTest(Func<DataFilters, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilters
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

    protected void SimpleContainsFilterTest(Func<DataFilters, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilters
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

    protected void SimpleStartsWithFilterTest(Func<DataFilters, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilters
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

    protected void SimpleEndsWithFilterTest(Func<DataFilters, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilters
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

    protected void SimpleInArrayFilterTest(Func<DataFilters, List<PeopleEntity>> fetcher)
    {
        var ids = new List<int> { 1, 3, 5, 7 };
        var filter = new DataFilters
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
        var filter1 = JsonConvert.DeserializeObject<DataFilters>(filterJson, NewtonsoftHelper.GetSerializerSettings())!;

        var result = fetcher(filter1);

        Assert.That(result.Count, Is.EqualTo(4));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(ids));

        filterJson = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter2 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(filterJson, SystemTextJsonHelper.GetSerializerSettings())!;

        result = fetcher(filter2);

        Assert.That(result.Count, Is.EqualTo(4));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(ids));
    }

    protected void SimpleFilterWithCustomFilterTest(Func<DataFilters, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilters
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

    protected void SimpleFilterWithNestedCustomFilterTest(Func<FlexFilter<PeopleEntity>, DataFilters, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilters
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

    protected void SimpleFilterWithFieldAliasTest(Func<DataFilters, FlexFilterOptions<PeopleEntity>, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilters
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

    protected void SimpleNestedEntityFilterWithFieldAliasTest(Func<FlexFilter<PeopleEntity>, DataFilters, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilters
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

    protected void SimpleNestedEntityFilterWithFieldAliasByFlexFilterTest(Func<SimpleNestedEntityPeopleFilterWithFieldAlias, DataFilters, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilters
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

    protected void SimpleFilterWithAndLogicTest(Func<DataFilters, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilters
        {
            Logic = DataFilterLogic.And, //TODO: add validation
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

    protected void SimpleFilterWithOrLogicTest(Func<DataFilters, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilters
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

    protected void FilterWithOrLogicAndNestedAndFiltersTest(Func<DataFilters, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilters
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

    protected void FilterWithAndLogicAndNestedOrFiltersTest(Func<DataFilters, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilters
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

    protected void FilterWithNestedEntitiesOfTheSameTypeTest(Func<FlexFilter<PeopleEntity>, DataFilters, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilters
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

    protected void FilterWithNestedManyToManyEntitiesTest(Func<FlexFilter<PeopleEntity>, DataFilters, List<PeopleEntity>> fetcher)
    {
        var filter = new DataFilters
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

    protected void SimpleFilterWithHiddenFieldTest(Func<DataFilters, List<PeopleEntity>> filter)
    {
        var filters = new DataFilters
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

    protected void SimpleFilterWithNotFoundFieldTest(Func<DataFilters, List<PeopleEntity>> filter)
    {
        var filters = new DataFilters
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