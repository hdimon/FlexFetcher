using System.Text.Json;
using FlexFetcher.Serialization.SystemTextJson;
using TestData.Database;

namespace FlexFetcherTests.SerializationTests;

public class SystemTextJsonTests
{
    [Test]
    public void SerializeDatesTest()
    {
        var settings = SystemTextJsonHelper.GetSerializerSettings();

        var dt1LocalKind = new DateTime(2024, 6, 10, 10, 20, 56, DateTimeKind.Local);
        var dt2UtcKind = new DateTime(2024, 6, 10, 10, 20, 56, DateTimeKind.Utc);
        var dt3UnspecifiedKind = new DateTime(2024, 6, 10, 10, 20, 56, DateTimeKind.Unspecified);
        var dt1UtcFromLocalKind = dt1LocalKind.ToUniversalTime();
        var dt2UtcFromUtcKind = dt2UtcKind.ToUniversalTime();
        var dt3UtcFromUnspecifiedKind = dt3UnspecifiedKind.ToUniversalTime();
        var dtJson1UtcFromLocalKind = JsonSerializer.Serialize(dt1UtcFromLocalKind);
        var dtJson2UtcFromUtcKind = JsonSerializer.Serialize(dt2UtcFromUtcKind);
        var dtJson3UtcFromUnspecifiedKind = JsonSerializer.Serialize(dt3UtcFromUnspecifiedKind);
        var dto1KindOffset = TimeZoneInfo.Local.GetUtcOffset(new DateTime(2024, 6, 10, 10, 20, 56));
        var dto1KindOffsetString = dto1KindOffset >= TimeSpan.Zero ? $"+{dto1KindOffset:hh\\:mm}" : $"{dto1KindOffset:hh\\:mm}";
        var dto1 = new DateTimeOffset(new DateTime(2024, 6, 10, 10, 20, 56), dto1KindOffset);
        var dto2 = new DateTimeOffset(new DateTime(2024, 6, 10, 10, 20, 56), TimeSpan.FromHours(0));

        Assert.That(dt1LocalKind, Is.EqualTo(dt2UtcKind));
        Assert.That(dt1LocalKind, Is.EqualTo(dt3UnspecifiedKind));
        Assert.That(dt2UtcKind, Is.EqualTo(dt3UnspecifiedKind));

        // Serialize with default settings
        var dt1LocalKindOffset = TimeZoneInfo.Local.GetUtcOffset(dt1LocalKind);
        var dt1LocalKindOffsetString = dt1LocalKindOffset >= TimeSpan.Zero ? $"+{dt1LocalKindOffset:hh\\:mm}" : $"{dt1LocalKindOffset:hh\\:mm}";
        var dtJson1LocalKind = JsonSerializer.Serialize(dt1LocalKind);
        Assert.That(dtJson1LocalKind, Is.EqualTo($"\"2024-06-10T10:20:56{dt1LocalKindOffsetString}\""));
        var dtJson2UtcKind = JsonSerializer.Serialize(dt2UtcKind);
        Assert.That(dtJson2UtcKind, Is.EqualTo("\"2024-06-10T10:20:56Z\""));
        var dtJson3UnspecifiedKind = JsonSerializer.Serialize(dt3UnspecifiedKind);
        Assert.That(dtJson3UnspecifiedKind, Is.EqualTo("\"2024-06-10T10:20:56\""));
        var dto1Json = JsonSerializer.Serialize(dto1);
        Assert.That(dto1Json, Is.EqualTo($"\"2024-06-10T10:20:56{dto1KindOffsetString}\""));
        var dto2Json = JsonSerializer.Serialize(dto2);
        Assert.That(dto2Json, Is.EqualTo("\"2024-06-10T10:20:56+00:00\""));

        // Serialize with custom settings
        var dtJson1Custom = JsonSerializer.Serialize(dt1LocalKind, settings);
        Assert.That(dtJson1Custom, Is.EqualTo($"\"2024-06-10T10:20:56{dt1LocalKindOffsetString}\""));
        var dtJson2Custom = JsonSerializer.Serialize(dt2UtcKind, settings);
        Assert.That(dtJson2Custom, Is.EqualTo("\"2024-06-10T10:20:56Z\""));
        var dtJson3Custom = JsonSerializer.Serialize(dt3UnspecifiedKind, settings);
        Assert.That(dtJson3Custom, Is.EqualTo("\"2024-06-10T10:20:56\""));
        var dto1JsonCustom = JsonSerializer.Serialize(dto1, settings);
        Assert.That(dto1JsonCustom, Is.EqualTo($"\"2024-06-10T10:20:56{dto1KindOffsetString}\""));
        var dto2JsonCustom = JsonSerializer.Serialize(dto2, settings);
        Assert.That(dto2JsonCustom, Is.EqualTo("\"2024-06-10T10:20:56+00:00\""));

        // Make sure that JSONs are the same
        Assert.That(dtJson1LocalKind, Is.EqualTo(dtJson1Custom));
        Assert.That(dtJson2UtcKind, Is.EqualTo(dtJson2Custom));
        Assert.That(dtJson3UnspecifiedKind, Is.EqualTo(dtJson3Custom));
        Assert.That(dto1Json, Is.EqualTo(dto1JsonCustom));
        Assert.That(dto2Json, Is.EqualTo(dto2JsonCustom));

        var dt1Deserialized = JsonSerializer.Deserialize<DateTime>(dtJson1LocalKind);
        Assert.That(dt1Deserialized, Is.EqualTo(dt1LocalKind));
        var dt2Deserialized = JsonSerializer.Deserialize<DateTime>(dtJson2UtcKind);
        Assert.That(dt2Deserialized, Is.EqualTo(dt2UtcKind));
        var dt3Deserialized = JsonSerializer.Deserialize<DateTime>(dtJson3UnspecifiedKind);
        Assert.That(dt3Deserialized, Is.EqualTo(dt3UnspecifiedKind));
        var dt1UtcDeserialized = JsonSerializer.Deserialize<DateTime>(dtJson1UtcFromLocalKind);
        var dt1UtcDeserializedExpected = new DateTime(2024, 6, 10, 10, 20, 56, DateTimeKind.Local).AddHours(-dto1KindOffset.TotalHours);
        Assert.That(dt1UtcDeserialized, Is.EqualTo(dt1UtcDeserializedExpected));
        var dt2UtcDeserialized = JsonSerializer.Deserialize<DateTime>(dtJson2UtcFromUtcKind);
        Assert.That(dt2UtcDeserialized, Is.EqualTo(dt2UtcKind));
        var dt3UtcDeserialized = JsonSerializer.Deserialize<DateTime>(dtJson3UtcFromUnspecifiedKind);
        Assert.That(dt3UtcDeserialized, Is.EqualTo(dt3UtcFromUnspecifiedKind));
        var dto1Deserialized = JsonSerializer.Deserialize<DateTimeOffset>(dto1Json);
        Assert.That(dto1Deserialized, Is.EqualTo(dto1));
        var dto2Deserialized = JsonSerializer.Deserialize<DateTimeOffset>(dto2Json);
        Assert.That(dto2Deserialized, Is.EqualTo(dto2));

        var dt1DeserializedObject = JsonSerializer.Deserialize<object>(dtJson1LocalKind, settings);
        Assert.That(dt1DeserializedObject, Is.EqualTo(new DateTimeOffset(dt1LocalKind)));
        var dt2DeserializedObject = JsonSerializer.Deserialize<object>(dtJson2UtcKind, settings);
        Assert.That(dt2DeserializedObject, Is.EqualTo(new DateTimeOffset(dt2UtcKind)));
        var dt3DeserializedObject = JsonSerializer.Deserialize<object>(dtJson3UnspecifiedKind, settings);
        Assert.That(dt3DeserializedObject, Is.EqualTo(new DateTimeOffset(dt3UnspecifiedKind)));
        var dt1UtcDeserializedObject = JsonSerializer.Deserialize<object>(dtJson1UtcFromLocalKind, settings);
        Assert.That(dt1UtcDeserializedObject, Is.EqualTo(new DateTimeOffset(dt1UtcFromLocalKind)));
        var dt2UtcDeserializedObject = JsonSerializer.Deserialize<object>(dtJson2UtcFromUtcKind, settings);
        Assert.That(dt2UtcDeserializedObject, Is.EqualTo(new DateTimeOffset(dt2UtcFromUtcKind)));
        var dt3UtcDeserializedObject = JsonSerializer.Deserialize<object>(dtJson3UtcFromUnspecifiedKind, settings);
        Assert.That(dt3UtcDeserializedObject, Is.EqualTo(new DateTimeOffset(dt3UtcFromUnspecifiedKind)));
        var dto1DeserializedObject = JsonSerializer.Deserialize<object>(dto1Json, settings);
        Assert.That(dto1DeserializedObject, Is.EqualTo(dto1));
        var dto2DeserializedObject = JsonSerializer.Deserialize<object>(dto2Json, settings);
        Assert.That(dto2DeserializedObject, Is.EqualTo(dto2));
    }

