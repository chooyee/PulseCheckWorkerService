using Cryptolib2;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace Factory.DB
{
    public class MongoDbService : IDisposable
    {
        protected IMongoDatabase db;
        protected MongoClient client;
        public string ConnectionString { get; set; }
        public string DefaultDB { get; set; }

        public MongoDbService()
        { 
        }

        public MongoDbService(string connectionString, string defaultDB)
        {
            ConnectionString = connectionString;
            DefaultDB = defaultDB;
        }

        public async Task CreateIndexAsync<T>(string collectionName, string indexName)
        {
            var collection = db.GetCollection<T>(collectionName);
            var indexOptions = new CreateIndexOptions();
            var indexKeys = Builders<T>.IndexKeys.Ascending(indexName);
            var indexModel = new CreateIndexModel<T>(indexKeys, indexOptions);
            _ = await collection.Indexes.CreateOneAsync(indexModel).ConfigureAwait(false);
        }

        public void CreateConnection()
        {
            CreateConnection(ConnectionString, DefaultDB, true);
        }
        //public void CreateConnection(string defaultConnection = "mgo", bool encrypted = true)
        //{
        //    CreateConnection(defaultConnection, ConfigurationManager.AppSettings["default_db"].ToString(), encrypted);
        //}

        public void CreateConnection(string connection, string database, bool encrypted)
        {
            if (encrypted)
                this.client = new MongoClient(Cryptolib2.Crypto.DecryptText(connection, false));
            else
                this.client = new MongoClient(connection);
            this.db = client.GetDatabase(database);
        }

        public List<string> GetCollectionNameAsync()
        {
            if (db == null)
                CreateConnection();

            List<string> results = new List<string>();

            foreach (BsonDocument collection in this.db.ListCollectionsAsync().Result.ToListAsync<BsonDocument>().Result)
            {
                results.Add(collection["name"].AsString);

            }
            return results;
        }

        public bool CollectionExists(string collectionName)
        {
            if (db == null)
                CreateConnection();

            var filter = new BsonDocument("name", collectionName);
            var options = new ListCollectionNamesOptions { Filter = filter };

            return db.ListCollectionNames(options).Any();
        }

        public Boolean InsertOne<T>(string collectionName, T document)
        {
            if (db == null)
                CreateConnection();

            try
            {
                var collection = db.GetCollection<T>(collectionName);
                collection.InsertOne(document);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public Boolean InsertMany<T>(string collectionName, List<T> documents)
        {
            if (db == null)
                CreateConnection();

            try
            {
                var collection = db.GetCollection<T>(collectionName);
                collection.InsertMany(documents);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public Boolean ReplaceOne<T>(string collectionName, BsonDocument filter, T update, bool upsert = true)
        {
            if (db == null)
                CreateConnection();

            try
            {
                var options = new UpdateOptions { IsUpsert = upsert };
                var collection = db.GetCollection<T>(collectionName);
                collection.ReplaceOne(filter, update, options);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }


        }

        public Boolean ReplaceOne<T>(string collectionName, FilterDefinition<T> filter, T update, bool upsert = true)
        {
            if (db == null)
                CreateConnection();

            try
            {
                var options = new UpdateOptions { IsUpsert = upsert };
                var collection = db.GetCollection<T>(collectionName);
                collection.ReplaceOne(filter, update, options);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }


        }

        /// <summary>
        /// Update the data
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="filter"></param>
        /// <param name="update"></param>
        /// <param name="upsert"></param>
        /// <returns></returns>
        public Boolean UpdateOne(string collectionName, BsonDocument filter, BsonDocument update, bool upsert = true)
        {
            if (db == null)
                CreateConnection();

            try
            {
                var options = new UpdateOptions { IsUpsert = upsert };
                var collection = db.GetCollection<BsonDocument>(collectionName);
                collection.UpdateOne(filter, update, options);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }


        }

        public T FindOneAndUpdate<T>(string collectionName, FilterDefinition<T> filter, UpdateDefinition<T> update, bool upsert = true)
        {
            if (db == null)
                CreateConnection();

            try
            {
                var options = new FindOneAndUpdateOptions<T> { IsUpsert = upsert };
                var collection = db.GetCollection<T>(collectionName);
                var result = collection.FindOneAndUpdate(
                               filter,
                               update, options
                               );
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public BsonDocument FindOneAndUpdate(string collectionName, FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> update, bool upsert = true)
        {
            if (db == null)
                CreateConnection();

            try
            {
                var options = new FindOneAndUpdateOptions<BsonDocument> { IsUpsert = upsert };
                var collection = db.GetCollection<BsonDocument>(collectionName);
                var result = collection.FindOneAndUpdate(
                               filter,
                               update, options
                               );
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<BsonDocument> FindOneAndUpdateAsync(string collectionName, FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> update, bool upsert = true)
        {
            if (db == null)
                CreateConnection();

            try
            {
                var options = new FindOneAndUpdateOptions<BsonDocument> { IsUpsert = upsert };
                var collection = db.GetCollection<BsonDocument>(collectionName);
                var result = await collection.FindOneAndUpdateAsync(
                               filter,
                               update,
                               options
                               ).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<UpdateResult> UpdateOneAsync(string collectionName, BsonDocument filter, BsonDocument update, bool upsert = true)
        {
            if (db == null)
                CreateConnection();

            try
            {
                var options = new UpdateOptions { IsUpsert = upsert };
                var collection = db.GetCollection<BsonDocument>(collectionName);
                UpdateResult x = await collection.UpdateOneAsync(filter, update, options);
                return x;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }


        }

        public async Task<ReplaceOneResult> ReplaceOneAsync<T>(string collectionName, BsonDocument filter, T update, bool upsert = true)
        {
            if (db == null)
                CreateConnection();

            try
            {
                var options = new UpdateOptions { IsUpsert = upsert };
                var collection = db.GetCollection<T>(collectionName);
                ReplaceOneResult x = await collection.ReplaceOneAsync(filter, update, options);
                return x;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }


        }

        public async Task<T> InsertOneAsync<T>(string collectionName, T document)
        {
            if (db == null)
                CreateConnection();

            try
            {
                var collection = db.GetCollection<T>(collectionName);
                await collection.InsertOneAsync(document);
                return document;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<Boolean> InsertManyAsync<T>(string collectionName, List<T> documents)
        {
            if (db == null)
                CreateConnection();

            try
            {
                var collection = db.GetCollection<T>(collectionName);
                await collection.InsertManyAsync(documents);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<Boolean> InsertOneAsync(string collectionName, BsonDocument document)
        {
            if (db == null)
                CreateConnection();

            try
            {
                var collection = db.GetCollection<BsonDocument>(collectionName);
                await collection.InsertOneAsync(document);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<Boolean> InsertManyAsync(string collectionName, List<BsonDocument> documents)
        {
            if (db == null)
                CreateConnection();

            try
            {
                var collection = db.GetCollection<BsonDocument>(collectionName);
                await collection.InsertManyAsync(documents);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public List<T> Find<T>(string collectionName, FilterDefinition<T> filter, SortDefinition<T> sort = null)
        {
            if (db == null)
                CreateConnection();

            var collection = db.GetCollection<T>(collectionName);

            if (sort !=null)
                return collection.Find<T>(filter).Sort(sort).ToList<T>();
            else
                return collection.Find<T>(filter).ToList<T>();
        }

        public List<T> Find<T>(string collectionName, FilterDefinition<T> filter, SortDefinition<T> sort, int skip, int limit)
        {
            if (db == null)
                CreateConnection();

            var collection = db.GetCollection<T>(collectionName);

            return collection.Find<T>(filter).Sort(sort).Skip(skip).Limit(limit).ToList<T>();

        }

        public List<BsonDocument> Find(string collectionName, FilterDefinition<BsonDocument> filter)
        {
            if (db == null)
                CreateConnection();

            var collection = db.GetCollection<BsonDocument>(collectionName);

            var list = collection.Find(filter).ToList();

            return list;
        }

        public List<BsonDocument> Find(string collectionName, BsonDocument filter)
        {
            if (db == null)
                CreateConnection();

            var collection = db.GetCollection<BsonDocument>(collectionName);

            var list = collection.Find(filter).ToList();

            return list;
        }
        public async ValueTask<List<T>> FindAsync<T>(string collectionName, FilterDefinition<T> filter)
        {
            if (db == null)
                CreateConnection();

            try
            {
                var collection = db.GetCollection<T>(collectionName);
                var result = await collection.FindAsync<T>(filter);
                return result.ToList<T>();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async ValueTask<List<T>> FindAsync<T>(string collectionName, FilterDefinition<T> filter, SortDefinition<T> sortDefinition)
        {
            if (db == null)
                CreateConnection();

            try
            {
                var collection = db.GetCollection<T>(collectionName);
                var result = await collection.FindAsync<T>(filter, new FindOptions<T>() { Sort=sortDefinition });
                return result.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<BsonDocument>> FindAsync(string collectionName, BsonDocument filter)
        {
            if (db == null)
                CreateConnection();

            var collection = db.GetCollection<BsonDocument>(collectionName);

            var list = await collection.Find(filter).ToListAsync();

            return list;
        }

        public async Task<List<T>> FindAsync<T>(string collectionName, BsonDocument filter)
        {
            if (db == null)
                CreateConnection();

            var collection = db.GetCollection<T>(collectionName);
            List<T> list = await collection.Find<T>(filter).ToListAsync<T>();

            return list;
        }

        public List<T> Find<T>(string collectionName, BsonDocument filter)
        {
            if (db == null)
                CreateConnection();

            var collection = db.GetCollection<T>(collectionName);
            List<T> list = collection.Find<T>(filter).ToList<T>();

            return list;
        }

        public bool DeleteOne<T>(string collectionName, FilterDefinition<T> filter)
        {
            if (db == null)
                CreateConnection();

            try
            {
                var collection = db.GetCollection<T>(collectionName);
                var result = collection.DeleteOne(filter);
                if (result.IsAcknowledged)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public bool DeleteMany<T>(string collectionName, FilterDefinition<T> filter)
        {
            if (db == null)
                CreateConnection();

            try
            {
                var collection = db.GetCollection<T>(collectionName);
                var result = collection.DeleteMany(filter);
                if (result.IsAcknowledged)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public Boolean Drop(string collectionName)
        {
            if (db == null)
                CreateConnection();

            try
            {
                db.DropCollection(collectionName);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<bool> DropAsync(string collectionName)
        {
            if (db == null)
                CreateConnection();

            try
            {
                await db.DropCollectionAsync(collectionName);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public List<T> Aggregate<T>(string collectionName, BsonDocument groupBy)
        {
            if (db == null)
                CreateConnection();

            var collection = db.GetCollection<T>(collectionName);
            var aggregate = collection.Aggregate()
                                       .Group<T>(groupBy);
            return aggregate.ToList();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).

                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MongoHelper() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
