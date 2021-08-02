using Microsoft.AspNetCore.Mvc;
using OrderAsyncWebApp.Models;
using OrderAsyncWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAsyncWebApp.Controllers
{
    public class SysShopController : Controller
    {
        public IActionResult Index(ShopSearch shopSearch)
        {
            return View(shopSearch);
        }
        public IActionResult DoList(ShopSearch shopSearch)
        {
            ShopService shopService = new ShopService();
            var res=shopService.GetShopList(shopSearch);
            return PartialView(res);
        }
        public IActionResult Add()
        {
            return View();
        }
        public IActionResult DoAdd(PlatformShop shop)
        {
            ShopService shopService = new ShopService();
            var res=shopService.AddShop(shop);
            return Ok(res);
        }

        public IActionResult Edit(string shopId)
        {
            ShopService shopService = new ShopService();
            var res = shopService.GetShop(shopId);
            return View(res);
        }
        public IActionResult DoEdit(PlatformShop shop)
        {
            ShopService shopService = new ShopService();
            var res = shopService.EditShop(shop);
            return Ok(res);
        }
    }
}
