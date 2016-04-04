using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace MongoDBDAL
{
    public class OrderRepository
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;

        public OrderRepository()
        {
            string dOrderConnectionString = ConfigurationManager.ConnectionStrings["dOrder"].ToString();
            _client = new MongoClient(dOrderConnectionString);
            _database = _client.GetDatabase("dOrder");
        }

        public async Task CreateOrder()
        {
            var document = new BsonDocument 
            { 
                {"address",new BsonDocument
                    {
                        {"street","2 Avenue"},
                        {"zipcode","77777"},
                        {"building","1480"},
                        {"coord",new BsonArray{73.9557413, 40.7720266}}
                    }
                },
                {"borough","Gotham"},
                {"cuisine","Indian"},
                {"grades",new BsonArray
                    {
                        new BsonDocument
                        {
                            {"date",new DateTime(2016,03,16,0,0,0,DateTimeKind.Utc)},
                            {"grade","B"},
                            {"score",32}
                        },
                        new BsonDocument
                        {
                            {"date",new DateTime(2016,6,1,0,0,0,DateTimeKind.Utc)}
                        }
                    }
                },
                {"name","Lunchbox"},
                {"restaurant_id","444444"}
            };

            var collection = _database.GetCollection<BsonDocument>("restaurants");
            await collection.InsertOneAsync(document);
        }

        public async Task<List<BsonDocument>> GetOrder()
        {
            List<BsonDocument> result = new List<BsonDocument>();
            var collection = _database.GetCollection<BsonDocument>("restaurants");
            var filter = new BsonDocument();

            using (var cursor = await collection.FindAsync(filter))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;

                    foreach (var document in batch)
                    {
                        result.Add(document);
                    }
                }
            }

            return result;
        }

        public async Task<List<BsonDocument>> GetOrderByBorough()
        {
            var collection = _database.GetCollection<BsonDocument>("restaurants");
            var filter = Builders<BsonDocument>.Filter.Eq("borough", "Gotham");
            var result = await collection.Find(filter).ToListAsync();

            return result;
        }
                  
        public async Task<List<BsonDocument>> GetOrdersByCuisineAndZipcode()
        {
            var collection = _database.GetCollection<BsonDocument>("restaurants");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("cuisine", "Indian") & builder.Eq("address.zipcode", "77777");
            var result = await collection.Find(filter).ToListAsync();
            return result;
        }

        public async Task<List<BsonDocument>> GetOrdersByCuisineORZipcode()
        {
            var collection = _database.GetCollection<BsonDocument>("restaurants");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("cuisine", "Indian") | builder.Eq("address.zipcode", "10075");
            var result = await collection.Find(filter).ToListAsync();
            return result;
        }

        public async Task<List<BsonDocument>> SortOrders()
        {
            var collection = _database.GetCollection<BsonDocument>("restaurants");
            var filter = new BsonDocument();
            var sort = Builders<BsonDocument>.Sort.Ascending("borough").Ascending("address.zipcode");
            var result = await collection.Find(filter).Sort(sort).ToListAsync();

            return result;
        }

        public async Task UpdateOrder()
        {
            var collection = _database.GetCollection<BsonDocument>("restaurants");
            var filter = Builders<BsonDocument>.Filter.Eq("name", "Lunchbox");
            var update = Builders<BsonDocument>.Update.Set("cuisine", "South Indian(New)").CurrentDate("lastModified");
            var result = await collection.UpdateOneAsync(filter, update);

            result.MatchedCount.Should().Be(1);
            if (result.IsModifiedCountAvailable)
            {
                result.ModifiedCount.Should().Be(1);
            }
        }

        public async Task UpdateOrderAddress()
        {
            var collection = _database.GetCollection<BsonDocument>("restaurants");
            var filter = Builders<BsonDocument>.Filter.Eq("restaurant_id", "1232328");
            var update = Builders<BsonDocument>.Update.Set("address.street", "East 31st street");
            var result = await collection.UpdateOneAsync(filter, update);

            result.MatchedCount.Should().Be(1);
            if (result.IsModifiedCountAvailable)
            {
                result.ModifiedCount.Should().Be(1);
            }
        }

        public async Task UpdateMany()
        {
            var collection = _database.GetCollection<BsonDocument>("restaurants");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("address.zipcode", "77777") & builder.Eq("cuisine", "Indian");
            var update = Builders<BsonDocument>.Update
                .Set("cuisine", "Category To Be Determined")
                .CurrentDate("lastModified");
            var result = await collection.UpdateManyAsync(filter, update);

        }

        public async Task ReplaceOrder()
        {
            var collection = _database.GetCollection<BsonDocument>("restaurants");
            var filter = Builders<BsonDocument>.Filter.Eq("restaurant_id", "88888");
            var docForReplace = new BsonDocument            
            { 
                {"address",new BsonDocument
                    {
                        {"street","MG Road"},
                        {"zipcode","888888"},
                        {"building","1480"},
                        {"coord",new BsonArray{73.9557413, 40.7720266}}
                    }
                },
                {"borough","Gotham"},
                {"cuisine","Indiana"},
                {"grades",new BsonArray
                    {
                        new BsonDocument
                        {
                            {"date",new DateTime(2016,03,16,0,0,0,DateTimeKind.Utc)},
                            {"grade","A+"},
                            {"score",32}
                        },
                        new BsonDocument
                        {
                            {"date",new DateTime(2016,6,1,0,0,0,DateTimeKind.Utc)}
                        }
                    }
                },
                {"name","New LunchBox launched"},
                {"restaurant_id","99999"}
            };

            var result = await collection.ReplaceOneAsync(filter, docForReplace);

        }

        public async Task DeleteOrders()
        {
            var collection = _database.GetCollection<BsonDocument>("restaurants");
            var filter = Builders<BsonDocument>.Filter.Eq("borough", "Manhattan");
            var result = await collection.DeleteManyAsync(filter);
        }

        public async Task DeleteAllOrders()
        {
            var collection = _database.GetCollection<BsonDocument>("restaurants");
            var filter = new BsonDocument();
            var result = await collection.DeleteManyAsync(filter);
        }
     
    }
}
