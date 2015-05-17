# DougHnutsPublishing.MongoRepository
Repository class for MongoDB API 2.0

Classes to make working with the MongoDB 2.0 API easier.

## Quick start

Add Connection String to Web.config or App.Config 


    <connectionStrings>
    <add name="dhpdb" connectionString="mongodb://localhost:27017/dhpdb"/>
    </connectionStrings>
    


Add Namespace

    using DougHnutsPublishing.MongoRepository;

Main processing is handled in the Database class.   Essential use is:

      using (var database =  new Database<Type>("CollectionName"))
      {
      }


##Using MongoRepository

# Code Examples #
####Add a single document to collection
    
            using (var database = new Database<BsonDocument>("DHPTest"))
            {
                await database.Add(doc);
            }


####Add multiple documents to collection
    
            using (var database = new Database<BsonDocument>("DHPTest"))
            {
                await database.Add(docs);
            }

####Update a document in the collection

            using (var database = new Database<BsonDocument>("DHPTest"))
            {
                var filter = Builders<BsonDocument>.Filter.Eq("TestField1", 192);
                var update = Builders<BsonDocument>.Update.Set("TestField1", 110);
                var result = await database.Update(filter, update);
            }

####Delete a specific document from a collection

            
            //database.Delete will only delete a single item  
            using (var database = new Database<BsonDocument>("DHPTest"))
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", doc["_id"]);
                await database.Delete(filter);
            }

             
            //This code will delete a single item (assumption is _id is unique for each document)
            using (var database = new Database<BsonDocument>("DHPTest"))
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", doc["_id"]);
                await database.DeleteMany(filter);
            }

####Delete multiple documents from a collection
            using (var database = new Database<BsonDocument>("DHPTest"))
            {
                var filter = Builders<BsonDocument>.Filter.Gt("TestField1", 191);
                await database.DeleteMany(filter);
            }

####Delete all documents from a collection
            using (var database = new Database<BsonDocument>("DHPTest"))
            {
                await database.DeleteMany(new BsonDocument());
            }
# More Code Examples to come... #
