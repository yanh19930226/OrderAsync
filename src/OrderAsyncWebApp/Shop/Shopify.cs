using OrderAsyncWebApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace OrderSync.Task
{
    public class Shopify : IShop
    {
        public override IList<orders> GetAll(PlatformShop shop)
        {
            Dictionary<string, orders> orderDic = new Dictionary<string, orders>();
            try
            {
                string time = "";
                //店铺读取订单
                for (; ; )
                {
                    System.Threading.Thread.Sleep(2000);
                    var list = new Shopify().GetList(shop.ShopId, shop.ApiKey, DateTime.Now.AddDays(-30).ToString("s") + "+08:00", time, "");
                    foreach (var info in list)
                    {
                        orderDic.Add(info.id, info);
                    }
                    if (list.Count == 0)
                    {
                        break;
                    }
                    else
                    {
                        if (list[list.Count - 1].created_at == time)
                        {
                            break;
                        }
                        time = list[list.Count - 1].created_at;
                    }
                }
                return orderDic.Values.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("下载订单失败");
            }
            return new List<orders>();
        }

        private IList<orders> GetList(string shopId, string url, string created_at_min, string created_at_max = "", string since_id = "")
        {
            string responseText = "";
            Encoding encoding = Encoding.UTF8;

            if (created_at_max.Length != 0)
            {
                created_at_max = "&updated_at_max=" + created_at_max.Replace("+", "%2B");
            }

            created_at_min = TimeMin(Convert.ToDateTime(created_at_min), shopId).ToString("s") + "+08:00";
            created_at_min = created_at_min.Replace("+", "%2B");

            var req = url + "/admin/api/2020-07/orders.json?status=open&updated_at_min=" + created_at_min + created_at_max + "&since_id=" + since_id;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(req);
            request.Method = "GET";
            request.Accept = "*/*";
            var apikey = url.Replace("https://", "").Split(':')[0];
            var apivalue = url.Replace("https://", "").Split(':')[1].Split('@')[0];
            request.Timeout = 60000;
            request.ReadWriteTimeout = 100000;
            request.Credentials = new NetworkCredential(apikey, apivalue);
            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                Console.WriteLine($"{shopId}请求地址:{req};错误信息:店铺" + ex.Message);
                response = (HttpWebResponse)ex.Response;
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
                {
                    responseText = reader.ReadToEnd();
                }
                request.Abort();
                request = null;
                throw new Exception(responseText);
            }

            if (response.StatusCode == HttpStatusCode.Redirect ||
           response.StatusCode == HttpStatusCode.MovedPermanently)
            {
                //发生重定向就重新获取
               return GetList(shopId, response.Headers["Location"], created_at_min, since_id);
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
                return Newtonsoft.Json.JsonConvert.DeserializeObject<ShopifyOrder>(responseText).orders;
            }
            catch(Exception ex)
            {
                Console.WriteLine(shopId + responseText, ex, "下载订单失败");
                System.Console.WriteLine("responseText:" + responseText);
            }
            return null;
        }
    }

    public class fulfillmentsInfo
    {
        public IList<fulfillmentsClass> fulfillments { get; set; }
    }
    
    public class fulfillmentsClass
    {
        public string id { get; set; }
    }

    public class imageslist
    {
        public IList<imagesClass> images { get; set; }
    }

    public class imagesClass
    {
        public string src { get; set; }
        public IList<string> variant_ids { get; set; }
    }

    public class ShopifyOrder
    {
        public IList<orders> orders { get; set; } = new List<orders>();

    }
    public class orders
    {
        /// <summary>
        /// api 内部ID
        /// </summary>
        public string id { get; set; }

        public string email { get; set; }

        public string updated_at { get; set; }

        public string number { get; set; }

        /// <summary>
        /// 所有订单项价格，折扣，运费，税费和商店货币小费的总和
        /// </summary>
        public decimal total_price { get; set; }

        public string transaction_id { get; set; }

        public string gateway { get; set; }

        public decimal subtotal_price { get; set; }

        /// <summary>
        /// 履行状态
        /// </summary>
        public string financial_status { get; set; }

        /// <summary>
        /// 物流状态
        /// </summary>
        public string fulfillment_status { get; set; }

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

        public string cart_token { get; set; }

        /// <summary>
        /// order_number 客服和客户看的id
        /// </summary>
        public string name { get; set; }


        public decimal total_price_usd { get; set; }


        public string browser_ip { get; set; }


        public string order_status_url { get; set; }

        public clientdetails client_details { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string note { get; set; }

        /// <summary>
        /// 订单唯一id
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// tags
        /// </summary>
        public string tags { get; set; }

        public string created_at { get; set; }

        public string phone { get; set; }

        public shippingaddress shipping_address { get; set; }

        public string checkout_id { get; set; }

        public IList<shippinglines> shipping_lines { get; set; }

        /// <summary>
        /// 兼容电匠
        /// </summary>
        public shippinglinesinfo shipping_line { get; set; }

        

        public IList<lineitems> line_items { get; set; }



        public DateTime processed_at { get; set; }

         /// <summary>
         /// 兼容电匠
         /// </summary>
        public paymentline payment_line { get; set; }


        /// <summary>
        /// 兼容shopbase
        /// </summary>
        public IList<string> payment_gateway_names { get; set; }

    }

    public class paymentline
    {
        public string payment_channel { get; set; }
        public string merchant_id { get; set; }


        public string transaction_no { get; set; }
        
    }

    public class lineitems
    {
        public string sku { get; set; }
        public int quantity { get; set; }
        public string title { get; set; }
        public string variant_id { get; set; }

        public string variant_title { get; set; }

        public string product_title { get; set; }

        public decimal price { get; set; }

        public string product_id { get; set; }

        public string url { get; set; } = "";

        
   public string image_src { get => imageurl; set => imageurl = value; }

        public string imageurl { get; set; }
        public string image { get => imageurl; set => imageurl = value;  }

        public string product_url { get => url; set => url = value; }
    }

    public class shippinglinesinfo
    {

        //兼容店匠
        public string name { get; set; }
    }

    public class shippinglines
    {
        public string id { get; set; }
        public string title { get; set; }


        public string source { get; set; }
    }

    public class clientdetails
    {
        public string browser_height
        { get; set; }

        public string browser_width
        { get; set; }
        public string accept_language
        { get; set; }

        public string browser_ip
        { get; set; }

        public string session_hash
        { get; set; }

        public string user_agent
        { get; set; }
    }

    public class shippingaddress
    {
        public string address1 { get; set; }

        public string address2
        { get; set; }

        public string city
        { get; set; }

        public string company
        { get; set; }

        public string country
        { get; set; }

        public string country_code
        { get; set; }

        public string first_name
        { get; set; }


        public string last_name
        { get; set; }
        public string latitude
        { get; set; }



        public string longitude
        { get; set; }


        public string name
        { get; set; }

        public string phone
        { get; set; }


        public string province
        { get; set; }


        public string province_code
        { get; set; }

        public string zip
        { get; set; }


        /// <summary>
        /// 兼容电匠
        /// </summary>
        public string email
        { get; set; }
        
    }
}