using OrderAsyncWebApp.Models;
using OrderSync.Task;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;


namespace OrderSync.Shop
{
    public class Funpinpin2 : IShop
    {

        PlatformShop sysShop = null;

        public override IList<orders> GetAll(PlatformShop shop)
        {
            try
            {
                Dictionary<string, OrderSync.Task.orders> orderDic = new Dictionary<string, OrderSync.Task.orders>();
                sysShop = shop;

                //店铺读取订单

                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区


                var syncTime = TimeMin(shop.StartSyncTime);
                DateTime dt = DateTime.Now;

                syncTime = TimeMin(syncTime, shop.ShopId);
                string url = "";
                url = shop.AdminUrl+ "openapi/2021-04/orders/?limit=200&status=open&payment_status=paid&fulfillment_status=unshipped&updated_at__gte="+ syncTime.ToString("yyyy-MM-dd HH:mm:ss") + "&updated_at__lte="+ dt.ToString("yyyy-MM-dd HH:mm:ss") + "";

                while (url!="")
                {
                    
                    var list = GetList(shop.Domain, url, shop.ApiKey);
                    foreach (var info in list)
                    {
                        orderDic.Add(info.id, info);
                    }
                    System.Threading.Thread.Sleep(2000);
                    url = NextPage(shop.Domain, url, shop.ApiKey);


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
        public static string Base64Encode(string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            return Convert.ToBase64String(bytes);
        }
        public string NextPage(string domain, string url, string apikey)
        {
            string responseText = "";
            Encoding encoding = Encoding.UTF8;

           
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Accept = "*/*";
            request.Headers.Add("Authorization", "Basic " + Base64Encode(apikey.Split(':')[0] + ":" + apikey.Split(':')[1]));

            request.Timeout = 60000;
            request.ReadWriteTimeout = 100000;
            request.ContentType = "application/json";

            HttpWebResponse response;
            string nextLink = "";
            try
            {
                response = (HttpWebResponse)request.GetResponse();


                var listLink = response.Headers.GetValues("Link");

                if (listLink != null)
                {
                    //<https://sunnyear.myfunpinpin.com/admin/openapi/2021-04/orders/?page_info=cj0xJnA9MjAyMS0wNy0xNSsxNCUzQTA1JTNBNDcuNjkzMzg3JTJCMDAlM0EwMA%3D%3D&shop_id=12782>; rel="previous", <https://sunnyear.myfunpinpin.com/admin/openapi/2021-04/orders/?page_info=cD0yMDIxLTA3LTE0KzE3JTNBMzUlM0EwOS40MzYwOTglMkIwMCUzQTAw&shop_id=12782>; rel="next"
                    var a = listLink[0].Split(',');

                    foreach (var b in a)
                    {
                        if (b.LastIndexOf("rel=\"next\"") != -1)
                        {
                            var Netstring = b.Trim();
                            nextLink = Netstring.Substring(Netstring.IndexOf('<') + 1, Netstring.LastIndexOf('>') - 1);
                        }
                    }
                }
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
            try
            {
                request.Abort();
                request = null;
                return nextLink;
            }
            catch (Exception ex)
            {
                Console.WriteLine(domain + nextLink, ex, "下一页订单失败");
            }
            return nextLink;
        }
        public  IList<orders> GetList(string domain, string url, string apikey)
        {


           
            string responseText = "";
            Encoding encoding = Encoding.UTF8;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{url}");

            request.Method = "GET";
            request.Accept = "*/*";
            request.Headers.Add("Authorization", "Basic " + Base64Encode(apikey.Split(':')[0] + ":" + apikey.Split(':')[1]));
            request.Timeout = 60000;
            request.ReadWriteTimeout = 100000;
            request.ContentType = "application/json";
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
            catch (Exception ex)
            {
                Console.WriteLine(domain + responseText + "下载订单失败:" + ex.Message);
            }
            return null;
        }
        public IList<orders> Conversion(string xshopy)
        {
            IList<orders> orders = new List<orders>();
            var list = Newtonsoft.Json.JsonConvert.DeserializeObject<dataorder>(xshopy).orders;
            foreach (var info in list)
            {
                orders order = new orders();
                order.id = info.id.ToString();
                order.email = info.customer.email;
                // order.updated_at = info.pay_time;
                order.number = info.line_items.Count.ToString();
                order.total_price = Convert.ToDecimal(info.total_amount);
                order.gateway = info.payment_line.payment_method;
                order.subtotal_price = Convert.ToDecimal(info.subtotal);
                order.financial_status = info.status;

                order.currency = info.currency;
                order.name = info.order_number.ToString();
                //order.total_price_usd = Convert.ToDecimal(info.total_amount) / Program.RatesDic[info.currency];

                order.browser_ip = "";
                order.order_status_url = "https://" ;
                order.note = info.customer_note;
                order.token = info.id.ToString();
                order.updated_at = info.updated_at;
                order.created_at = info.created_at;


                order.phone = info.shipping_address.phone;
                order.shipping_address = new shippingaddress()
                {
                    address1 = info.shipping_address.address_1,
                    address2 = info.shipping_address.address_2,
                    city = info.shipping_address.city,
                    company = info.shipping_address.company,
                    country = info.shipping_address.country,
                    country_code = info.shipping_address.country_code,
                    first_name = info.shipping_address.first_name,
                    last_name = info.shipping_address.last_name,
                    name = info.name,
                    phone = info.shipping_address.phone,
                    province = info.shipping_address.province,
                    zip = info.shipping_address.zip,
                    province_code = info.shipping_address.province_code
                };
                order.checkout_id = info.payment_line.transaction_no;
                order.shipping_lines = new List<shippinglines>() { new shippinglines() {
                 title=info.shipping_method,
                  id=info.shipping_method
                } };
                order.line_items = new List<lineitems>();
                foreach (var product in info.line_items)
                {
                    order.line_items.Add(new lineitems()
                    {
                        sku = product.sku,
                        quantity = product.num,
                        title = (product.title),
                        variant_id = product.id.ToString(),
                        variant_title = (product.variant_title),
                        price = Convert.ToDecimal( product.price),
                        product_id = product.product_id.ToString(),
                        imageurl = product.image_url,
                        url = "https://" ,
                    });
                }
                order.processed_at = Convert.ToDateTime(info.pay_time);
                //  order.c = info.id;

                if (order.line_items.Count != 0)
                {
                    orders.Add(order);
                }

            }

            return orders;
        }
        public class DiscountsItem
        {
            /// <summary>
            /// 
            /// </summary>
            public string price { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string write { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string percentage { get; set; }
        }
        public class Properties_data
        {
            /// <summary>
            /// 
            /// </summary>
            public string Field { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string color { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string preview { get; set; }
        }
        public class Line_itemsItem
        {
            /// <summary>
            /// 
            /// </summary>
            public int id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string variant_title { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string removal_text { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int product_id_subject { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int restock_enable { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<DiscountsItem> discounts { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int num { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string subtotal { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string subtotal_discount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int product_id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string sku { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string price { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string compare_at_price { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string subtotal_original { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string currency { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string image_url { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string fulfillment_status { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> options { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int fulfillment_amount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string fulfillment_price { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int is_delete { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int id_restock { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string refund_location_id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string discount_price { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string write { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string weight { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string weight_unit { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string vendor { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int order { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Properties_data properties_data { get; set; }
        }
        public class Customer
        {
            /// <summary>
            /// 
            /// </summary>
            public int id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string email { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string first_name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string last_name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string phone { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int orders_count { get; set; }
        }
        public class Line_unshippingItem
        {
            /// <summary>
            /// 
            /// </summary>
            public int id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string variant_title { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string removal_text { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int product_id_subject { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int restock_enable { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<DiscountsItem> discounts { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int product_id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string sku { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int num { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string price { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string subtotal { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string compare_at_price { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string subtotal_original { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string currency { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string image_url { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string fulfillment_status { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> options { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int fulfillment_amount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string fulfillment_price { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int is_delete { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int id_restock { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string refund_location_id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string discount_price { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string subtotal_discount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string write { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string weight { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string weight_unit { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string vendor { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int order { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Properties_data properties_data { get; set; }
        }

        public class Payment_line
        {
            /// <summary>
            /// 
            /// </summary>
            public string payment_method { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string transaction_no { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string merchant_id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string merchant_email { get; set; }
        }
        public class Shipping_address
        {
            /// <summary>
            /// 
            /// </summary>
            public string zip { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string city { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string phone { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string company { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string country { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string province { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string address_1 { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string address_2 { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string last_name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string first_name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string country_code { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string province_code { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string email { get; set; }
        }

        public class Billing_address
        {
            /// <summary>
            /// 
            /// </summary>
            public string zip { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string city { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string phone { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string company { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string country { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string province { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string address_1 { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string address_2 { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string last_name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string first_name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string country_code { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string province_code { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string email { get; set; }
        }

        public class OrdersItem
        {
            /// <summary>
            /// 
            /// </summary>
            public int id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<Line_itemsItem> line_items { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string pay_time { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Customer customer { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string total_amount_fix { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> fulfillment_info { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<Line_unshippingItem> line_unshipping { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string real_pay { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string balance_amount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string pay_method { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Payment_line payment_line { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string shipping_original { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string total_before_discount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Shipping_address shipping_address { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Billing_address billing_address { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string created_at { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string updated_at { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int order_number { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string status { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string payment_status { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string fulfillment_status { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string user_email { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string currency { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string language_code { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string customer_note { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int all_num { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string subtotal { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string subtotal_original { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string shipping_method { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string shipping_amount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string shipping_amount_refund { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string shipping_discount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string discount_amount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<DiscountsItem> discounts { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string tax_amount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string total_amount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string total_amount_original { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string weight { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string weight_unit { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string cancelled_at { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string already_paid { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string refund_amount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int fulfillment_amount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> tags { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> refunds { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string cancelled_reason { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string source { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string is_archive { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string edited_reason { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string tip { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string shipping_insurance { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string shipping_discount_code { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int user { get; set; }
        }

        public class dataorder
        {
            /// <summary>
            /// 
            /// </summary>
            public List<OrdersItem> orders { get; set; }
        }



        public class AddTrackingFpp
        {
            public List<Fulfillment> fulfillment { get; set; } 

            public Courier_company courier_company { get; set; }
            public string waybill_number { get; set; } = "";
            public string send_email { get; set; } = "NO";

        }

        public class Fulfillment
        { 
            public int id { get; set; }
            public int quantity { get; set; }
        }

        public class Courier_company
        { 
            public string name { get; set; }
            public string url { get; set; }
        }
    }
}
