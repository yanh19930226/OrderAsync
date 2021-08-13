using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        public String ShopId { get; set; } = "";
        /// <summary>
        /// 标题
        /// </summary>
        [Required]
        public String Title { get; set; } = "";

        /// <summary>
        /// 标题
        /// </summary>
        [Required]
        public String MerchantId { get; set; } = "";
        /// <summary>
        /// 标题
        /// </summary>
        [Required]
        public String PPId { get; set; } = "";
        /// <summary>
        /// 密钥
        /// </summary>
        [Required]
        public String ApiKey { get; set; } = "";
        /// <summary>
        /// 是否启用
        /// </summary>
        public int IsOpen { get; set; } = -1;
        /// <summary>
        /// 类型
        /// </summary>
        public int Types { get; set; } = -1;
        [Required]
        public String AdminUrl { get; set; } = "";
        public String Domain { get; set; } = "111";
        /// <summary>
        /// 开始同步时间
        /// </summary>
        [Required]
        public DateTime StartSyncTime { get; set; }
    }
}
