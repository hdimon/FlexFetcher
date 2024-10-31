using FlexFetcher;
using FlexFetcher.DependencyInjection.Microsoft;
using FlexFetcher.Models.FlexFetcherOptions;
using FlexFetcher.Serialization.SystemTextJson.Converters;
using TestData.Database;
using WebApiSample.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new FlexFetcherModelBinderProvider());
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new GenericConverter());
    options.JsonSerializerOptions.Converters.Add(new FlexFetcherDataSorterConverter());
    options.JsonSerializerOptions.Converters.Add(new FlexFetcherDataSortersConverter());
    options.JsonSerializerOptions.Converters.Add(new FlexFetcherDataPagerConverter());
    options.JsonSerializerOptions.Converters.Add(new FlexFetcherDataFilterConverter());
    options.JsonSerializerOptions.Converters.Add(new FlexFetcherDataQueryConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// FlexFetcher setup
builder.Services.AddSingletonFlexOptions<FlexFetcherOptions<AddressEntity>>(options =>
{
    options.Field(entity => entity.City).Map("Town");
});
builder.Services.AddSingleton<FlexFetcher<AddressEntity>>();
builder.Services.AddSingletonFlexOptions<FlexFetcherOptions<PeopleEntity>>((provider, options) =>
{
    options.AddNestedFlexFetcher(provider.GetRequiredService<FlexFetcher<AddressEntity>>());
});
builder.Services.AddSingleton<FlexFetcher<PeopleEntity>>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
