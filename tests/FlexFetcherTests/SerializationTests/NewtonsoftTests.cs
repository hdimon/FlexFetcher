using FlexFetcher.Models.Queries;
using FlexFetcher.Serialization.NewtonsoftJson;
using FlexFetcher.Serialization.NewtonsoftJson.Converters;
using Newtonsoft.Json;
using TestData.Database;

namespace FlexFetcherTests.SerializationTests;

public class NewtonsoftTests
{
    [Test]
    public void SerializeDatesTest()
    {
        var settings = NewtonsoftHelper.GetSerializerSettings();

        var dt1LocalKind = new DateTime(2024, 6, 10, 10, 20, 56, DateTimeKind.Local);
        var dt2UtcKind = new DateTime(2024, 6, 10, 10, 20, 56, DateTimeKind.Utc);
        var dt3UnspecifiedKind = new DateTime(2024, 6, 10, 10, 20, 56, DateTimeKind.Unspecified);
        var dt1UtcFromLocalKind = dt1LocalKind.ToUniversalTime();
        var dt2UtcFromUtcKind = dt2UtcKind.ToUniversalTime();
        var dt3UtcFromUnspecifiedKind = dt3UnspecifiedKind.ToUniversalTime();
        var dtJson1UtcFromLocalKind = JsonConvert.SerializeObject(dt1UtcFromLocalKind);
        var dtJson2UtcFromUtcKind = JsonConvert.SerializeObject(dt2UtcFromUtcKind);
        var dtJson3UtcFromUnspecifiedKind = JsonConvert.SerializeObject(dt3UtcFromUnspecifiedKind);
        var dto1KindOffset = TimeZoneInfo.Local.GetUtcOffset(new DateTime(2024, 6, 10, 10, 20, 56));
        var dto1KindOffsetString = dto1KindOffset >= TimeSpan.Zero ? $"+{dto1KindOffset:hh\\:mm}" : $"{dto1KindOffset:hh\\:mm}";
        var dto1 = new DateTimeOffset(new DateTime(2024, 6, 10, 10, 20, 56), dto1KindOffset);
        var dto2 = new DateTimeOffset(new DateTime(2024, 6, 10, 10, 20, 56), TimeSpan.FromHours(0));

        Assert.That(dt1LocalKind, Is.EqualTo(dt2UtcKind));
        Assert.That(dt1LocalKind, Is.EqualTo(dt3UnspecifiedKind));
        Assert.That(dt2UtcKind, Is.EqualTo(dt3UnspecifiedKind));

        // Serialize with default settings
        var dt1LocalKindOffset = TimeZoneInfo.Local.GetUtcOffset(dt1LocalKind);
        var dt1LocalKindOffsetString = dt1LocalKindOffset >= TimeSpan.Zero
            ? $"+{dt1LocalKindOffset:hh\\:mm}"
            : $"{dt1LocalKindOffset:hh\\:mm}";
        var dtJson1LocalKind = JsonConvert.SerializeObject(dt1LocalKind);
        Assert.That(dtJson1LocalKind, Is.EqualTo($"\"2024-06-10T10:20:56{dt1LocalKindOffsetString}\""));
        var dtJson2UtcKind = JsonConvert.SerializeObject(dt2UtcKind);
        Assert.That(dtJson2UtcKind, Is.EqualTo("\"2024-06-10T10:20:56Z\""));
        var dtJson3UnspecifiedKind = JsonConvert.SerializeObject(dt3UnspecifiedKind);
        Assert.That(dtJson3UnspecifiedKind, Is.EqualTo("\"2024-06-10T10:20:56\""));
        var dto1Json = JsonConvert.SerializeObject(dto1);
        Assert.That(dto1Json, Is.EqualTo($"\"2024-06-10T10:20:56{dto1KindOffsetString}\""));
        var dto2Json = JsonConvert.SerializeObject(dto2);
        Assert.That(dto2Json, Is.EqualTo("\"2024-06-10T10:20:56+00:00\""));

        // Serialize with custom settings
        //var dtJson1CustomKindOffset = TimeZoneInfo.Local.GetUtcOffset(dt1LocalKind);
        var dtJson1Custom = JsonConvert.SerializeObject(dt1LocalKind, settings);
        Assert.That(dtJson1Custom, Is.EqualTo($"\"2024-06-10T10:20:56{dt1LocalKindOffsetString}\""));
        var dtJson2Custom = JsonConvert.SerializeObject(dt2UtcKind, settings);
        Assert.That(dtJson2Custom, Is.EqualTo("\"2024-06-10T10:20:56Z\""));
        var dtJson3Custom = JsonConvert.SerializeObject(dt3UnspecifiedKind, settings);
        Assert.That(dtJson3Custom, Is.EqualTo("\"2024-06-10T10:20:56\""));
        var dto1JsonCustom = JsonConvert.SerializeObject(dto1, settings);
        Assert.That(dto1JsonCustom, Is.EqualTo($"\"2024-06-10T10:20:56{dto1KindOffsetString}\""));
        var dto2JsonCustom = JsonConvert.SerializeObject(dto2, settings);
        Assert.That(dto2JsonCustom, Is.EqualTo("\"2024-06-10T10:20:56+00:00\""));

        // Make sure that JSONs are the same
        Assert.That(dtJson1LocalKind, Is.EqualTo(dtJson1Custom));
        Assert.That(dtJson2UtcKind, Is.EqualTo(dtJson2Custom));
        Assert.That(dtJson3UnspecifiedKind, Is.EqualTo(dtJson3Custom));
        Assert.That(dto1Json, Is.EqualTo(dto1JsonCustom));
        Assert.That(dto2Json, Is.EqualTo(dto2JsonCustom));

        var dt1Deserialized = JsonConvert.DeserializeObject<DateTime>(dtJson1LocalKind, settings);
        Assert.That(dt1Deserialized, Is.EqualTo(dt1LocalKind));
        var dt2Deserialized = JsonConvert.DeserializeObject<DateTime>(dtJson2UtcKind, settings);
        Assert.That(dt2Deserialized, Is.EqualTo(dt2UtcKind));
        var dt3Deserialized = JsonConvert.DeserializeObject<DateTime>(dtJson3UnspecifiedKind, settings);
        Assert.That(dt3Deserialized, Is.EqualTo(dt3UnspecifiedKind));
        var dt1UtcDeserialized = JsonConvert.DeserializeObject<DateTime>(dtJson1UtcFromLocalKind, settings);
        var dt1UtcDeserializedExpected =
            new DateTime(2024, 6, 10, 10, 20, 56, DateTimeKind.Local).AddHours(-dto1KindOffset.TotalHours);
        Assert.That(dt1UtcDeserialized, Is.EqualTo(dt1UtcDeserializedExpected));
        var dt2UtcDeserialized = JsonConvert.DeserializeObject<DateTime>(dtJson2UtcFromUtcKind, settings);
        Assert.That(dt2UtcDeserialized, Is.EqualTo(dt2UtcKind));
        var dt3UtcDeserialized = JsonConvert.DeserializeObject<DateTime>(dtJson3UtcFromUnspecifiedKind, settings);
        Assert.That(dt3UtcDeserialized, Is.EqualTo(dt3UtcFromUnspecifiedKind));
        var dto1Deserialized = JsonConvert.DeserializeObject<DateTimeOffset>(dto1Json, settings);
        Assert.That(dto1Deserialized, Is.EqualTo(dto1));
        var dto2Deserialized = JsonConvert.DeserializeObject<DateTimeOffset>(dto2Json, settings);
        Assert.That(dto2Deserialized, Is.EqualTo(dto2));

        var dt1DeserializedObject = JsonConvert.DeserializeObject<object>(dtJson1LocalKind, settings);
        Assert.That(dt1DeserializedObject, Is.EqualTo(new DateTimeOffset(dt1LocalKind)));
        var dt2DeserializedObject = JsonConvert.DeserializeObject<object>(dtJson2UtcKind, settings);
        Assert.That(dt2DeserializedObject, Is.EqualTo(new DateTimeOffset(dt2UtcKind)));
        var dt3DeserializedObject = JsonConvert.DeserializeObject<object>(dtJson3UnspecifiedKind, settings);
        Assert.That(dt3DeserializedObject, Is.EqualTo(new DateTimeOffset(dt3UnspecifiedKind)));
        var dt1UtcDeserializedObject = JsonConvert.DeserializeObject<object>(dtJson1UtcFromLocalKind, settings);
        Assert.That(dt1UtcDeserializedObject, Is.EqualTo(new DateTimeOffset(dt1UtcFromLocalKind)));
        var dt2UtcDeserializedObject = JsonConvert.DeserializeObject<object>(dtJson2UtcFromUtcKind, settings);
        Assert.That(dt2UtcDeserializedObject, Is.EqualTo(new DateTimeOffset(dt2UtcFromUtcKind)));
        var dt3UtcDeserializedObject = JsonConvert.DeserializeObject<object>(dtJson3UtcFromUnspecifiedKind, settings);
        Assert.That(dt3UtcDeserializedObject, Is.EqualTo(new DateTimeOffset(dt3UtcFromUnspecifiedKind)));
        var dto1DeserializedObject = JsonConvert.DeserializeObject<object>(dto1Json, settings);
        Assert.That(dto1DeserializedObject, Is.EqualTo(dto1));
        var dto2DeserializedObject = JsonConvert.DeserializeObject<object>(dto2Json, settings);
        Assert.That(dto2DeserializedObject, Is.EqualTo(dto2));
    }

