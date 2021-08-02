using OrderAsyncWebApp.Models;
using OrderSync.Task;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;


namespace OrderSync.Shop
{
    public class Funpinpin : IShop
    {
        public class AddTrackingFpp {
            public string order_id { get; set; } = "";

            public string tracking_provider { get; set; } = "EMS";
            public string tracking_number { get; set; } = "";
            public string erp_name { get; set; } = "fpp";

        }


        public override IList<orders> GetAll(PlatformShop shop)
        {
            var endtime = DateTime.Now;
            int page = 1;
            Dictionary<string, orders> orderDic = new Dictionary<string, orders>();
            try
            {
                //店铺读取订单
                for (; ; )
                {
                    var list = ToOrder( GetList(shop.ShopId, shop.Domain,DateTime.Now.AddDays(-30), endtime, shop.ApiKey, page++), shop);
                    foreach (var info in list)
                    {
                        if (info.id == "3942"|| info.id == "3942")
                        {
                            var dd = 0;
                        }
                        orderDic.Add(info.id, info);
                    }
                    if (list.Count == 0)
                    {
                        break;
                    }
                }
                return orderDic.Values.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(shop.ShopId + " 店匠" + "下载订单失败:" + ex.Message);
            }
            return new List<orders>();
        }

        Dictionary<string, FunpinpinProduct> ProductDic = new Dictionary<string, FunpinpinProduct>();


        private IList<orders> ToOrder(IList<FunpinpinOrders> list, PlatformShop shop)
        {
            IList<orders> orderslist = new List<orders>();
         
            foreach (var order in list)
            {
                if (String.IsNullOrEmpty(order.date_paid))
                {
                    continue;
                }

                orders info = new orders()
                {
                    browser_ip = "",
                    cart_token = "",
                    checkout_id = order.transaction_id,
                    client_details = null,
                    created_at = order.date_created,
                    currency = order.currency,
                    transaction_id = order.transaction_id,
                    customer_locale = "",
                    email = order.billing.email,
                    financial_status = order.status,
                    fulfillment_status = order.status,
                    gateway = order.payment_method_title,
                    id = order.id,
                    landing_site = "",
                    location_id = "",
                    name = order.id,
                    note = "",
                    number = order.number,
                    order_status_url = "",
                    phone = order.billing.phone,
                    processed_at = Convert.ToDateTime(order.date_paid),
                    updated_at = (order.date_paid),
                    token = order.id,
                    total_price = order.total,
                    //total_price_usd = order.total / Program.RatesDic[order.currency],
                    subtotal_price = order.total,
                    tags = "",
                    shipping_lines = new List<shippinglines>() { new shippinglines() { title= order.payment_method_title } },
                    shipping_address = new shippingaddress()
                    {
                        address1 = order.billing.address_1,
                        name = order.billing.first_name + order.billing.last_name,
                        first_name = order.billing.first_name,
                        last_name = order.billing.last_name,
                        address2 = "",
                        city = order.billing.city,
                        company = order.billing.company,
                        country = order.billing.country,
                        country_code = order.billing.country,
                        email = order.billing.email,
                        latitude = "",
                        longitude = "",
                        phone = order.billing.phone,
                        province = order.billing.state,
                        province_code = order.billing.state,
                        zip = order.billing.postcode
                    }
                };
                info.line_items = new List<lineitems>();
                foreach (var line in order.line_items)
                {
                    FunpinpinProduct p = null;
                    if (ProductDic.ContainsKey(line.product_id))
                    {
                        p= ProductDic[line.product_id];
                    }
                    else
                    {
                        p = GetProduct(shop.Domain, line.product_id, shop.ApiKey);
                        ProductDic.Add(line.product_id, p);
                    }
                    var temp = new lineitems()
                    {
                        imageurl = p == null ? "" : ((p.images == null || p.images.Count == 0) ? "" : p.images[0].src),
                        price = line.total,
                        product_id = line.product_id,
                        product_title = line.name,
                        product_url = p == null ? "" : (p.permalink ?? ""),
                        quantity = line.quantity,
                        sku = line.sku,
                        title = line.name,
                        url = p == null ? "" : (p.permalink ?? ""),
                        variant_id = line.variation_id.ToString(),
                        variant_title = line.name

                    };
                    info.line_items.Add(temp);
                }
                orderslist.Add(info);
            }
            return orderslist;
        }

