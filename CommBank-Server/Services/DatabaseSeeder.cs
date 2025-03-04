using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson.Serialization;


namespace CommBank.Services
{
    public class DatabaseSeeder
    {
        private readonly IMongoDatabase _database;

        public DatabaseSeeder(IConfiguration configuration)
        {
            var mongoConnectionString = configuration.GetConnectionString("CommBank");
            var client = new MongoClient(mongoConnectionString);
            _database = client.GetDatabase("CommBank"); // Ensure this matches your database name
        }

        private async Task SeedCollection(string collectionName, string filePath)
        {
            var collection = _database.GetCollection<BsonDocument>(collectionName);
            await collection.DeleteManyAsync(FilterDefinition<BsonDocument>.Empty); // Clear existing data

            var jsonData = await File.ReadAllTextAsync(filePath);
            var records = BsonSerializer.Deserialize<BsonArray>(jsonData); // Deserialize as BsonArray

            List<BsonDocument> documents = new List<BsonDocument>();
            foreach (var record in records)
            {
                documents.Add(record.AsBsonDocument);
            }

            await collection.InsertManyAsync(documents);
            Console.WriteLine($"{collectionName} seeded successfully!");
        }

        public async Task SeedData()
        {
            await SeedCollection("Goals", "data/Goals.json");
            await SeedCollection("Accounts", "data/Accounts.json");
            await SeedCollection("Users", "data/Users.json");
            await SeedCollection("Transactions", "data/Transactions.json");
            await SeedCollection("Tags", "data/Tags.json");
            Console.WriteLine("Database seeding complete!");
        }
    }
}