    [Test]
    public void SerializeArrayTest()
    {
        var settings = NewtonsoftHelper.GetSerializerSettings();

        var intArray = new[] { 1, 2, 3, 4, 5 };
        var json = JsonConvert.SerializeObject(intArray, settings);
        var arrayDeserialized = JsonConvert.DeserializeObject<object>(json, settings);
        Assert.That(arrayDeserialized, Is.EqualTo(intArray));

        var decimalArray = new[] { 1.1m, 2.2m, 3.3m, 4.4m, 5.5m };
        var json2 = JsonConvert.SerializeObject(decimalArray, settings);
        var arrayDeserialized2 = JsonConvert.DeserializeObject<object>(json2, settings);
        Assert.That(arrayDeserialized2, Is.EqualTo(decimalArray));

        var stringArray = new[] { "a", "b", "c", "d", "e" };
        var json3 = JsonConvert.SerializeObject(stringArray, settings);
        var arrayDeserialized3 = JsonConvert.DeserializeObject<object>(json3, settings);
        Assert.That(arrayDeserialized3, Is.EqualTo(stringArray));

        var doubleArray = new[] { 1.1, 2.2, 3.3, 4.4, 5.5 };
        var json4 = JsonConvert.SerializeObject(doubleArray, settings);
        var arrayDeserialized4 = JsonConvert.DeserializeObject<object>(json4, settings);
        Assert.That(arrayDeserialized4, Is.EqualTo(doubleArray));

        var doubleNullableArray = new double?[] { 1.1, 2.2, null, 3.3, 4.4, 5.5, null };
        var json5 = JsonConvert.SerializeObject(doubleNullableArray, settings);
        var arrayDeserialized5 = JsonConvert.DeserializeObject<object>(json5, settings);
        Assert.That(arrayDeserialized5, Is.EqualTo(doubleNullableArray));

        var dateOnlyArray = new[] { new DateOnly(2024, 6, 10), new DateOnly(2024, 6, 11), new DateOnly(2024, 6, 12) };
        var json6 = JsonConvert.SerializeObject(dateOnlyArray, settings);
        Assert.That(json6, Is.EqualTo("[\"2024-06-10\",\"2024-06-11\",\"2024-06-12\"]"));
        var arrayDeserialized6 = JsonConvert.DeserializeObject<object>(json6, settings);
        // It must be deserialized to array of strings
        Assert.That(arrayDeserialized6, Is.EqualTo(new[] { "2024-06-10", "2024-06-11", "2024-06-12" }));

        var dateTimeUtcArray = new[]
        {
            new DateTime(2024, 6, 10, 10, 20, 56, DateTimeKind.Utc), new DateTime(2024, 6, 11, 10, 20, 56, DateTimeKind.Utc),
            new DateTime(2024, 6, 12, 10, 20, 56, DateTimeKind.Utc)
        };
        var expectedDateTimeUtcArray = new[]
        {
            new DateTimeOffset(new DateTime(2024, 6, 10, 10, 20, 56), TimeSpan.FromHours(0)),
            new DateTimeOffset(new DateTime(2024, 6, 11, 10, 20, 56), TimeSpan.FromHours(0)),
            new DateTimeOffset(new DateTime(2024, 6, 12, 10, 20, 56), TimeSpan.FromHours(0))
        };
        var json7 = JsonConvert.SerializeObject(dateTimeUtcArray, settings);
        Assert.That(json7, Is.EqualTo("[\"2024-06-10T10:20:56Z\",\"2024-06-11T10:20:56Z\",\"2024-06-12T10:20:56Z\"]"));
        var arrayDeserialized7 = JsonConvert.DeserializeObject<object>(json7, settings);
        Assert.That(arrayDeserialized7, Is.EqualTo(expectedDateTimeUtcArray));

        var currentOffset = TimeZoneInfo.Local.GetUtcOffset(new DateTime(2024, 6, 10, 10, 20, 56));
        var offsetString = currentOffset >= TimeSpan.Zero ? $"+{currentOffset:hh\\:mm}" : $"{currentOffset:hh\\:mm}";
        var dateTimeLocalArray = new[]
        {
            new DateTime(2024, 6, 10, 10, 20, 56, DateTimeKind.Local), new DateTime(2024, 6, 11, 10, 20, 56, DateTimeKind.Local),
            new DateTime(2024, 6, 12, 10, 20, 56, DateTimeKind.Local)
        };
        var expectedDateTimeLocalArray = new[]
        {
            new DateTimeOffset(new DateTime(2024, 6, 10, 10, 20, 56), currentOffset),
            new DateTimeOffset(new DateTime(2024, 6, 11, 10, 20, 56), currentOffset),
            new DateTimeOffset(new DateTime(2024, 6, 12, 10, 20, 56), currentOffset)
        };
        var json8 = JsonConvert.SerializeObject(dateTimeLocalArray, settings);
        Assert.That(json8,
            Is.EqualTo(
                $"[\"2024-06-10T10:20:56{offsetString}\",\"2024-06-11T10:20:56{offsetString}\",\"2024-06-12T10:20:56{offsetString}\"]"));
        var arrayDeserialized8 = JsonConvert.DeserializeObject<object>(json8, settings);
        Assert.That(arrayDeserialized8, Is.EqualTo(expectedDateTimeLocalArray));

        var dateTimeUnspecifiedArray = new[]
        {
            new DateTime(2024, 6, 10, 10, 20, 56, DateTimeKind.Unspecified),
            new DateTime(2024, 6, 11, 10, 20, 56, DateTimeKind.Unspecified),
            new DateTime(2024, 6, 12, 10, 20, 56, DateTimeKind.Unspecified)
        };
        var expectedDateTimeUnspecifiedArray = new[]
        {
            new DateTimeOffset(new DateTime(2024, 6, 10, 10, 20, 56), currentOffset),
            new DateTimeOffset(new DateTime(2024, 6, 11, 10, 20, 56), currentOffset),
            new DateTimeOffset(new DateTime(2024, 6, 12, 10, 20, 56), currentOffset)
        };
        var json9 = JsonConvert.SerializeObject(dateTimeUnspecifiedArray, settings);
        Assert.That(json9, Is.EqualTo("[\"2024-06-10T10:20:56\",\"2024-06-11T10:20:56\",\"2024-06-12T10:20:56\"]"));
        var arrayDeserialized9 = JsonConvert.DeserializeObject<object>(json9, settings);
        Assert.That(arrayDeserialized9, Is.EqualTo(expectedDateTimeUnspecifiedArray));

        var dateTimeOffsetArray = new[]
        {
            new DateTimeOffset(new DateTime(2024, 6, 10, 10, 20, 56), TimeSpan.FromHours(3)),
            new DateTimeOffset(new DateTime(2024, 6, 11, 10, 20, 56), TimeSpan.FromHours(0)),
            new DateTimeOffset(new DateTime(2024, 6, 12, 10, 20, 56), TimeSpan.FromHours(-5))
        };
        var json10 = JsonConvert.SerializeObject(dateTimeOffsetArray, settings);
        Assert.That(json10,
            Is.EqualTo("[\"2024-06-10T10:20:56+03:00\",\"2024-06-11T10:20:56+00:00\",\"2024-06-12T10:20:56-05:00\"]"));
        var arrayDeserialized10 = JsonConvert.DeserializeObject<object>(json10, settings);
        Assert.That(arrayDeserialized10, Is.EqualTo(dateTimeOffsetArray));

        var timeSpanArray = new[]
        {
            new TimeSpan(1, 2, 3), new TimeSpan(4, 5, 6), new TimeSpan(7, 8, 9)
        };
        var expectedTimeSpanArray = new[]
        {
            "01:02:03", "04:05:06", "07:08:09"
        };
        var json11 = JsonConvert.SerializeObject(timeSpanArray, settings);
        Assert.That(json11, Is.EqualTo("[\"01:02:03\",\"04:05:06\",\"07:08:09\"]"));
        var arrayDeserialized11 = JsonConvert.DeserializeObject<object>(json11, settings);
        Assert.That(arrayDeserialized11, Is.EqualTo(expectedTimeSpanArray));

        var timeOnlyArray = new[]
        {
            new TimeOnly(10, 20, 56), new TimeOnly(11, 20, 56), new TimeOnly(12, 20, 56)
        };
        var expectedTimeOnlyArray = new[]
        {
            "10:20:56", "11:20:56", "12:20:56"
        };
        var json12 = JsonConvert.SerializeObject(timeOnlyArray, settings);
        Assert.That(json12, Is.EqualTo("[\"10:20:56\",\"11:20:56\",\"12:20:56\"]"));
        var arrayDeserialized12 = JsonConvert.DeserializeObject<object>(json12, settings);
        Assert.That(arrayDeserialized12, Is.EqualTo(expectedTimeOnlyArray));

        var guidArray = new[]
        {
            new Guid("00000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000002"),
            new Guid("00000000-0000-0000-0000-000000000003")
        };
        var expectedGuidArray = new[]
        {
            "00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000002", "00000000-0000-0000-0000-000000000003"
        };
        var json13 = JsonConvert.SerializeObject(guidArray, settings);
        Assert.That(json13,
            Is.EqualTo(
                "[\"00000000-0000-0000-0000-000000000001\",\"00000000-0000-0000-0000-000000000002\",\"00000000-0000-0000-0000-000000000003\"]"));
        var arrayDeserialized13 = JsonConvert.DeserializeObject<object>(json13, settings);
        Assert.That(arrayDeserialized13, Is.EqualTo(expectedGuidArray));

        var enumAsIntArray = new[] { (int)Gender.Unknown, (int)Gender.Male, (int)Gender.Female };
        var json14 = JsonConvert.SerializeObject(enumAsIntArray, settings);
        Assert.That(json14, Is.EqualTo("[0,1,2]"));
        var arrayDeserialized14 = JsonConvert.DeserializeObject<object>(json14, settings);
        Assert.That(arrayDeserialized14, Is.EqualTo(enumAsIntArray));

        var enumAsStringArray = new[] { Gender.Unknown.ToString(), Gender.Male.ToString(), Gender.Female.ToString() };
        var json15 = JsonConvert.SerializeObject(enumAsStringArray, settings);
        Assert.That(json15, Is.EqualTo("[\"Unknown\",\"Male\",\"Female\"]"));
        var arrayDeserialized15 = JsonConvert.DeserializeObject<object>(json15, settings);
        Assert.That(arrayDeserialized15, Is.EqualTo(enumAsStringArray));

        var boolNullableArray = new bool?[] { true, false, null };
        var json16 = JsonConvert.SerializeObject(boolNullableArray, settings);
        Assert.That(json16, Is.EqualTo("[true,false,null]"));
        var arrayDeserialized16 = JsonConvert.DeserializeObject<object>(json16, settings);
        Assert.That(arrayDeserialized16, Is.EqualTo(boolNullableArray));
    }