    [Test]
    public void GenericConverterTest()
    {
        var settings = SystemTextJsonHelper.GetSerializerSettings();

        object obj = new object();
        var objJson = JsonSerializer.Serialize(obj, settings);
        Assert.That(objJson, Is.EqualTo("{}"));
        var objDeserialized = JsonSerializer.Deserialize<object>(objJson, settings);
        Assert.That(objDeserialized, Is.TypeOf<object>());

        object[] array = { 1, 2, 3, 4, 5 };
        var arrayJson = JsonSerializer.Serialize(array, settings);
        var deserialized = JsonSerializer.Deserialize<object>(arrayJson, settings);
        Assert.That(deserialized, Is.EqualTo(array));

        var intSer = JsonSerializer.Serialize(1, settings);
        var intDeser = JsonSerializer.Deserialize<object>(intSer, settings);
        Assert.That(intDeser, Is.EqualTo(1));

        var nullSer = JsonSerializer.Serialize<object>(null!, settings);
        var nullDeser = JsonSerializer.Deserialize<object>(nullSer, settings);
        Assert.That(nullDeser, Is.Null);

        var stringSer = JsonSerializer.Serialize("test", settings);
        var stringDeser = JsonSerializer.Deserialize<object>(stringSer, settings);
        Assert.That(stringDeser, Is.EqualTo("test"));
    }

