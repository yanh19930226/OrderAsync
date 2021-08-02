using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using OrderAsyncWebApp.Models;

namespace OrderSync.Task
{
    public class XShoppy : IShop
    {
        PlatformShop sysShop = null;

        public override IList<OrderSync.Task.orders> GetAll(PlatformShop shop)
        {
            try
            {
                Dictionary<string, OrderSync.Task.orders> orderDic = new Dictionary<string, OrderSync.Task.orders>();
                sysShop = shop;

                //店铺读取订单
                for (var page = 1; ; page++)
                {
                    System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
                    var syncTime =DateTime.Now.AddDays(-30);

                    
                    syncTime = TimeMin(syncTime,shop.ShopId);
                    
                    long timeStamp = (long)(syncTime - startTime).TotalSeconds;

                    var list = GetList(shop.Domain, shop.ApiKey, timeStamp.ToString(), page);
                    foreach (var info in list)
                    {
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
                Console.WriteLine(shop.ShopId + " xshopy" + "下载订单失败:"+ex.Message);
            }
            return new List<orders>();
        }

        public static string Base64Encode(string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            return Convert.ToBase64String(bytes);
        }

        private IList<orders> GetList(string domain, string apikey, string created_at_min, int page = 1)
        {
            string responseText = "";
            Encoding encoding = Encoding.UTF8;

            var url = "https://openapi.xshoppy.shop/order/orders/list";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "?limit=100&page=" + page + "&financial_status=paid&pay_time_start=" + created_at_min);
            request.Method = "GET";//&status=pending
            request.Accept = "*/*";
            request.Headers.Add("X-SAIL-ACCESS-TOKEN", apikey.Split(':')[2]);
            request.Headers.Add("Authorization", "Basic " + Base64Encode(apikey.Split(':')[0] + ":" + apikey.Split(':')[1]));
            request.Timeout = 60000;
            request.ReadWriteTimeout = 100000;
            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)request.GetResponse();


            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
               
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
                {
                    responseText = reader.ReadToEnd();
                   
                }
                request.Abort();
                request = null;
                throw new Exception(responseText);
            }
            if (response != null)
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
                {
                    responseText = reader.ReadToEnd();
                }
            }
            try
            {
                request.Abort();
                request = null;
                return Conversion(responseText);
            }
            catch(Exception ex)
            {
                Console.WriteLine(domain + "下载订单失败"+responseText+ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 数据格式转化
        /// </summary>
        /// <param name="xshopy"></param>
        /// <returns></returns>
        public IList<orders> Conversion(string xshopy)
        {
            IList<orders> orders = new List<orders>();
            var list = Newtonsoft.Json.JsonConvert.DeserializeObject<dataorder>(xshopy).data.data;
            foreach (var info in list)
            {
                orders order = new orders();
                order.id = info.id;
                order.email = info.email;
                // order.updated_at = info.pay_time;
                order.number = info.products.Count.ToString();
                order.total_price = info.total_price;
                order.gateway = info.gateway;
                order.subtotal_price = info.subtotal_price;
                order.financial_status = info.financial_status;

                order.currency = info.currency;
                order.name = info.order_name;
                //order.total_price_usd = info.total_price / Program.RatesDic[info.currency];

                order.browser_ip = info.browser_ip;
                order.order_status_url = "https://" + info.order_status_url;
                order.note = info.note;
                order.token = info.id;
                order.updated_at = info.updated_at;
                order.created_at = info.created_at;


                order.phone = info.shipping.phone;
                order.shipping_address = new shippingaddress()
                {
                    address1 = info.shipping.address1,
                    address2 = info.shipping.address2,
                    city = info.shipping.city,
                    company = info.shipping.company,
                    country = info.shipping.country,
                    country_code = info.shipping.country_code,
                    first_name = info.shipping.first_name,
                    last_name = info.shipping.last_name,
                    name = info.shipping.name,
                    phone = info.shipping.phone,
                    province = info.shipping.province,
                    zip = info.shipping.zip,
                    province_code = info.shipping.province_code
                };
                order.checkout_id = info.checkout_id;
                order.shipping_lines = new List<shippinglines>() { new shippinglines() {
                 title=info.shipping_method,
                  id=info.shipping_method
                } };
                order.line_items = new List<lineitems>();
                foreach (var product in info.products)
                {
                    order.line_items.Add(new lineitems()
                    {
                        sku = product.sku,
                        quantity = product.quantity,
                        title = (product.title),
                        variant_id = product.variant_id,
                        variant_title = (product.attr_name),
                        price = product.price,
                        product_id = product.product_id,
                        imageurl = product.image,
                        url = "https://" + sysShop.Domain + product.url,
                    });
                }
                order.processed_at = info.pay_time;
                //  order.c = info.id;

                if (order.line_items.Count != 0)
                {
                    orders.Add(order);
                }

            }
            return orders;
        }

        public class dataorder
        {
            public dataorders data { get; set; }
        }

        public class dataorders
        {
            public IList<data> data { get; set; }
        }

        public class data
        {
            public string shop_url { set; get; }

            public string id { set; get; }

            public string browser_ip { set; get; } = "";

            public string note { set; get; } = "";




            public string customer_id { set; get; }

            public string shipping_method { set; get; }


            public string email { set; get; }
            public string gateway { set; get; }
            public Decimal total_price { set; get; }
            public Decimal subtotal_price { set; get; }
            public string total_weight { set; get; }


            public string currency { set; get; }
            public string financial_status { set; get; }
            public string order_number { set; get; }
            public string order_name { set; get; }
            public DateTime pay_time { set; get; }

            public string updated_at { set; get; }
            public string created_at { set; get; }


            public string transaction_id { set; get; }
            public string order_status_url { set; get; }

            public string checkout_id { set; get; }

            public shipping shipping { set; get; }
            public IList<products> products { set; get; }
        }

        public class products
        {
            public string product_id { set; get; }
            public string variant_id { set; get; }
            public string title { set; get; }
            public int quantity { set; get; }

            public decimal price { set; get; }


            public string sku { set; get; }

            public string image { set; get; }

            public string url { set; get; }


            public string attr_name { set; get; }

        }

        public class shipping
        {
            public string name { set; get; }

            public string province_code { set; get; }




            public string address1 { set; get; }
            public string address2 { set; get; }
            public string city { set; get; }
            public string company { set; get; }

            public string country_code { set; get; }

            public string country { set; get; }
            public string first_name { set; get; }
            public string last_name { set; get; }
            public string province { set; get; }
            public string phone { set; get; }

            public string email { set; get; }

            public string zip { set; get; }


        }
    }
}
