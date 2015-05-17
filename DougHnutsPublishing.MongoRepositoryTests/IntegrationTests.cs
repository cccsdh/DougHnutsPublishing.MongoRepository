#region Copyright Notice

/**************************************************
 * Copyright 2015 DougHnuts Publishing LLC.
 * All rights Reserved
 **************************************************/

#endregion

using DougHnutsPublishing.MongoRepository;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

// namespaces...
namespace DougHnutsPublishing.MongoRepositoryTests
{
    // public classes...
    public class When_accessing_MongoDB_With_Database
    {

        [Fact]
        public async void An_add_of_a_single_doc_is_Successful()
        {
            var doc = new BsonDocument() { new BsonElement("TestField1", 192) };
            using (var database = new Database<BsonDocument>("DHPTest"))
            {
                //clear all from database
                await database.DeleteMany(new BsonDocument());
                await database.Add(doc);

                var result = await database.All();

                result.Count().Should().Be(1);
                var resultDoc = result.First();
                resultDoc["TestField1"].Should().Be(192);
            }
        }

        [Fact]
        public async void An_add_of_a_collection_Successfully_Adds_All_Documents()
        {
            var docs = new List<BsonDocument>();
            docs.Add(new BsonDocument() { new BsonElement("TestField1", 192) });
            docs.Add(new BsonDocument() { new BsonElement("TestField1", 193) });

            using (var database = new Database<BsonDocument>("DHPTest"))
            {
                //clear all from database
                await database.DeleteMany(new BsonDocument());
                await database.Add(docs);

                var result = await database.All();

                result.Count().Should().Be(2);
                var resultDoc1 = result.First();
                var resultDoc2 = result.Last();
                resultDoc1["TestField1"].Should().Be(192);
                resultDoc2["TestField1"].Should().Be(193);
            }
        }
        [Fact]
        public async void Can_Update_an_existing_item()
        {
            var docs = new List<BsonDocument>();
            docs.Add(new BsonDocument() { new BsonElement("TestField1", 192) });
            docs.Add(new BsonDocument() { new BsonElement("TestField1", 193) });

            using (var database = new Database<BsonDocument>("DHPTest"))
            {
                //clear all from database
                await database.DeleteMany(new BsonDocument());
                await database.Add(docs);

                var filter = Builders<BsonDocument>.Filter.Eq("TestField1", 192);
                var update = Builders<BsonDocument>.Update.Set("TestField1", 110);
                var result = await database.Update(filter, update);

                result.MatchedCount.Should().Be(1);
                result.ModifiedCount.Should().Be(1);

            }
        }

        [Fact]
        public async void Can_Update_Multiple_items()
        {
            var docs = new List<BsonDocument>();
            docs.Add(new BsonDocument() { new BsonElement("TestField1", 192) });
            docs.Add(new BsonDocument() { new BsonElement("TestField1", 193) });
            docs.Add(new BsonDocument() { new BsonElement("TestField1", 192) });

            using (var database = new Database<BsonDocument>("DHPTest"))
            {
                //clear all from database
                await database.DeleteMany(new BsonDocument());
                await database.Add(docs);

                var filter = Builders<BsonDocument>.Filter.Eq("TestField1", 192);
                var update = Builders<BsonDocument>.Update.Set("TestField1", 110);
                var result = await database.UpdateMany(filter, update);

                result.MatchedCount.Should().Be(2);
                result.ModifiedCount.Should().Be(2);

            }
        }
    }
}
