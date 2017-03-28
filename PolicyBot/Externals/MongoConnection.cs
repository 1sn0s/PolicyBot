using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using PolicyBot.Models;
using System.Threading.Tasks;

namespace PolicyBot.Externals
{
    public class MongoConnection
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;
        private const string dbEntryPoint = "test";
        private const string collectionName = "policy";

        public MongoConnection()
        {
            _client = new MongoClient();
            _database = _client.GetDatabase(dbEntryPoint);
        }

        public async Task<Policy> GetPolicyDetailsByKeyAsync(string key)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("key", key);
            var collection = _database.GetCollection<BsonDocument>(collectionName);
            var result = await collection.Find(filter).ToListAsync();
            DocumentModel dt = BsonSerializer.Deserialize<DocumentModel>(result[0]);

            Policy policy = new Policy();
            policy.policyText = dt.text;
            //TODO :- Get the subpolicies here
            return policy;
        }

        //private async Task<List> GetSubpolicy()
    }

    public class DocumentModel
    {
        public object _id { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public string text { get; set; }
        public string[] related { get; set; }
    }

}