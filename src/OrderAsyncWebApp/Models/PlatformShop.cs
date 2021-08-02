using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAsyncWebApp.Models
{
    public class PlatformShop
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        /// <summary>
        /// ShopId
        /// </summary>
        public String ShopId { get; set; } = "";
        /// <summary>
        /// 标题
        /// </summary>
        public String Title { get; set; } = "";

        /// <summary>
        /// 标题
        /// </summary>
        public String MerchantId { get; set; } = "";
        /// <summary>
        /// 标题
        /// </summary>
        public String PPId { get; set; } = "";
        /// <summary>
        /// 密钥
        /// </summary>
        public String ApiKey { get; set; } = "";
        /// <summary>
        /// 是否启用
        /// </summary>
        public int IsOpen { get; set; } = -1;
        /// <summary>
        /// 类型
        /// </summary>
        public int Types { get; set; } = -1;
        public String AdminUrl { get; set; } = "";
        public String Domain { get; set; } = "";
    }
}
