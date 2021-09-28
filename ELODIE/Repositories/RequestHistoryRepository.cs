using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Helpers;
using ELODIE.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Repositories
{
    public interface IRequestHistoryRepository
    {
        Task<long> CountRequestHistory<T>(long RequestId);
        Task<List<RequestHistory<T>>> ListRequestHistory<T>(long RequestId);
        Task<RequestHistory<T>> GetRequestHistory<T>(string Id);
        Task CreateRequestHistory<T>(
            long RequestId, long? AppUserId, 
            T obj, Dictionary<string, object> displayFields) where T : DataEntity;
    }
    public class RequestHistoryRepository : IRequestHistoryRepository
    {
        public static string ConnectionString;
        public static string DatabaseName;
        protected IMongoDatabase MongoDatabase;
        protected DataContext DataContext;
        public RequestHistoryRepository(DataContext DataContext)
        {
            IMongoClient MongoClient = new MongoClient(ConnectionString);
            this.MongoDatabase = MongoClient.GetDatabase(DatabaseName);
            this.DataContext = DataContext;
        }

        public string GetCollectionName<T>()
        {
            string collectionName = $"History_{typeof(T).Name}";
            return collectionName;
        }

        public async Task<long> CountRequestHistory<T>(long RequestId)
        {
            string collectionName = GetCollectionName<T>();
            var collection = MongoDatabase.GetCollection<RequestHistory<T>>(collectionName);

            FilterDefinition<RequestHistory<T>> BuilderFilter = Builders<RequestHistory<T>>.Filter.Empty;
            BuilderFilter &= Builders<RequestHistory<T>>.Filter.Eq("RequestId", RequestId);
            return await collection.CountDocumentsAsync(BuilderFilter);
        }

        public async Task<List<RequestHistory<T>>> ListRequestHistory<T>(long RequestId)
        {
            string collectionName = GetCollectionName<T>();
            var collection = MongoDatabase.GetCollection<RequestHistory<T>>(collectionName);

            FilterDefinition<RequestHistory<T>> BuilderFilter = Builders<RequestHistory<T>>.Filter.Empty;

            BuilderFilter &= Builders<RequestHistory<T>>.Filter.Eq("RequestId", RequestId);

            var fieldsBuilder = Builders<RequestHistory<T>>.Projection;
            var fields = fieldsBuilder.Exclude(d => d.OldContent).Exclude(d => d.Content);

            List<RequestHistory<T>> RequestHistories = await collection
                .Find(BuilderFilter)
                .Project<RequestHistory<T>>(fields)
                .SortByDescending(e => e.SavedAt)
                .ToListAsync();
            return RequestHistories;
        }

        public async Task<RequestHistory<T>> GetRequestHistory<T>(string Id)
        {
            string collectionName = GetCollectionName<T>();
            var collection = MongoDatabase.GetCollection<RequestHistory<T>>(collectionName);

            FilterDefinition<RequestHistory<T>> BuilderFilter = Builders<RequestHistory<T>>.Filter.Empty;
            StringFilter StringFilter = new StringFilter
            {
                Equal = Id
            };
            BuilderFilter = BuilderFilter.MgWhere(x => x.Id, StringFilter);
            RequestHistory<T> RequestHistory = await collection
                .Find(BuilderFilter)
                .FirstOrDefaultAsync();

            return RequestHistory;
        }

        public async Task CreateRequestHistory<T>(
            long RequestId, 
            long? AppUserId,
            T obj, Dictionary<string, object> displayFields) where T : DataEntity
        {
            string collectionName = GetCollectionName<T>();
            var collection = MongoDatabase.GetCollection<RequestHistory<T>>(collectionName);

            FilterDefinition<RequestHistory<T>> BuilderFilter = Builders<RequestHistory<T>>.Filter.Empty;
            BuilderFilter &= Builders<RequestHistory<T>>.Filter.Eq("RequestId", RequestId);

            RequestHistory<T> OldRequestHistory = await collection
                .Find(BuilderFilter)
                .SortByDescending(e => e.SavedAt)
                .FirstOrDefaultAsync();


            AppUser AppUser = null;
            if (AppUserId != null)
            {
                AppUser = await DataContext.AppUser
               .Where(x => x.Id == AppUserId)
               .Select(x => new AppUser
               {
                   Id = x.Id,
                   Username = x.Username,
                   DisplayName = x.DisplayName,
               }).FirstOrDefaultAsync();
            }
            RequestHistory<T> NewRequestHistory = new RequestHistory<T>
            {
                AppUserId = AppUserId,
                AppUser = AppUser,
                Content = obj,
                OldContent = OldRequestHistory?.Content,
                SavedAt = StaticParams.DateTimeNow,
                DisplayFields = displayFields,
                RequestId = RequestId,
            };
            await collection.InsertOneAsync(NewRequestHistory);
        }
    }
}
