using MediatR;
using Microsoft.Extensions.Logging;
using OrderAsyncWebApp;
using OrderAsyncWebApp.Models;
using OrderAsyncWebApp.Services;
using Quartz;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Help.Jobs
{
    [DisallowConcurrentExecution]
    public class TestJob : IJob
    {
        private readonly ILogger<TestJob> _logger;
        private readonly IMediator _mediator;

        public TestJob(ILogger<TestJob> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogWarning("开始同步:" + DateTime.Now.ToString());
            try
            {
                var shopList = GetPlatformShop().ToList();
                var handler = new ShopHandler(_mediator);
                handler.Producer(shopList);
                _logger.LogWarning("结束同步:" + DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogWarning("错误:" + ex.Message.ToString());
            }
            
            return Task.CompletedTask;
        }
        public List<PlatformShop> GetPlatformShop()
        {
            ShopService shopService = new ShopService();
            ShopSearch shopSearch = new ShopSearch();
            var list = shopService.GetShopList(shopSearch).Where(obj => obj.ApiKey != "" && obj.IsOpen ==1).ToList();
            return list;
        }
        //public  List<SysShop>GetAllShop()
        //{
        //    Services services = new Services();
        //    var list = services.GetShopList().Where(obj => obj.ApiKey != ""&&obj.IsOpen == yq.enums.BooleanType.No).ToList();
        //    return list;
        //}
    }
}
