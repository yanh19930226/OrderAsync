using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class HtmlHelperViewExtensions
    {
#pragma warning disable CS8625 // 无法将 null 文本转换为不可为 null 的引用类型。
        public static IHtmlContent Action(this IHtmlHelper helper, string action, object parameters = null)
#pragma warning restore CS8625 // 无法将 null 文本转换为不可为 null 的引用类型。
        {
            var controller = (string)helper.ViewContext.RouteData.Values["controller"];

            return Action(helper, action, controller, parameters);
        }

#pragma warning disable CS8625 // 无法将 null 文本转换为不可为 null 的引用类型。
        public static IHtmlContent Action(this IHtmlHelper helper, string action, string controller, object parameters = null)
#pragma warning restore CS8625 // 无法将 null 文本转换为不可为 null 的引用类型。
        {
            var area = (string)helper.ViewContext.RouteData.Values["area"];

            return Action(helper, action, controller, area, parameters);
        }

#pragma warning disable CS8625 // 无法将 null 文本转换为不可为 null 的引用类型。
        public static IHtmlContent Action(this IHtmlHelper helper, string action, string controller, string area, object parameters = null)
#pragma warning restore CS8625 // 无法将 null 文本转换为不可为 null 的引用类型。
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (controller == null)
                throw new ArgumentNullException("controller");


            var task = RenderActionAsync(helper, action, controller, area, parameters);

            return task.Result;
        }

#pragma warning disable CS8625 // 无法将 null 文本转换为不可为 null 的引用类型。
        private static async Task<IHtmlContent> RenderActionAsync(this IHtmlHelper helper, string action, string controller, string area, object parameters = null)
#pragma warning restore CS8625 // 无法将 null 文本转换为不可为 null 的引用类型。
        {
            // fetching required services for invocation
            var serviceProvider = helper.ViewContext.HttpContext.RequestServices;
            var actionContextAccessor = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IActionContextAccessor>();
            var httpContextAccessor = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IHttpContextAccessor>();
            var actionSelector = serviceProvider.GetRequiredService<IActionSelector>();

            // creating new action invocation context
            var routeData = new AspNetCore.Routing.RouteData();
            foreach (var router in helper.ViewContext.RouteData.Routers)
            {
                routeData.PushState(router, null, null);
            }
            routeData.PushState(null, new RouteValueDictionary(new { controller = controller, action = action, area = area, parameters = parameters }), null);
            routeData.PushState(null, new RouteValueDictionary(parameters ?? new { }), null);

            //get the actiondescriptor
            RouteContext routeContext = new RouteContext(helper.ViewContext.HttpContext) { RouteData = routeData };
            var candidates = actionSelector.SelectCandidates(routeContext);
            var actionDescriptor = actionSelector.SelectBestCandidate(routeContext, candidates);

            var originalActionContext = actionContextAccessor.ActionContext;
            var originalhttpContext = httpContextAccessor.HttpContext;
            try
            {
                var newHttpContext = serviceProvider.GetRequiredService<IHttpContextFactory>().Create(helper.ViewContext.HttpContext.Features);
                if (newHttpContext.Items.ContainsKey(typeof(IUrlHelper)))
                {
                    newHttpContext.Items.Remove(typeof(IUrlHelper));
                }
                newHttpContext.Response.Body = new MemoryStream();
                var actionContext = new ActionContext(newHttpContext, routeData, actionDescriptor);
                actionContextAccessor.ActionContext = actionContext;
                var invoker = serviceProvider.GetRequiredService<IActionInvokerFactory>().CreateInvoker(actionContext);
                await invoker.InvokeAsync();
                newHttpContext.Response.Body.Position = 0;
                using (var reader = new StreamReader(newHttpContext.Response.Body))
                {
                    return new HtmlString(reader.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                return new HtmlString(ex.Message);
            }
            finally
            {
                actionContextAccessor.ActionContext = originalActionContext;
                httpContextAccessor.HttpContext = originalhttpContext;
                if (helper.ViewContext.HttpContext.Items.ContainsKey(typeof(IUrlHelper)))
                {
                    helper.ViewContext.HttpContext.Items.Remove(typeof(IUrlHelper));
                }
            }
        }

        public static Dictionary<string, string> GetCurrency()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("USD", "美元");
            dic.Add("CHF", "瑞士法郎");
            dic.Add("NZD", "新西兰元");
            dic.Add("EUR", "欧元");
            dic.Add("CAD", "加拿大元");
            dic.Add("HKD", "港币");
            dic.Add("GBP", "英镑");
            dic.Add("JPY", "日元");
            dic.Add("SGD", "新加坡元");
            dic.Add("AUD", "澳大利亚元");
            dic.Add("CNY", "人民币");
            return dic;
        }
    }
}
