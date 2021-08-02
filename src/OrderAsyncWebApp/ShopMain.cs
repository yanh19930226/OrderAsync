using MediatR;
using MongoDB.Driver;
using OrderAsyncWebApp.Models;
using OrderSync.Task;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAsyncWebApp
{
    public class ShopMain
    {
        PlatformShop shop;
        public ShopMain(PlatformShop _shop)
        {
            shop = _shop;
        }

        public List<orders> Run()
        {
            var shopImp = IShop.Factory(shop.Types);
            try
            {
                List<orders> orderList = shopImp.GetAll(shop).ToList();
                return orderList;
            }
            catch (Exception ex)
            {
                return new List<orders>();
            }
        }
    }
}
