using Newtonsoft.Json;

namespace FlexFetcherTests.SerializationTests;

public class NewtonsoftTests
{
    [Test]
    public void SerializeDatesTest()
    {
        var dt1 = new DateTime(2024, 6, 10, 10, 20, 56, DateTimeKind.Local);
        var dt2 = new DateTime(2024, 6, 10, 10, 20, 56, DateTimeKind.Utc);
        var dt1Utc = dt1.ToUniversalTime();
        var dt2Utc = dt2.ToUniversalTime();
        var dtJson1Utc = JsonConvert.SerializeObject(dt1Utc);
        var dtJson2Utc = JsonConvert.SerializeObject(dt2Utc);
        var dto1 = new DateTimeOffset(new DateTime(2024, 6, 10, 10, 20, 56), TimeSpan.FromHours(3));
        var dto2 = new DateTimeOffset(new DateTime(2024, 6, 10, 10, 20, 56), TimeSpan.FromHours(0));
        
        // Serialize with default settings
        var dtJson1 = JsonConvert.SerializeObject(dt1);
        Assert.That(dtJson1, Is.EqualTo("\"2024-06-10T10:20:56+03:00\""));
        var dtJson2 = JsonConvert.SerializeObject(dt2);
        Assert.That(dtJson2, Is.EqualTo("\"2024-06-10T10:20:56Z\""));
        var dto1Json = JsonConvert.SerializeObject(dto1);
        Assert.That(dto1Json, Is.EqualTo("\"2024-06-10T10:20:56+03:00\""));
        var dto2Json = JsonConvert.SerializeObject(dto2);
        Assert.That(dto2Json, Is.EqualTo("\"2024-06-10T10:20:56+00:00\""));

        var settings = new JsonSerializerSettings();
        settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
        settings.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
        settings.DateParseHandling = DateParseHandling.DateTimeOffset;

        // Serialize with custom settings
        var dtJson1Custom = JsonConvert.SerializeObject(dt1, settings);
        Assert.That(dtJson1Custom, Is.EqualTo("\"2024-06-10T10:20:56+03:00\""));
        var dtJson2Custom = JsonConvert.SerializeObject(dt2, settings);
        Assert.That(dtJson2Custom, Is.EqualTo("\"2024-06-10T10:20:56Z\""));
        var dto1JsonCustom = JsonConvert.SerializeObject(dto1, settings);
        Assert.That(dto1JsonCustom, Is.EqualTo("\"2024-06-10T10:20:56+03:00\""));
        var dto2JsonCustom = JsonConvert.SerializeObject(dto2, settings);
        Assert.That(dto2JsonCustom, Is.EqualTo("\"2024-06-10T10:20:56+00:00\""));

        // Make sure that JSONs are the same
        Assert.That(dtJson1, Is.EqualTo(dtJson1Custom));
        Assert.That(dtJson2, Is.EqualTo(dtJson2Custom));
        Assert.That(dto1Json, Is.EqualTo(dto1JsonCustom));
        Assert.That(dto2Json, Is.EqualTo(dto2JsonCustom));

        var dt1Deserialized = JsonConvert.DeserializeObject<DateTime>(dtJson1, settings);
        Assert.That(dt1Deserialized, Is.EqualTo(dt1));
        var dt2Deserialized = JsonConvert.DeserializeObject<DateTime>(dtJson2, settings);
        Assert.That(dt2Deserialized, Is.EqualTo(dt2));
        var dt1UtcDeserialized = JsonConvert.DeserializeObject<DateTime>(dtJson1Utc, settings);
        Assert.That(dt1UtcDeserialized, Is.EqualTo(new DateTime(2024, 6, 10, 7, 20, 56, DateTimeKind.Local)));
        var dt2UtcDeserialized = JsonConvert.DeserializeObject<DateTime>(dtJson2Utc, settings);
        Assert.That(dt2UtcDeserialized, Is.EqualTo(dt2));
        var dto1Deserialized = JsonConvert.DeserializeObject<DateTimeOffset>(dto1Json, settings);
        Assert.That(dto1Deserialized, Is.EqualTo(dto1));
        var dto2Deserialized = JsonConvert.DeserializeObject<DateTimeOffset>(dto2Json, settings);
        Assert.That(dto2Deserialized, Is.EqualTo(dto2));

        var dt1DeserializedObject = JsonConvert.DeserializeObject<object>(dtJson1, settings);
        Assert.That(dt1DeserializedObject, Is.EqualTo(new DateTimeOffset(dt1)));
        var dt2DeserializedObject = JsonConvert.DeserializeObject<object>(dtJson2, settings);
        Assert.That(dt2DeserializedObject, Is.EqualTo(new DateTimeOffset(dt2)));
        var dt1UtcDeserializedObject = JsonConvert.DeserializeObject<object>(dtJson1Utc, settings);
        Assert.That(dt1UtcDeserializedObject, Is.EqualTo(new DateTimeOffset(dt1Utc)));
        var dt2UtcDeserializedObject = JsonConvert.DeserializeObject<object>(dtJson2Utc, settings);
        Assert.That(dt2UtcDeserializedObject, Is.EqualTo(new DateTimeOffset(dt2Utc)));
        var dto1DeserializedObject = JsonConvert.DeserializeObject<object>(dto1Json, settings);
        Assert.That(dto1DeserializedObject, Is.EqualTo(dto1));
        var dto2DeserializedObject = JsonConvert.DeserializeObject<object>(dto2Json, settings);
        Assert.That(dto2DeserializedObject, Is.EqualTo(dto2));
    }
}