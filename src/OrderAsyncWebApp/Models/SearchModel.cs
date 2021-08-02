using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAsyncWebApp.Models
{
    public class SearchModel
    {
        public string ShopId { get; set; }

        public string MerchantId { get; set; }

        public string PPId { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }
    }
}
