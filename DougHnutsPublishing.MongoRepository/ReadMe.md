# DougHnutsPublishing MONGODB REPOSITORY  

## Quick start

Add Connection String to Web.config or App.Config 


    <connectionStrings>
    <add name="dhpdb" connectionString="mongodb://localhost:27017/dhpdb"/>
    </connectionStrings>
    


Add Namespace

    using DougHnutsPublishing.MongoRepository;


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

####Add multiple documents to collection

            using (var database = new Database<BsonDocument>("DHPTest"))
            {
                var filter = Builders<BsonDocument>.Filter.Eq("TestField1", 192);
                var update = Builders<BsonDocument>.Update.Set("TestField1", 110);
                var result = await database.Update(filter, update);
            }

