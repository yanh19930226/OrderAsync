using MediatR;
using MongoDB.Driver;
using OrderAsyncWebApp.Models;
using OrderSync.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrderAsyncWebApp
{
    public class WriteOrderCommand : IRequest<bool>
    {
        public WriteOrderCommand(List<orders> orderList, string shopId,string merchantId, string ppId)
        {
            OrderList = orderList;
            ShopId = shopId;
            MerchantId = merchantId;
            PPId = ppId;
        }
        public List<orders> OrderList { get; set; } = new List<orders>();
        public string ShopId { get; set; }

        public string MerchantId { get; set; }

        public string PPId { get; set; }
    }
    public class WriteOrderCommandHandler : IRequestHandler<WriteOrderCommand, bool>
    {
        public Task<bool> Handle(WriteOrderCommand request, CancellationToken cancellationToken)
        {
            var client = new MongoClient("mongodb://127.0.0.1:27017");
            var database = client.GetDatabase("PlatformOrder");
            var _collection = database.GetCollection<PlatformOrder>("PlatformOrder");
            foreach (var info in request.OrderList)
            {
                var filter = Builders<PlatformOrder>.Filter;
                var options = new FindOneAndUpdateOptions<PlatformOrder, PlatformOrder>() { IsUpsert = true };
                var update = Builders<PlatformOrder>.Update.
                                Set(p => p.ExternalOrderId, info.id).
                                Set(p => p.ShopId, request.ShopId).
                                Set(p => p.MerchantId, request.MerchantId).
                                Set(p => p.PPId, request.PPId).
                                Set(p => p.Address1, info.shipping_address.address1).
                                Set(p => p.Address2, info.shipping_address.address2 ?? "").
                                Set(p => p.CartToken, info.cart_token ?? "").
                                Set(p => p.CheckoutId,  info?.payment_line?.transaction_no?? info.checkout_id ?? "").
                                Set(p => p.City, info.shipping_address.city).
                                Set(p => p.ClientDetails, Newtonsoft.Json.JsonConvert.SerializeObject(info.client_details)).
                                Set(p => p.Country, info.shipping_address.country_code).
                                Set(p => p.Currency, info.currency).
                                Set(p => p.Email, info.email ?? "guest").
                                Set(p => p.ExternalName, info.name).
                                Set(p => p.Gateway, info.gateway).
                                Set(p => p.FirstName, info.shipping_address.first_name).
                                Set(p => p.Ip, info.browser_ip).
                                Set(p => p.LastName, info.shipping_address.last_name).
                                Set(p => p.Note, info.note).
                                Set(p => p.OrderStatusUrl, info.order_status_url).
                                Set(p => p.Phone, info.shipping_address.phone).
                                Set(p => p.State, info.shipping_address.province_code ?? info.shipping_address.province).
                                Set(p => p.TotalPrice, info.total_price).
                                Set(p => p.TotalPriceUSD, info.total_price_usd).
                                Set(p => p.Zip, info.shipping_address.zip).
                                Set(p => p.FirstName, info.shipping_address.first_name).
                                Set(p => p.LastName, info.shipping_address.last_name).
                                Set(p => p.ShopifLogisticsName, info.shipping_lines.Count == 0 ? "FREE" : info.shipping_lines[0].title).
                                Set(p => p.ShopifLogisticsId, info.shipping_lines.Count == 0 ? "FREE" : info.shipping_lines[0].id).
                                Set(p => p.ShopifyFee, info.total_price_usd * 0.02m).
                                Set(p => p.Processed_at, Convert.ToDateTime(info.processed_at)).
                                Set(p => p.Updated_at, Convert.ToDateTime(info.updated_at)).
                                Set(p => p.Created_at, Convert.ToDateTime(info.created_at)).
                                Set(p => p.CreatedTime, DateTime.Now).
                                Set(p => p.UpdatedTime, DateTime.Now);
                                //平台订单号区分
                var res = _collection.FindOneAndUpdate(filter.Eq(p => p.ShopId, request.ShopId) & filter.Eq(p => p.ExternalOrderId, info.id), update, options);
            }
            return Task.FromResult(true);
        }
    }
}