    [Test]
    public void SerializeFilterShortFormTest()
    {
        var settingsFullForm = NewtonsoftHelper.GetSerializerSettings();
        var settingsShortForm = NewtonsoftHelper.GetSerializerSettings();
        settingsShortForm.Converters.Add(new FlexFetcherDataFilterConverter());

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

        // That's how json looks like with default serializer
        var filterFullSerialized = JsonConvert.SerializeObject(filter, settingsFullForm);
        Assert.That(filterFullSerialized.Length, Is.EqualTo(242));
        Assert.That(filterFullSerialized,
            Is.EqualTo(
                "{\"Logic\":\"And\",\"Filters\":[{\"Logic\":null,\"Filters\":null,\"Operator\":\"Neq\",\"Field\":\"Address\",\"Value\":null},{\"Logic\":null,\"Filters\":null,\"Operator\":\"Eq\",\"Field\":\"Address.Location\",\"Value\":\"Chicago, IL\"}],\"Operator\":null,\"Field\":null,\"Value\":null}"));

        // That's how json looks like with custom "short form" serializer
        var filterShortSerialized = JsonConvert.SerializeObject(filter, settingsShortForm);
        Assert.That(filterShortSerialized.Length, Is.EqualTo(105));
        Assert.That(filterShortSerialized,
            Is.EqualTo(
                "{\"L\":\"And\",\"Fs\":[{\"O\":\"Neq\",\"F\":\"Address\",\"V\":null},{\"O\":\"Eq\",\"F\":\"Address.Location\",\"V\":\"Chicago, IL\"}]}"));

        // That's how json looks like with full form but without null values, i.e. how external clients should send it
        var filterFullJson =
            "{\"Logic\":\"And\",\"Filters\":[{\"Operator\":\"Neq\",\"Field\":\"Address\",\"Value\":null},{\"Operator\":\"Eq\",\"Field\":\"Address.Location\",\"Value\":\"Chicago, IL\"}]}";
        Assert.That(filterFullJson.Length, Is.EqualTo(144));

        // Deserialize jsons and validate
        var deserializedFilterFull = JsonConvert.DeserializeObject<DataFilter>(filterFullSerialized, settingsFullForm);
        var deserializedFilterShort = JsonConvert.DeserializeObject<DataFilter>(filterShortSerialized, settingsShortForm);
        // Serialize with default serializer and compare to make sure that filters are the same, i.e. full form is correct
        var originalFilterSerializedDefault = JsonConvert.SerializeObject(filter);
        var filterFullSerializedDefault = JsonConvert.SerializeObject(deserializedFilterFull);
        Assert.That(filterFullSerializedDefault, Is.EqualTo(originalFilterSerializedDefault));

        // Serialize with default serializer and compare to make sure that filters are the same, i.e. short form is correct
        var filterShortSerializedDefault = JsonConvert.SerializeObject(deserializedFilterShort);
        Assert.That(filterShortSerializedDefault, Is.EqualTo(originalFilterSerializedDefault));
    }

