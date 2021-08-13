using OrderAsyncWebApp.Models;
using OrderAsyncWebApp.Services;
using OrderSync.Task;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace OrderSync.Task
{
    public class Shopbase : IShop
    {
        public override IList<orders> GetAll(PlatformShop shop)
        {
            Dictionary<string, orders> orderDic = new Dictionary<string, orders>();
            try
            {
                int page = 1;
                //店铺读取订单
                for (; ; )
                {
                    var syncTime = TimeMin(shop.StartSyncTime).ToString("s") + "+08:00";


                    var list = new Shopbase().GetList(shop.ShopId, shop.ApiKey, syncTime, DateTime.Now.ToString("s") + "+08:00", page++);
                    foreach (var info in list)
                    {
                        //info.total_price_usd = info.total_price / Program.RatesDic[info.currency];

                        info.gateway = info.payment_gateway_names[0];

                        orderDic.Add(info.id, info);
                       
                    }
                    if (list.Count == 0)
                    {
                        break;
                    }
                }
                ShopService shopService = new ShopService();
                shopService.DeleteErrorMsg(shop.ShopId);

                return orderDic.Values.ToList();
            }
            catch (Exception ex)
            {
                if (ex.Message.ToString().IndexOf("shop frozen") != -1)
                { 
                
                }
                Console.WriteLine(shop.ShopId, ex, "下载订单失败");
            }
            return orderDic.Values.ToList();
        }

        private IList<orders> GetList(string shopId, string url, string created_at_min = "", string created_at_max = "", int page = 1)
        {
            string responseText = "";
            Encoding encoding = Encoding.UTF8;

            created_at_min = TimeMin(Convert.ToDateTime(created_at_min), shopId).ToString("s") + "+08:00";
            created_at_max = created_at_max.Replace("+", "%2B");
            created_at_min = created_at_min.Replace("+", "%2B");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "/admin/orders.json?financial_status=paid&order_status=open&updated_at_min=" + created_at_min + "&updated_at_max=" + created_at_max + "&page=" + page);
            request.Method = "GET";
            request.Accept = "*/*";
            request.Timeout = 60000;
            request.ReadWriteTimeout = 100000;
            var apikey = url.Replace("https://", "").Split(':')[0];
            var apivalue = url.Replace("https://", "").Split(':')[1].Split('@')[0];
            byte[] bytes = Encoding.UTF8.GetBytes(apikey + ":" + apivalue);

            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytes));

            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                ShopService shopService = new ShopService();
                var msg = $"{shopId};错误信息:店铺" + ex.Message;
                ErrorMsg errorMsg = new ErrorMsg()
                {
                    ShopId = shopId,
                    Msg = msg
                };
                shopService.AddErrorMsg(errorMsg);
                Console.WriteLine(msg);
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
                return Newtonsoft.Json.JsonConvert.DeserializeObject<ShopifyOrder>(responseText).orders;
            }
            catch(Exception ex)
            {
                ShopService shopService = new ShopService();
                var msg = $"{shopId};错误信息:店铺" + ex.Message;
                ErrorMsg errorMsg = new ErrorMsg()
                {
                    ShopId = shopId,
                    Msg = msg
                };
                shopService.AddErrorMsg(errorMsg);
                Console.WriteLine(msg);


                Console.WriteLine("下载订单失败");
                System.Console.WriteLine("responseText:" + responseText);
            }
            return null;
        }
    }
}
