using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OrderAsyncWebApp.Models;
using OrderAsyncWebApp.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAsyncWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult DoLogin(string email, string password)
        {
            bool Result = true; ;
            if (ModelState.IsValid)
            {
                if (email.Trim() == "easycool" && password.Trim() == "easy123456")
                {
                    Result = true;
                }
                else
                {
                    Result = false;
                }
            }
            return Ok(Result);

        }
        public IActionResult Order()
        {
            ShopService shopService = new ShopService();
            ShopSearch shopSearch = new ShopSearch();
            var list = shopService.GetShopList(shopSearch).ToList();
            ViewBag.Shop = list;
            return View(
                  new SearchModel()
                  {
                      StartTime = DateTime.Now.ToString("yyyy-MM-dd"),
                      EndTime = DateTime.Now.ToString("yyyy-MM-dd"),
                  }
           );
        }
        public IActionResult DoList(SearchModel searchModel)
        {
            OrderAsyncWebApp.Services.Services services = new Services.Services();
            var res = services.GetOrderList(searchModel);
            ViewBag.Count = res?.Count;
            return PartialView(res);
        }

        public IActionResult DoExport(SearchModel searchModel)
        {
            OrderAsyncWebApp.Services.Services services = new Services.Services();
            var list = services.GetOrderList(searchModel).ToList();
            HSSFWorkbook book = new HSSFWorkbook();
            ISheet s1 = book.CreateSheet("订单信息");
            IRow r1 = s1.CreateRow(0);
            r1.CreateCell(0).SetCellValue("店铺类型");
            r1.CreateCell(1).SetCellValue("店铺名称");
            r1.CreateCell(2).SetCellValue("店铺Id");
            r1.CreateCell(3).SetCellValue("平台订单Id");
            r1.CreateCell(4).SetCellValue("平台订单号");
            r1.CreateCell(5).SetCellValue("商户");
            r1.CreateCell(6).SetCellValue("PPId");
            r1.CreateCell(7).SetCellValue("邮箱");
            r1.CreateCell(8).SetCellValue("电话");
            r1.CreateCell(9).SetCellValue("货币");
            r1.CreateCell(10).SetCellValue("国家");
            r1.CreateCell(11).SetCellValue("州");
            r1.CreateCell(12).SetCellValue("市");
            r1.CreateCell(13).SetCellValue("交易号");
            r1.CreateCell(14).SetCellValue("Gateway");
            r1.CreateCell(15).SetCellValue("总金额");
            r1.CreateCell(16).SetCellValue("物流名称");
            r1.CreateCell(17).SetCellValue("姓名");
            r1.CreateCell(18).SetCellValue("地址");
            r1.CreateCell(19).SetCellValue("邮编");
            r1.CreateCell(20).SetCellValue("Ip");
            r1.CreateCell(21).SetCellValue("OrderStatusUrl");
            r1.CreateCell(22).SetCellValue("订单时间");
            r1.CreateCell(23).SetCellValue("订单修改时间");
            var i = 1;
            foreach (var info in list)
            {
                ShopService shopService = new ShopService();
                var shop = shopService.GetShop(info.ShopId);
                var k = 0;
                NPOI.SS.UserModel.IRow rt = s1.CreateRow(i++);
                rt.CreateCell(0).SetCellType(CellType.String);

                if (shop.Types == 1)
                {
                    rt.CreateCell(k++).SetCellValue("Shopify");
                }
                else if(shop.Types==2)
                {
                    rt.CreateCell(k++).SetCellValue("XShoppy");
                }
                else if (shop.Types == 3)
                {
                    rt.CreateCell(k++).SetCellValue("Shopbase");
                }
                else if (shop.Types == 4)
                {
                    rt.CreateCell(k++).SetCellValue("Shoplaza");
                }
                else if (shop.Types == 5)
                {
                    rt.CreateCell(k++).SetCellValue("自建站");
                }
                else if (shop.Types == 6)
                {
                    rt.CreateCell(k++).SetCellValue("Funpinpin");
                }
                else if (shop.Types == 7)
                {
                    rt.CreateCell(k++).SetCellValue("Funpinpin");
                }
                rt.CreateCell(k++).SetCellValue(shop.Title);
                rt.CreateCell(k++).SetCellValue(info.ShopId);
                rt.CreateCell(k++).SetCellValue(info.ExternalOrderId);
                rt.CreateCell(k++).SetCellValue(info.ExternalName);
                rt.CreateCell(k++).SetCellValue(info.MerchantId);
                rt.CreateCell(k++).SetCellValue(info.PPId);
                rt.CreateCell(k++).SetCellValue(info.Email);
                rt.CreateCell(k++).SetCellValue(info.Phone);
                rt.CreateCell(k++).SetCellValue(info.Currency);
                rt.CreateCell(k++).SetCellValue(info.Country);
                rt.CreateCell(k++).SetCellValue(info.State);
                rt.CreateCell(k++).SetCellValue(info.City);
                rt.CreateCell(k++).SetCellValue(info.CheckoutId);
                rt.CreateCell(k++).SetCellValue(info.Gateway);
                rt.CreateCell(k).SetCellType(CellType.Numeric);
                rt.CreateCell(k++).SetCellValue(info.TotalPrice.ToString());
                rt.CreateCell(k++).SetCellValue(info.ShopifLogisticsName);
                rt.CreateCell(k++).SetCellValue(info.LastName+" "+info.FirstName);
                rt.CreateCell(k++).SetCellValue(info.Address1);
                rt.CreateCell(k++).SetCellValue(info.Zip);
                rt.CreateCell(k++).SetCellValue(info.Ip);
                rt.CreateCell(k++).SetCellValue(info.OrderStatusUrl);
                rt.CreateCell(k++).SetCellValue(Convert.ToDateTime(info.Created_at).AddHours(8).ToString());
                rt.CreateCell(k++).SetCellValue(Convert.ToDateTime(info.Updated_at).AddHours(8).ToString());
            }
            using (Stream stream = new MemoryStream())
            {
                book.Write(stream);
                stream.Seek(0, SeekOrigin.Begin);
                book.Close();
                return File(StreamToBytes(stream), "application/ms-excel", DateTime.Now.ToString("yyyy-MM-dd_HH_mm") + "订单导出.xls");
            }
        }
       
        public byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];

            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;

        }
        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