    [Test]
    public void SerializeSorterShortFormTest()
    {
        var settingsFullForm = NewtonsoftHelper.GetSerializerSettings();
        var settingsShortForm = NewtonsoftHelper.GetSerializerSettings();
        settingsShortForm.Converters.Add(new FlexFetcherDataSorterConverter());
        settingsShortForm.Converters.Add(new FlexFetcherDataSortersConverter());

        var sorter = new DataSorters
        {
            Sorters = new List<DataSorter>
            {
                new()
                {
                    Field = "Name",
                    Direction = DataSorterDirection.Asc
                },
                new()
                {
                    Field = "Age",
                    Direction = DataSorterDirection.Desc
                }
            }
        };

        // That's how json looks like with default serializer
        var sorterFullSerialized = JsonConvert.SerializeObject(sorter, settingsFullForm);
        Assert.That(sorterFullSerialized.Length, Is.EqualTo(83));
        Assert.That(sorterFullSerialized,
            Is.EqualTo("{\"Sorters\":[{\"Field\":\"Name\",\"Direction\":\"Asc\"},{\"Field\":\"Age\",\"Direction\":\"Desc\"}]}"));

        // That's how json looks like with custom "short form" serializer
        var sorterShortSerialized = JsonConvert.SerializeObject(sorter, settingsShortForm);
        Assert.That(sorterShortSerialized.Length, Is.EqualTo(53));
        Assert.That(sorterShortSerialized, Is.EqualTo("{\"S\":[{\"F\":\"Name\",\"D\":\"Asc\"},{\"F\":\"Age\",\"D\":\"Desc\"}]}"));

        // Deserialize jsons and validate
        var deserializedSorterFull = JsonConvert.DeserializeObject<DataSorters>(sorterFullSerialized, settingsFullForm);
        var deserializedSorterShort = JsonConvert.DeserializeObject<DataSorters>(sorterShortSerialized, settingsShortForm);
        // Serialize with default serializer and compare to make sure that sorters are the same, i.e. full form is correct
        var originalSorterSerializedDefault = JsonConvert.SerializeObject(sorter);
        var sorterFullSerializedDefault = JsonConvert.SerializeObject(deserializedSorterFull);
        Assert.That(sorterFullSerializedDefault, Is.EqualTo(originalSorterSerializedDefault));

        // Serialize with default serializer and compare to make sure that sorters are the same, i.e. short form is correct
        var filterShortSerializedDefault = JsonConvert.SerializeObject(deserializedSorterShort);
        Assert.That(filterShortSerializedDefault, Is.EqualTo(originalSorterSerializedDefault));
    }

