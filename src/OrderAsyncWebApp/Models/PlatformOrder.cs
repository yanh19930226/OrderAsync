using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAsyncWebApp.Models
{
    public class PlatformOrder
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public String ShopId { get; set; } = "";
        public String MerchantId { get; set; } 
        public String PPId { get; set; }
        public String ExternalOrderId { get; set; } = "";
        public String Email { get; set; } = "";
        public String Phone { get; set; } = "";
        public decimal TotalPriceUSD { get; set; } = decimal.MinValue;
        public decimal TotalPrice { get; set; } = decimal.MinValue;
        public String Currency { get; set; } = "";
        public String ExternalName { get; set; } = "";
        public String ShopifLogisticsName { get; set; } = "";
        public String ShopifLogisticsId { get; set; } = "";
        public String Ip { get; set; } = "";
        public String OrderStatusUrl { get; set; } = "";
        public String Gateway { get; set; } = "";
        public String CheckoutId { get; set; } = "";
        public String Note { get; set; } = "";
        public String Country { get; set; } = "";
        public String State { get; set; } = "";
        public String City { get; set; } = "";
        public String LastName { get; set; } = "";
        public String FirstName { get; set; } = "";
        public String Address1 { get; set; } = "";
        public String Address2 { get; set; } = "";
        public String Zip { get; set; } = "";
        public decimal ShopifyFee { get; set; } = decimal.MinValue;
        public decimal ExchangeRate { get; set; } = decimal.MinValue;
        public decimal ExchangeRateRMB { get; set; } = decimal.MinValue;
        public String CartToken { get; set; } = "";
        public String ClientDetails { get; set; } = "";
        public DateTime Created_at { get; set; } = DateTime.MinValue;
        public DateTime Updated_at { get; set; } = DateTime.MinValue;
        public DateTime Processed_at { get; set; } = DateTime.MinValue;
        public DateTime CreatedTime { get; set; } = DateTime.MinValue;
        public DateTime UpdatedTime { get; set; } = DateTime.MinValue;
    }
}
