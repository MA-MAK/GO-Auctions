using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace CustomerService.Models;

[BsonIgnoreExtraElements]
public class Customer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public bool Premium { get; set; }
}
