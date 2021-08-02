using MongoDB.Driver;
using OrderAsyncWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAsyncWebApp.Services
{
    public class Services
    {
        public IList<PlatformOrder> GetOrderList(SearchModel searchModel)
        {
            var client = new MongoClient("mongodb://127.0.0.1:27017");
            var database = client.GetDatabase("PlatformOrder");
            var _collection = database.GetCollection<PlatformOrder>("PlatformOrder");
            FilterDefinition<PlatformOrder> filter = Builders<PlatformOrder>.Filter.Empty;
            SortDefinition<PlatformOrder> sorter = Builders<PlatformOrder>.Sort.Descending("_id");
            if (string.IsNullOrEmpty(searchModel.ShopId) == false)
            {
                filter = filter & Builders<PlatformOrder>.Filter.Eq(p => p.ShopId, searchModel.ShopId);
            }
            if (string.IsNullOrEmpty(searchModel.MerchantId) == false)
            {
                filter = filter & Builders<PlatformOrder>.Filter.Eq(p => p.MerchantId, searchModel.MerchantId);
            }
            if (string.IsNullOrEmpty(searchModel.PPId) == false)
            {
                filter = filter & Builders<PlatformOrder>.Filter.Eq(p => p.PPId, searchModel.PPId);
            }
            if (string.IsNullOrEmpty(searchModel.StartTime) == false)
            {
                var StartTime = Convert.ToDateTime(searchModel.StartTime).Date;
                filter = filter & Builders<PlatformOrder>.Filter.Gte(p => p.Created_at, StartTime);
            }
            if (string.IsNullOrEmpty(searchModel.EndTime) == false)
            {
                var EndTime = Convert.ToDateTime(searchModel.EndTime).AddDays(1).Date;
                filter = filter & Builders<PlatformOrder>.Filter.Lte(p => p.Created_at, EndTime);
            }
            var res = _collection.Find(filter).Sort(sorter).ToList();

            return res;
        }
    }
}