        private IList<FunpinpinOrders> GetList(string shopId, string url, DateTime created_at_min, DateTime endtime, string key, int page = 1)
        {

            created_at_min = TimeMin(Convert.ToDateTime(created_at_min), shopId).AddDays(-3);

            RestSharp.RestClient client = new RestSharp.RestClient($"https://{url}/");
            var keys = key.Split(":");
            try
            {
                var request = new RestSharp.RestRequest($"wp-json/wc/v3/orders?per_page=10&page={page}&consumer_key={keys[0]}&consumer_secret={keys[1]}&status=processing&after={created_at_min.ToString("s")}&before={endtime.ToString("s")}", Method.GET);
                var data = client.Execute<List<FunpinpinOrders>>(request);

                return data.Data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToLower());
            }

        }
        

        private FunpinpinProduct GetProduct(string url, string product_id, string key)
        {
            // /wp-json/wc/v3/products/
            RestSharp.RestClient client = new RestSharp.RestClient($"https://{url}/");
            var keys = key.Split(":");
            try
            {
                var request = new RestSharp.RestRequest($"wp-json/wc/v3/products/{product_id}?consumer_key={keys[0]}&consumer_secret={keys[1]}", Method.GET);
                return client.Execute<FunpinpinProduct>(request).Data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToLower());
            }
        }


        public class FunpinpinProduct
        {
            public string permalink { get; set; }

            public IList<Funpinpinimages> images { get; set; }
        }

        public class Funpinpinimages
        {
            public string src { get; set; }

        }


        public class FunpinpinOrders
        {
            /// <summary>
            /// api 内部ID
            /// </summary>
            public string id { get; set; }

            public string email { get; set; }

            public string date_created { get; set; }

            public string date_created_gmt { get; set; }



            public string number { get; set; }

            /// <summary>
            /// 所有订单项价格，折扣，运费，税费和商店货币小费的总和
            /// </summary>
            public decimal total { get; set; }

            public string transaction_id { get; set; }

            public string gateway { get; set; }

            public decimal subtotal_price { get; set; }


            public string status { get; set; }


            /// <summary>
            /// 来源URl
            /// </summary>
            public string landing_site { get; set; }

            public string location_id { get; set; }

            /// <summary>
            /// 使用语言
            /// </summary>
            public string customer_locale { get; set; }

            /// <summary>
            ///使用币种
            /// </summary>
            public string currency { get; set; }

            public string date_paid { get; set; }


            /// <summary>
            /// 支付时间
            /// </summary>
            public string date_paid_gmt { get; set; }


            public fppshippingaddress shipping { get; set; }

            public fppshippingaddress billing { get; set; }

            public IList<fpplineitems> line_items { get; set; }


            public string payment_method { get; set; }


            public string payment_method_title { get; set; }



            public IList<shipping_lines> shipping_lines { get; set; }

        }

        public class shipping_lines
        {
            public string method_title { get; set; }
        }


        public class fpplineitems
        {
            public string sku { get; set; }
            public int quantity { get; set; }
            public string name { get; set; }
            public string variation_id { get; set; }

            public decimal total { get; set; }

            public string product_id { get; set; }
        }

        public class fppshippingaddress
        {
            public string address_1 { get; set; }

            public string address_2
            { get; set; }

            public string city
            { get; set; }

            public string company
            { get; set; }

            public string country
            { get; set; }


            public string first_name
            { get; set; }


            public string last_name
            { get; set; }



            public string state
            { get; set; }



            public string postcode
            { get; set; }

            public string email
            { get; set; }

            public string phone
            { get; set; }

        }
    }
}

