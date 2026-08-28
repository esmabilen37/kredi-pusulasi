using FinansalAsistanApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FinansalAsistanApi.Services;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<BankProfile> BankProfiles =>
        _database.GetCollection<BankProfile>("BankProfiles");

    public IMongoCollection<User> Users =>
        _database.GetCollection<User>("Users");
}