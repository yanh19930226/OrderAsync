using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OrderAsyncWebApp.Models;
using OrderSync.Shop;

namespace OrderSync.Task
{
    public abstract class IShop
    {
        //public static IShop Factory(Types types)
        //{
        //    IShop shopImp = null;
        //    if (types == OrderAsyncWebApp.Models.Types.Shopify)
        //    {
        //        shopImp = new Shopify();
        //    }
        //    else if (types == OrderAsyncWebApp.Models.Types.Xshoppy)
        //    {
        //        shopImp = new XShoppy();
        //    }
        //    else if (types == OrderAsyncWebApp.Models.Types.Shoplaza)
        //    {
        //        shopImp = new Shoplaza();
        //    }
        //    else if (types == OrderAsyncWebApp.Models.Types.Shopbase)
        //    {
        //        shopImp = new Shopbase();
        //    }
        //    else if (types == OrderAsyncWebApp.Models.Types.NewWeb)
        //    {
        //        shopImp = new ShopNewWeb();
        //    }
        //    else if (types == OrderAsyncWebApp.Models.Types.Funpinpin)
        //    {
        //        shopImp = new Funpinpin();
        //    }
        //    else if (types == OrderAsyncWebApp.Models.Types.Funpinpin2)
        //    {
        //        shopImp = new Funpinpin2();
        //    }
        //    return shopImp;
        //}


        public static IShop Factory(int types)
        {
            IShop shopImp = null;
            if (types == 1)
            {
                shopImp = new Shopify();
            }
            else if (types == 2)
            {
                shopImp = new XShoppy();
            }
            else if (types == 4)
            {
                shopImp = new Shoplaza();
            }
            else if (types == 3)
            {
                shopImp = new Shopbase();
            }
            else if (types == 5)
            {
                shopImp = new ShopNewWeb();
            }
            else if (types == 6)
            {
                shopImp = new Funpinpin();
            }
            else if (types == 7)
            {
                shopImp = new Funpinpin2();
            }
            return shopImp;
        }

        public abstract IList<orders> GetAll(PlatformShop shop);

        public DateTime TimeMin(DateTime dt, string shopId)
        {
                 if (Convert.ToDateTime(dt) < Convert.ToDateTime("2020-12-01 00:00:01"))
                 {
                     return Convert.ToDateTime("2020-12-01 00:00:01");
                 }
                 return dt;
          
        }
    }
}
