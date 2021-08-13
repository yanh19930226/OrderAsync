using Microsoft.AspNetCore.Mvc;
using OrderAsyncWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAsyncWebApp.Controllers
{
    public class ErrorMsgController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult DoList()
        {
            ShopService shopService = new ShopService();
            var res = shopService.GetErrorMsgList();
            return PartialView(res);
        }
    }
}
