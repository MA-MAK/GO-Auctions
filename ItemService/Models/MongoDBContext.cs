using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using ItemService.Models;
using ItemService.Services;

namespace ItemService.Models;

/// <summary>
/// MongoDB database context class.
/// </summary>
public class MongoDBContext 
{
    private ILogger<MongoDBContext> _logger;
    private IConfiguration _config;
    public IMongoDatabase GODatabase { get; set; }
    public IMongoCollection<Item> items { get; set; }

    /// <summary>
    /// Create an instance of the context class.
    /// </summary>
    /// <param name="logger">Global logging facility.</param>
    /// <param name="config">System configuration instance.</param>
    public MongoDBContext(ILogger<MongoDBContext> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
        
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

        var client = new MongoClient(_config["MongoDBSettings:MongoConnectionString"]);
        GODatabase = client.GetDatabase(_config["MongoDBSettings:DatabaseName"]);
        items = GODatabase.GetCollection<Item>(_config["MongoDBSettings:ItemCollection"]);
    }

    public IMongoCollection<Item> Items => GODatabase.GetCollection<Item>("Items");

}
