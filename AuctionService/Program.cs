using NLog;
using NLog.Web;
using AuctionService.Services;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using AuctionService.Models;

//var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
//logger.Debug("init main");
try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    builder.Services.AddLogging();
    builder.Services.AddHttpClient("ItemService", client =>
    {
        client.BaseAddress = new Uri("http://localhost:5164");
    });

    builder.Services.AddHttpClient("BidService", client =>
    {
        client.BaseAddress = new Uri("http://localhost:5223");
    });

    builder.Services.AddSingleton<IAuctionRepository, AuctionRepository>();

    builder.Services.AddSingleton<IItemRepository, ItemRepository>(
        b => new ItemRepository(b.GetService<IHttpClientFactory>()
        .CreateClient("ItemService"), 
        builder.Services.BuildServiceProvider().GetRequiredService<ILogger<ItemRepository>>()));
    
    builder.Services.AddSingleton<IBidRepository, BidRepository>(
        b => new BidRepository(b.GetService<IHttpClientFactory>()
        .CreateClient("BidService"), 
        builder.Services.BuildServiceProvider().GetRequiredService<ILogger<BidRepository>>()));

    // MongoDB configuration
    var connectionString = builder.Configuration.GetConnectionString("MongoDBConnection");
    var databaseName = builder.Configuration.GetSection("MongoDBSettings:DatabaseName").Value;
    var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<MongoDBContext>>();
    builder.Services.AddSingleton<MongoDBContext>(provider => new MongoDBContext(logger, builder.Configuration));

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

}
catch (Exception ex)
{
    //logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}

