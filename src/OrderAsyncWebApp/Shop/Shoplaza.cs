using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using OrderAsyncWebApp.Models;
using OrderAsyncWebApp.Services;
using OrderSync.Task;
using RestSharp;

namespace OrderSync.Shop
{

    public class Shoplaza : IShop
    {
        public override IList<orders> GetAll(PlatformShop shop)
        {
            Dictionary<string, orders> orderDic = new Dictionary<string, orders>();
            try
            {
                var next = "";
                var endtime = DateTime.Now;
                int page = 1;
                //店铺读取订单
                for (; ; )
                {
                    var syncTime = TimeMin(shop.StartSyncTime);

                    var list = GetList(shop.ShopId, shop.ApiKey, shop.Domain, shop.StartSyncTime, endtime, page++);
                    foreach (var info in list)
                    {
                        //info.total_price_usd = info.total_price / Program.RatesDic[info.currency];
                        info.email = info.shipping_address.email;
                        info.name = info.number;
                        info.order_status_url = info.landing_site;
                        info.phone = info.shipping_address.phone;
                        if (info.payment_line == null)
                        {
                            info.gateway = "unknow";

                        }
                        else
                        {
                            info.gateway = info.payment_line.payment_channel;
                        }

                        if (info.shipping_line == null)
                        {
                            info.shipping_lines = new List<shippinglines>() { new shippinglines(){ title="shop"}};
                        }
                        else
                        {
                            info.shipping_lines = new List<shippinglines>() {new shippinglines(){ title=info.shipping_line.name}};
                        }


                        if (info.processed_at == null)
                        {
                            info.processed_at = Convert.ToDateTime(info.created_at);
                        }
                        //info.updated_at = Convert.ToDateTime(info.created_at);
                        if (orderDic.ContainsKey(info.id) == false)
                        {
                            foreach (var item in info.line_items)
                            {
                                item.title = item.product_title ?? item.sku;
                                item.imageurl = "https:" + item.imageurl;
                                item.product_url = "https://" + shop.Domain + item.product_url;
                            }
                            orderDic.Add(info.id, info);
                        }
                    }
                    var curr = string.Join("|", list.Select(OBJ => OBJ.id).ToList());
                    if (list.Count == 0 || curr == next)
                    {
                        break;
                    }
                    next = curr;
                }
                ShopService shopService = new ShopService();
                shopService.DeleteErrorMsg(shop.ShopId);
                return orderDic.Values.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(shop.ShopId + " 店匠"+ "下载订单失败:"+ex.Message);
            }
            return new List<orders>();
        }

        private IList<orders> GetList(string shopid, string token, string url, DateTime created_at_min, DateTime created_at_max, int page = 1)
        {
            string responseText = "";
            Encoding encoding = Encoding.UTF8;
            ShopService shopService = new ShopService();
            created_at_min = TimeMin(created_at_min, shopid);

            RestSharp.RestClient client = new RestSharp.RestClient($"https://{url}/");
            var request = new RestSharp.RestRequest("openapi/2020-01/orders?financial_status=paid&page=" + page + "&placed_at_min=" + created_at_min.ToString("s") + "&placed_at_max=" + created_at_max.ToString("s"), Method.GET);

            try
            {
                request.AddHeader("Access-Token", token);
                var tt = client.Execute<string>(request);
                var res = client.Execute<ShopifyOrder>(request);
                if (res.StatusCode.GetHashCode() == 200)
                {
                    return res.Data.orders;
                }
                else
                {

                    var msg = $"{shopid};错误信息:店铺" + res.Content;
                    ErrorMsg errorMsg = new ErrorMsg()
                    {
                        ShopId = shopid,
                        Msg = msg
                    };
                    shopService.AddErrorMsg(errorMsg);
                    Console.WriteLine(shopid + "下载订单失败"+ res.Content);
                    return null;
                }
            }
            catch (Exception ex)
            {
               
                var msg = $"{shopid};错误信息:店铺" + ex.Message;
                ErrorMsg errorMsg = new ErrorMsg()
                {
                    ShopId = shopid,
                    Msg = msg
                };
                shopService.AddErrorMsg(errorMsg);
                Console.WriteLine(msg);


                Console.WriteLine(shopid + responseText+ "下载订单失败:"+ex.Message);
                return null;
            }
        }

    

        public class Getfulfillments
        {
            public IList<GetfulfillmentsList> fulfillments { get; set; }
        }

        public class GetfulfillmentsList
        {
            public String id { get; set; }
        }

        public class fulfillments
        {

            public String tracking_number { get; set; }

            public List<String> line_item_ids { get; set; }
        }
        public class ordersOne
        {
            public ordersOneInfo order { get; set; }
        }

        public class ordersOneInfo
        {
            public IList<line_items> line_items { get; set; }
        }


        public class line_items
        {
            public string id { get; set; }
        }



    }
}
