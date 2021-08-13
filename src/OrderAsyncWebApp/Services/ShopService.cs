using MongoDB.Driver;
using OrderAsyncWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAsyncWebApp.Services
{
    public class ShopService
    {
        private MongoClient mongoClient;
        public ShopService()
        {
            mongoClient = new MongoClient("mongodb://127.0.0.1:27017");
        }

        public PlatformShop GetShop(string shopId)
        {
            var database = mongoClient.GetDatabase("PlatformOrder");
            var _collection = database.GetCollection<PlatformShop>("PlatformShop");
            FilterDefinition<PlatformShop> filter = Builders<PlatformShop>.Filter.Empty;
            filter = filter & Builders<PlatformShop>.Filter.Eq(p => p.ShopId, shopId);
            var res=_collection.Find(filter).FirstOrDefault();
            return res;
        }
        public bool EditShop(PlatformShop platformShop)
        {
            var database = mongoClient.GetDatabase("PlatformOrder");
            var _collection = database.GetCollection<PlatformShop>("PlatformShop");
            var filter = Builders<PlatformShop>.Filter;
            var options = new FindOneAndUpdateOptions<PlatformShop, PlatformShop>() { IsUpsert = true };
            var update = Builders<PlatformShop>.Update.
                                Set(p => p.IsOpen, platformShop.IsOpen).
                                Set(p => p.MerchantId, platformShop.MerchantId).
                                Set(p => p.PPId, platformShop.PPId).
                                Set(p => p.ShopId, platformShop.ShopId).
                                Set(p => p.Title, platformShop.Title).
                                 Set(p => p.AdminUrl, platformShop.AdminUrl).
                                Set(p => p.ApiKey, platformShop.ApiKey).
                                Set(p => p.Domain, platformShop.Domain).
                                 Set(p => p.StartSyncTime, DateTime.SpecifyKind(platformShop.StartSyncTime, DateTimeKind.Utc)).
                                Set(p => p.Types, platformShop.Types);
            var res = _collection.FindOneAndUpdate(filter.Eq(p => p.ShopId, platformShop.ShopId), update, options);

            return true;
        }
        public bool AddShop(PlatformShop platformShop)
        {
            platformShop.StartSyncTime=DateTime.SpecifyKind(platformShop.StartSyncTime, DateTimeKind.Utc);
            var database = mongoClient.GetDatabase("PlatformOrder");
            var _collection = database.GetCollection<PlatformShop>("PlatformShop");
            _collection.InsertOne(platformShop);
            return true;
        }
        public List<PlatformShop>GetShopList(ShopSearch shopSearch)
        {
            var database = mongoClient.GetDatabase("PlatformOrder");
            var _collection = database.GetCollection<PlatformShop>("PlatformShop");
            FilterDefinition<PlatformShop> filter = Builders<PlatformShop>.Filter.Empty;
            SortDefinition<PlatformShop> sorter = Builders<PlatformShop>.Sort.Descending("_id");
            if (string.IsNullOrEmpty(shopSearch.ShopId) == false)
            {
                filter = filter & (Builders<PlatformShop>.Filter.Eq(p => p.ShopId, shopSearch.ShopId)| Builders<PlatformShop>.Filter.Eq(p => p.Domain, shopSearch.ShopId));
            }
            var res = _collection.Find(filter).Sort(sorter).ToList();

            return res;
        }

        public List<ErrorMsg> GetErrorMsgList()
        {
            var database = mongoClient.GetDatabase("PlatformOrder");
            var _collection = database.GetCollection<ErrorMsg>("ErrorMsg");
            FilterDefinition<ErrorMsg> filter = Builders<ErrorMsg>.Filter.Empty;
            SortDefinition<ErrorMsg> sorter = Builders<ErrorMsg>.Sort.Descending("_id");
        
            var res = _collection.Find(filter).Sort(sorter).ToList();

            return res;
        }

        public bool AddErrorMsg(ErrorMsg msg)
        {
            var database = mongoClient.GetDatabase("PlatformOrder");
            var _collection = database.GetCollection<ErrorMsg>("ErrorMsg");
            var filter = Builders<ErrorMsg>.Filter;
            var options = new FindOneAndUpdateOptions<ErrorMsg, ErrorMsg>() { IsUpsert = true };
            var update = Builders<ErrorMsg>.Update.
                            Set(p => p.ShopId, msg.ShopId).
                            Set(p => p.Msg, msg.Msg);
                            
            //平台订单号区分
            var res = _collection.FindOneAndUpdate(filter.Eq(p => p.ShopId,msg.ShopId), update, options);

            return true;
        }

        public bool DeleteErrorMsg(string shopId)
        {
            var database = mongoClient.GetDatabase("PlatformOrder");
            var _collection = database.GetCollection<ErrorMsg>("ErrorMsg");
            _collection.DeleteOne(p => p.ShopId == shopId);
            return true;
        }
    }
}
