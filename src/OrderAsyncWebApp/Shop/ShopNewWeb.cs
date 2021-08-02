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
    public class ShopNewWeb : IShop
    {
        public override IList<orders> GetAll(PlatformShop shop)
        {
            Dictionary<string, orders> orderDic = new Dictionary<string, orders>();
            try
            {
                //店铺读取订单
                var oldid = "";
                for(var p = 1; ;p++ )
                {
                    var list = new ShopNewWeb().GetList(shop.ShopId, shop.Domain, DateTime.Now, shop.ApiKey,p);
                    foreach (var info in list)
                    {
                        orderDic.Add(info.id, info);
                    }
                    if (list.Count == 0)
                    {
                        break;
                    }
                    if (list[list.Count - 1].id == oldid)
                    {
                        break;
                    }
                    oldid = list[list.Count-1].id;

                }
               

                return orderDic.Values.ToList();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(shop.ShopId + "下载订单失败" + ex.Message.ToString());
            }
            return new List<orders>();
        }

        private IList<orders> GetList(string shopId,  string url, DateTime created_at_min, string key,int p=1)
        {
            string responseText = "";
            Encoding encoding = Encoding.UTF8;

            var created_at_max = "&endAddTime=" + DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
             created_at_min = TimeMin(Convert.ToDateTime(created_at_min), shopId).AddDays(-30);

            var reqUrl = "https://" + url + "/api/OrderListApi?startAddTime=" + created_at_min + created_at_max + "&KEY=" + key + "&pageindex=" + p;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(reqUrl);
            request.Method = "GET";
            request.Accept = "*/*";
            request.Timeout = 60000;
          //  request.MaximumAutomaticRedirections = 1;
        // request.AllowAutoRedirect = true;
            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                Console.WriteLine($"{shopId}请求地址:{reqUrl};错误信息:店铺"+ex.Message);
                response = (HttpWebResponse)ex.Response;
                if (response.StatusCode == HttpStatusCode.Found) // 判斷是否為 302
                {
                    request = (HttpWebRequest)WebRequest.Create(response.Headers["Location"]);
                    request.Method = "GET";
                    request.Accept = "*/*";
                    request.Timeout = 60000;
                    response = (HttpWebResponse)request.GetResponse();
                }
                else
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
                    {
                        responseText = reader.ReadToEnd();
                    }
                    request.Abort();
                    throw new Exception(responseText);

                }
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
                return Newtonsoft.Json.JsonConvert.DeserializeObject<ShopifyOrder>(responseText).orders;
            }
            catch
            {
                System.Console.WriteLine("responseText:" + responseText);
            }
            return null;
        }
    }
}
