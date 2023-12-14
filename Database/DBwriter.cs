using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EscapeFromTheWoods;

public class DBwriter {
    private string _connectionString;
    private IMongoClient mongoClient;
    private IMongoDatabase mongoDB;

    public DBwriter(string connectionString) {
        _connectionString = connectionString;
        mongoClient = new MongoClient(connectionString);
        mongoDB = mongoClient.GetDatabase("EscapeFromTheWoods");
    }
    public async Task WriteWoodRecordsAsync(List<DBWoodRecord> data) {
        var collection = mongoDB.GetCollection<BsonDocument>("Wood");
        var document = data.Select(x => new BsonDocument
        { { "woodID", x.woodID },
          { "treeID", x.treeID },
          { "x", x.x },
          { "y", x.y }, });
        await collection.InsertManyAsync(document);
    }

    public async Task WriteMonkeyRecordsAsyncDb(List<DBMonkeyRecord> data) {
        var collection = mongoDB.GetCollection<BsonDocument>("Monkey");
        var document = data.Select(x => new BsonDocument
        { 
            { "monkeyID", x.monkeyID },
            { "monkeyName", x.monkeyName },
            { "woodID", x.woodID },
            { "seqNr", x.seqNr },
            { "treeID", x.treeID },
            { "x", x.x },
            { "y", x.y }
        });
        await collection.InsertManyAsync(document);
    }
}