    [Test]
    public void SerializePagerShortFormTest()
    {
        var settingsFullForm = NewtonsoftHelper.GetSerializerSettings();
        var settingsShortForm = NewtonsoftHelper.GetSerializerSettings();
        settingsShortForm.Converters.Add(new FlexFetcherDataPagerConverter());

        // Variant with Page and PageSize
        var pager1 = new DataPager
        {
            Page = 1,
            PageSize = 10
        };

        // That's how json looks like with default serializer
        var pagerFullSerialized1 = JsonConvert.SerializeObject(pager1, settingsFullForm);
        Assert.That(pagerFullSerialized1.Length, Is.EqualTo(42));
        Assert.That(pagerFullSerialized1, Is.EqualTo("{\"Page\":1,\"PageSize\":10,\"Skip\":0,\"Take\":0}"));

        // That's how json looks like with custom "short form" serializer
        var pagerShortSerialized1 = JsonConvert.SerializeObject(pager1, settingsShortForm);
        Assert.That(pagerShortSerialized1.Length, Is.EqualTo(15));
        Assert.That(pagerShortSerialized1, Is.EqualTo("{\"P\":1,\"Ps\":10}"));

        // That's how json looks like with full form but without null values, i.e. how external clients should send it
        var pagerFullJson1 = "{\"Page\":1,\"PageSize\":10}";
        Assert.That(pagerFullJson1.Length, Is.EqualTo(24));

        // Deserialize jsons and validate
        var deserializedPagerFull1 = JsonConvert.DeserializeObject<DataPager>(pagerFullSerialized1, settingsFullForm);
        var deserializedPagerShort1 = JsonConvert.DeserializeObject<DataPager>(pagerShortSerialized1, settingsShortForm);
        // Serialize with default serializer and compare to make sure that pager is the same, i.e. full form is correct
        var originalPagerSerializedDefault1 = JsonConvert.SerializeObject(pager1);
        var pagerFullSerializedDefault1 = JsonConvert.SerializeObject(deserializedPagerFull1);
        Assert.That(pagerFullSerializedDefault1, Is.EqualTo(originalPagerSerializedDefault1));

        // Serialize with default serializer and compare to make sure that pager is the same, i.e. short form is correct
        var pagerShortSerializedDefault1 = JsonConvert.SerializeObject(deserializedPagerShort1);
        Assert.That(pagerShortSerializedDefault1, Is.EqualTo(originalPagerSerializedDefault1));

        // Variant with Skip and Take
        var pager2 = new DataPager
        {
            Skip = 20,
            Take = 30
        };

        // That's how json looks like with default serializer
        var pagerFullSerialized2 = JsonConvert.SerializeObject(pager2, settingsFullForm);
        Assert.That(pagerFullSerialized2.Length, Is.EqualTo(43));
        Assert.That(pagerFullSerialized2, Is.EqualTo("{\"Page\":0,\"PageSize\":0,\"Skip\":20,\"Take\":30}"));

        // That's how json looks like with custom "short form" serializer
        var pagerShortSerialized2 = JsonConvert.SerializeObject(pager2, settingsShortForm);
        Assert.That(pagerShortSerialized2.Length, Is.EqualTo(15));
        Assert.That(pagerShortSerialized2, Is.EqualTo("{\"S\":20,\"T\":30}"));

        // That's how json looks like with full form but without null values, i.e. how external clients should send it
        var pagerFullJson2 = "{\"Skip\":20,\"Take\":30}";
        Assert.That(pagerFullJson2.Length, Is.EqualTo(21));

        // Deserialize jsons and validate
        var deserializedPagerFull2 = JsonConvert.DeserializeObject<DataPager>(pagerFullSerialized2, settingsFullForm);
        var deserializedPagerShort2 = JsonConvert.DeserializeObject<DataPager>(pagerShortSerialized2, settingsShortForm);
        // Serialize with default serializer and compare to make sure that pager is the same, i.e. full form is correct
        var originalPagerSerializedDefault2 = JsonConvert.SerializeObject(pager2);
        var pagerFullSerializedDefault2 = JsonConvert.SerializeObject(deserializedPagerFull2);
        Assert.That(pagerFullSerializedDefault2, Is.EqualTo(originalPagerSerializedDefault2));

        // Serialize with default serializer and compare to make sure that pager is the same, i.e. short form is correct
        var pagerShortSerializedDefault2 = JsonConvert.SerializeObject(deserializedPagerShort2);
        Assert.That(pagerShortSerializedDefault2, Is.EqualTo(originalPagerSerializedDefault2));
    }

