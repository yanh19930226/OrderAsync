using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAsyncWebApp.Models
{

    public class ShopState
    {
        public DateTime RunTime { get; set; }

        public string RunText { get; set; }

        public string ShopId { get; set; }

    }
    public enum Types
    {
        [Description("Default")]
        Default = -1,

        [Description("Shopify")]
        Shopify = 1,

        [Description("Xshoppy")]
        Xshoppy = 2,

        [Description("Shopbase")]
        Shopbase = 3,

        [Description("NewWeb")]
        NewWeb = 5,

        [Description("店匠")]
        Shoplaza = 4,

        [Description("Funpinpin")]
        Funpinpin = 6,

        [Description("Funpinpin2")]
        Funpinpin2 = 7
    }

    public enum SyncTypes
    {
        [Description("Default")]
        Default = 0,
        [Description("Failed")]
        Failed = 2,

        [Description("Successed")]
        Successed = 1,
    }


    public enum Status
    {
        [Description("Default")]
        Default = -1,

        [Description("正常")]
        Normal = 1,

        [Description("异常")]
        Error = 2,

        [Description("订单已关闭")]
        Close = 0,
    }


    public enum ErrorType
    {
        [Description("全部")]
        Default = -1,

        [Description("等待重试")]
        Wait = 0,

        [Description("超重")]
        OverWeight = 1,

        [Description("收件人信息不正确")]
        RecipientError = 2,

        [Description("地址信息不正确")]
        AddressError = 3,


        [Description("邮编不正确")]
        ZipCodeError = 4,

        [Description("店铺物流未关联")]
        NoConfigured = 5,

        [Description("争议")]
        Dispute = 6,

        [Description("SKU未匹配")]
        SkuError = 7,

        [Description("物流未匹配")]
        ExpressError = 8,


        [Description("请忽略异常")]
        Skip = 9,

        [Description("无异常")]
        No = 10,

        [Description("手机或者邮箱不正确")]
        PhoneError = 11,


        [Description("金额过高或者过低，可设置忽略")]
        MoenyError = 12,


        [Description("商品成本超过营业额50%")]
        CostFreeError = 13,

        [Description("部分退款")]
        PartClose = 14,
    }


    public enum IsPack
    {

        [Description("未包装")]
        UnPack = 0,

        [Description("已包装")]
        PackFinsh = 1,
    }

    public enum IsComplete
    {
        [Description("Default")]
        Default = -1,

        [Description("未拣货")]
        UnComplete = 0,

        [Description("拣货完成")]
        CompleteFinsh = 1,
    }

    public enum BooleanType
    {
        [Description("默认")]
        UnKnow = -1,

        [Description("否")]
        No = 0,

        [Description("是")]
        Yes = 1,
    }
}
