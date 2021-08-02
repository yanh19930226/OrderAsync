using MediatR;
using Microsoft.Extensions.Logging;
using OrderAsyncWebApp.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrderAsyncWebApp
{
    public class ShopHandler
    {
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private readonly IMediator _mediator;
        private readonly BlockingCollection<PlatformShop> _messageCollection = new BlockingCollection<PlatformShop>(5000);
        public ShopHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public void Enqueue(PlatformShop message)
        {
            if (_messageCollection.Count < _messageCollection.BoundedCapacity)
                _messageCollection.Add(message);
            if (_cancellationToken.IsCancellationRequested && !_messageCollection.IsCompleted)
                _messageCollection.CompleteAdding();
        }

        public void Producer(List<PlatformShop> shops)
        {
            int pageSize = 500;
            Setup();
            var producerManager = new TaskManager();
            producerManager.Setup(4);
            producerManager.Start((index) =>
            {
                var pageIndex = Convert.ToInt32(index) + 1;
                while (true)
                {
                    try
                    {
                        int pageCount = pageSize;
                        var modelList = GetModelList(pageIndex, pageSize, shops, ref pageCount);
                        CountdownEvent countd = new CountdownEvent(pageCount);
                        if (modelList == null)
                        {
                            break;
                        }
                        foreach (var model in modelList)
                        {
                            //自旋
                            while (!IsCanEnqueue)
                            {
                                Thread.Sleep(5 * 1000);
                            }
                            Enqueue(model);
                            countd.Signal();
                        }
                        countd.Wait();
                        break;
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            });
            producerManager.Wait();
            Console.WriteLine($"生产者完成任务{GetCount()}");
            Cancel();
            Consumer();
        }

        public void Consumer()
        {
            var consumerManager = new TaskManager();
            consumerManager.Setup(6);
            consumerManager.Start(() =>
            {
                if (GetCount() > 0)
                {
                    Dequeue((info) =>
                    {
                        try
                        {
                            ShopMain shop = new ShopMain(info);
                            var list = shop.Run();
                            Console.WriteLine($"{info.ShopId}店铺同步到订单数量:" +list?.Count.ToString());
                            if (list!=null&&list.Count>0)
                            {
                                WriteOrderCommand command = new WriteOrderCommand(list, info.ShopId,info.MerchantId,info.PPId);
                                _mediator.Send(command);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    });
                }
            });
            consumerManager.Wait();
            Console.WriteLine($"消费者线程完成任务{GetCount()}");
        }
        private List<PlatformShop> GetModelList(int pageIndex, int pageSize, List<PlatformShop> dict, ref int pageCount)
        {

            var res = dict.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            pageCount = res.Count();
            return res;
        }
        public void Dequeue(Action<PlatformShop> writeAction)
        {
            foreach (var message in _messageCollection.GetConsumingEnumerable())
            {
                writeAction(message);
                Thread.Sleep(10);
            }
        }
        public void Setup()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }
        public void Cancel()
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
                _cancellationTokenSource.Cancel(false);
            if (!_messageCollection.IsCompleted) _messageCollection.CompleteAdding();
        }
        public int GetCount()
        {
            return _messageCollection.Count;
        }
        public bool IsCanEnqueue
        {
            get
            {
                return _messageCollection.BoundedCapacity > _messageCollection.Count;
            }
        }
    }
}