    [Test]
    public void SerializeArrayTest()
    {
        var settings = SystemTextJsonHelper.GetSerializerSettings();

        var intArray = new[] { 1, 2, 3, 4, 5 };
        var json1 = JsonSerializer.Serialize(intArray, settings);
        var arrayDeserialized1 = JsonSerializer.Deserialize<object>(json1, settings);
        Assert.That(arrayDeserialized1, Is.EqualTo(intArray));

        var decimalArray = new[] { 1.1m, 2.2m, 3.3m, 4.4m, 5.5m };
        var json2 = JsonSerializer.Serialize(decimalArray, settings);
        var arrayDeserialized2 = JsonSerializer.Deserialize<object>(json2, settings);
        Assert.That(arrayDeserialized2, Is.EqualTo(decimalArray));

        var stringArray = new[] { "a", "b", "c", "d", "e" };
        var json3 = JsonSerializer.Serialize(stringArray, settings);
        var arrayDeserialized3 = JsonSerializer.Deserialize<object>(json3, settings);
        Assert.That(arrayDeserialized3, Is.EqualTo(stringArray));

        var doubleArray = new[] { 1.1, 2.2, 3.3, 4.4, 5.5 };
        var json4 = JsonSerializer.Serialize(doubleArray, settings);
        var arrayDeserialized4 = JsonSerializer.Deserialize<object>(json4, settings);
        Assert.That(arrayDeserialized4, Is.EqualTo(doubleArray));

        var doubleNullableArray = new double?[] { 1.1, 2.2, null, 3.3, 4.4, 5.5, null };
        var json5 = JsonSerializer.Serialize(doubleNullableArray, settings);
        var arrayDeserialized5 = JsonSerializer.Deserialize<object>(json5, settings);
        Assert.That(arrayDeserialized5, Is.EqualTo(doubleNullableArray));

        var dateOnlyArray = new[] { new DateOnly(2024, 6, 10), new DateOnly(2024, 6, 11), new DateOnly(2024, 6, 12) };
        var json6 = JsonSerializer.Serialize(dateOnlyArray, settings);
        Assert.That(json6, Is.EqualTo("[\"2024-06-10\",\"2024-06-11\",\"2024-06-12\"]"));
        var arrayDeserialized6 = JsonSerializer.Deserialize<object>(json6, settings);
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
        var json7 = JsonSerializer.Serialize(dateTimeUtcArray, settings);
        Assert.That(json7, Is.EqualTo("[\"2024-06-10T10:20:56Z\",\"2024-06-11T10:20:56Z\",\"2024-06-12T10:20:56Z\"]"));
        var arrayDeserialized7 = JsonSerializer.Deserialize<object>(json7, settings);
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
        var json8 = JsonSerializer.Serialize(dateTimeLocalArray, settings);
        Assert.That(json8, Is.EqualTo($"[\"2024-06-10T10:20:56{offsetString}\",\"2024-06-11T10:20:56{offsetString}\",\"2024-06-12T10:20:56{offsetString}\"]"));
        var arrayDeserialized8 = JsonSerializer.Deserialize<object>(json8, settings);
        Assert.That(arrayDeserialized8, Is.EqualTo(expectedDateTimeLocalArray));

        var dateTimeUnspecifiedArray = new[]
        {
            new DateTime(2024, 6, 10, 10, 20, 56, DateTimeKind.Unspecified), new DateTime(2024, 6, 11, 10, 20, 56, DateTimeKind.Unspecified),
            new DateTime(2024, 6, 12, 10, 20, 56, DateTimeKind.Unspecified)
        };
        var expectedDateTimeUnspecifiedArray = new[]
        {
            new DateTimeOffset(new DateTime(2024, 6, 10, 10, 20, 56), currentOffset),
            new DateTimeOffset(new DateTime(2024, 6, 11, 10, 20, 56), currentOffset),
            new DateTimeOffset(new DateTime(2024, 6, 12, 10, 20, 56), currentOffset)
        };
        var json9 = JsonSerializer.Serialize(dateTimeUnspecifiedArray, settings);
        Assert.That(json9, Is.EqualTo("[\"2024-06-10T10:20:56\",\"2024-06-11T10:20:56\",\"2024-06-12T10:20:56\"]"));
        var arrayDeserialized9 = JsonSerializer.Deserialize<object>(json9, settings);
        Assert.That(arrayDeserialized9, Is.EqualTo(expectedDateTimeUnspecifiedArray));

        var dateTimeOffsetArray = new[]
        {
            new DateTimeOffset(new DateTime(2024, 6, 10, 10, 20, 56), TimeSpan.FromHours(3)),
            new DateTimeOffset(new DateTime(2024, 6, 11, 10, 20, 56), TimeSpan.FromHours(0)),
            new DateTimeOffset(new DateTime(2024, 6, 12, 10, 20, 56), TimeSpan.FromHours(-5))
        };
        var json10 = JsonSerializer.Serialize(dateTimeOffsetArray, settings);
        Assert.That(json10, Is.EqualTo("[\"2024-06-10T10:20:56+03:00\",\"2024-06-11T10:20:56+00:00\",\"2024-06-12T10:20:56-05:00\"]"));
        var arrayDeserialized10 = JsonSerializer.Deserialize<object>(json10, settings);
        Assert.That(arrayDeserialized10, Is.EqualTo(dateTimeOffsetArray));

        var timeSpanArray = new[]
        {
            new TimeSpan(1, 2, 3), new TimeSpan(4, 5, 6), new TimeSpan(7, 8, 9)
        };
        var expectedTimeSpanArray = new[]
        {
            "01:02:03", "04:05:06", "07:08:09"
        };
        var json11 = JsonSerializer.Serialize(timeSpanArray, settings);
        Assert.That(json11, Is.EqualTo("[\"01:02:03\",\"04:05:06\",\"07:08:09\"]"));
        var arrayDeserialized11 = JsonSerializer.Deserialize<object>(json11, settings);
        Assert.That(arrayDeserialized11, Is.EqualTo(expectedTimeSpanArray));

        var timeOnlyArray = new[]
        {
            new TimeOnly(10, 20, 56), new TimeOnly(11, 20, 56), new TimeOnly(12, 20, 56)
        };
        var expectedTimeOnlyArray = new[]
        {
            "10:20:56", "11:20:56", "12:20:56"
        };
        var json12 = JsonSerializer.Serialize(timeOnlyArray, settings);
        Assert.That(json12, Is.EqualTo("[\"10:20:56\",\"11:20:56\",\"12:20:56\"]"));
        var arrayDeserialized12 = JsonSerializer.Deserialize<object>(json12, settings);
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
        var json13 = JsonSerializer.Serialize(guidArray, settings);
        Assert.That(json13,
            Is.EqualTo(
                "[\"00000000-0000-0000-0000-000000000001\",\"00000000-0000-0000-0000-000000000002\",\"00000000-0000-0000-0000-000000000003\"]"));
        var arrayDeserialized13 = JsonSerializer.Deserialize<object>(json13, settings);
        Assert.That(arrayDeserialized13, Is.EqualTo(expectedGuidArray));

        var enumAsIntArray = new[] { (int)Gender.Unknown, (int)Gender.Male, (int)Gender.Female };
        var json14 = JsonSerializer.Serialize(enumAsIntArray, settings);
        Assert.That(json14, Is.EqualTo("[0,1,2]"));
        var arrayDeserialized14 = JsonSerializer.Deserialize<object>(json14, settings);
        Assert.That(arrayDeserialized14, Is.EqualTo(enumAsIntArray));

        var enumAsStringArray = new[] { Gender.Unknown.ToString(), Gender.Male.ToString(), Gender.Female.ToString() };
        var json15 = JsonSerializer.Serialize(enumAsStringArray, settings);
        Assert.That(json15, Is.EqualTo("[\"Unknown\",\"Male\",\"Female\"]"));
        var arrayDeserialized15 = JsonSerializer.Deserialize<object>(json15, settings);
        Assert.That(arrayDeserialized15, Is.EqualTo(enumAsStringArray));

        var boolNullableArray = new bool?[] { true, false, null };
        var json16 = JsonSerializer.Serialize(boolNullableArray, settings);
        Assert.That(json16, Is.EqualTo("[true,false,null]"));
        var arrayDeserialized16 = JsonSerializer.Deserialize<object>(json16, settings);
        Assert.That(arrayDeserialized16, Is.EqualTo(boolNullableArray));

        var nullArray = new object[] { null!, null!, null! };
        var json17 = JsonSerializer.Serialize(nullArray, settings);
        Assert.That(json17, Is.EqualTo("[null,null,null]"));
        var arrayDeserialized17 = JsonSerializer.Deserialize<object>(json17, settings);
        Assert.That(arrayDeserialized17, Is.EqualTo(nullArray));
    }
}