    [Test]
    public void SerializeDataQueryShortFormTest()
    {
        var settingsFullForm = NewtonsoftHelper.GetSerializerSettings();
        var settingsShortForm = NewtonsoftHelper.GetSerializerSettings();
        settingsShortForm.Converters.Add(new FlexFetcherDataFilterConverter());
        settingsShortForm.Converters.Add(new FlexFetcherDataSorterConverter());
        settingsShortForm.Converters.Add(new FlexFetcherDataSortersConverter());
        settingsShortForm.Converters.Add(new FlexFetcherDataPagerConverter());
        settingsShortForm.Converters.Add(new FlexFetcherDataQueryConverter());

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

        var sorters = new DataSorters
        {
            Sorters = new List<DataSorter>
            {
                new()
                {
                    Field = "Name",
                    Direction = DataSorterDirection.Asc
                },
                new()
                {
                    Field = "Age",
                    Direction = DataSorterDirection.Desc
                }
            }
        };

        var pager = new DataPager
        {
            Page = 1,
            PageSize = 10
        };

        var dataQuery = new DataQuery
        {
            Filter = filter,
            Sorters = sorters,
            Pager = pager
        };

        // That's how json looks like with default serializer
        var dataQueryFullSerialized = JsonConvert.SerializeObject(dataQuery, settingsFullForm);
        Assert.That(dataQueryFullSerialized.Length, Is.EqualTo(398));
        Assert.That(dataQueryFullSerialized, Is.EqualTo("{\"Filter\":" +
                                                        "{\"Logic\":\"And\",\"Filters\":" +
                                                        "[{\"Logic\":null,\"Filters\":null,\"Operator\":\"Neq\",\"Field\":\"Address\",\"Value\":null}," +
                                                        "{\"Logic\":null,\"Filters\":null,\"Operator\":\"Eq\",\"Field\":\"Address.Location\",\"Value\":\"Chicago, IL\"}]," +
                                                        "\"Operator\":null,\"Field\":null,\"Value\":null}," +
                                                        "\"Sorters\":{\"Sorters\":" +
                                                        "[{\"Field\":\"Name\",\"Direction\":\"Asc\"}," +
                                                        "{\"Field\":\"Age\",\"Direction\":\"Desc\"}]}," +
                                                        "\"Pager\":{\"Page\":1,\"PageSize\":10,\"Skip\":0,\"Take\":0}}"));

        // That's how json looks like with custom "short form" serializer
        var dataQueryShortSerialized = JsonConvert.SerializeObject(dataQuery, settingsShortForm);
        Assert.That(dataQueryShortSerialized.Length, Is.EqualTo(189));
        Assert.That(dataQueryShortSerialized, Is.EqualTo("{\"F\":" +
                                                         "{\"L\":\"And\",\"Fs\":" +
                                                         "[{\"O\":\"Neq\",\"F\":\"Address\",\"V\":null}," +
                                                         "{\"O\":\"Eq\",\"F\":\"Address.Location\",\"V\":\"Chicago, IL\"}]" +
                                                         "}," +
                                                         "\"S\":{\"S\":" +
                                                         "[{\"F\":\"Name\",\"D\":\"Asc\"}," +
                                                         "{\"F\":\"Age\",\"D\":\"Desc\"}]}," +
                                                         "\"P\":{\"P\":1,\"Ps\":10}}"));

        // That's how json looks like with full form but without null values, i.e. how external clients should send it
        var dataQueryFullJson =
            "{\"Filter\":" +
            "{\"Logic\":\"And\",\"Filters\":" +
            "[{\"Operator\":\"Neq\",\"Field\":\"Address\",\"Value\":null}," +
            "{\"Operator\":\"Eq\",\"Field\":\"Address.Location\",\"Value\":\"Chicago, IL\"}]" +
            "}," +
            "\"Sorters\":{\"Sorters\":" +
            "[{\"Field\":\"Name\",\"Direction\":\"Asc\"}," +
            "{\"Field\":\"Age\",\"Direction\":\"Desc\"}]}," +
            "\"Pager\":{\"Page\":1,\"PageSize\":10}}";
        Assert.That(dataQueryFullJson.Length, Is.EqualTo(282));

        // Deserialize jsons and validate
        var deserializedDataQueryFull = JsonConvert.DeserializeObject<DataQuery>(dataQueryFullSerialized, settingsFullForm);
        var deserializedDataQueryShort = JsonConvert.DeserializeObject<DataQuery>(dataQueryShortSerialized, settingsShortForm);
        // Serialize with default serializer and compare to make sure that dataQueries are the same, i.e. full form is correct
        var originalDataQuerySerializedDefault = JsonConvert.SerializeObject(dataQuery);
        var dataQueryFullSerializedDefault = JsonConvert.SerializeObject(deserializedDataQueryFull);
        Assert.That(dataQueryFullSerializedDefault, Is.EqualTo(originalDataQuerySerializedDefault));

        // Serialize with default serializer and compare to make sure that dataQueries are the same, i.e. short form is correct
        var dataQueryShortSerializedDefault = JsonConvert.SerializeObject(deserializedDataQueryShort);
        Assert.That(dataQueryShortSerializedDefault, Is.EqualTo(originalDataQuerySerializedDefault));
    }
}