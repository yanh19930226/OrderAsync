using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderAsyncWebApp.Models
{
    /// <summary>
    /// 分页控件
    /// dyq 2012-3-30
    /// 分页参数固定为pageindex ，使用post是需要引用外部脚本
    /// </summary>
    public static class HtmlPager
    {

        /// <summary>
        /// 分页 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IHtmlContent Pager(this IHtmlHelper htmlHelper, PageConfig config)
        {
            //每页数量不能为O
            if (config.PageSize == 0)
            {
                return new HtmlString("参数配置错误！每页数量不能为0");
            }

            StringBuilder html = new StringBuilder();
            html.Append("<div  " + config.DivClass + " > <ul>");
            if (config.TotalItemCount == 0 || config.TotalPages <= 1)
            {
                //如果只有一页
                html.Append(CreatePagerOnlyOne(config));
            }
            else
            {
                //生成分页数据
                html.Append(CreatePager(config));
            }
            html.Append("</ul></div>");
            return new HtmlString(html.ToString());
        }

        /// <summary>
        /// Ajax分页重载
        /// dyq 2012-3-30
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="pagerList"></param>
        /// <param name="formId"></param>
        /// <param name="pageNum"></param>
        /// <param name="isPost"></param>
        /// <returns></returns>
        public static IHtmlContent Pager<T>(this IHtmlHelper htmlHelper,
             PageList<T> pagerList, string formId = "form0", int pageNum = 10, bool isPost = true, string filePath = "~/Views/Shared/Pager.cshtml")
        {
            var task = (htmlHelper.PartialAsync(
                filePath,
                new PageConfig
                {
                    TotalItemCount = pagerList.PagerModel.Count,
                    PageSize = pagerList.PagerModel.PageSize,
                    CurrentPageIndex = pagerList.PagerModel.PageIndex,
                    PageNum = pageNum,
                    FormId = formId,
                    IsPost = isPost
                }));
            task.Wait();
            return task.Result;
            /*

            return Pager(htmlHelper,
                new PageConfig
                {
                    TotalItemCount = pagerList.PagerModel.Count,
                    PageSize = pagerList.PagerModel.PageSize,
                    CurrentPageIndex = pagerList.PagerModel.PageIndex,
                    PageNum = pageNum,
                    FormId = formId,
                    IsPost = isPost
                });
            */
        }

        #region 内部方法

        /// <summary>
        /// 生成分页代码
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private static string CreatePager(PageConfig config)
        {
            var _Output = new StringBuilder();

            //处理首页链接
            _Output.AppendFormat("<li><a {0}  {1} >&nbsp;Index&nbsp;</a>", config.LabelClass, PageJump(config, 1));

            //处理上一页
            _Output.AppendFormat("<li><a {0} {1} >Up&nbsp;</a></li>",
                config.CurrentPageIndex == 1 ? config.NotLabelClass : config.LabelClass,
                config.CurrentPageIndex == 1 ? "" : PageJump(config, config.CurrentPageIndex - 1)
             );

            //处理前面的...
            if (config.TotalPages > config.PageNum && config.CurrentPageIndex > config.PageNum / 2 + 1)
            {
                _Output.AppendFormat("<li><a {0} {1} >...</a></li>",
                    config.LabelClass,
                    PageJump(config, config.CurrentPageIndex - config.HalfPagerNum - 1)
                );
            }

            //循环输出页码
            for (int i = config.IndexPagerNum; i < config.IndexPagerNum + config.PageNum && i <= config.TotalPages; i++)
            {
                if (i == config.CurrentPageIndex)
                {
                    _Output.AppendFormat("<li {1} ><a >&nbsp;{0}&nbsp;</a></li>", i, config.CurrentpageClass);
                }
                else
                {
                    _Output.AppendFormat("<li><a {0} {1} >&nbsp;{2}&nbsp;</a></li>", config.PageClass, PageJump(config, i), i);
                }
            }

            //处理后面那个...
            if (config.TotalPages > config.PageNum && config.CurrentPageIndex < config.TotalPages - (config.HalfPagerNum) + 1)
            {
                _Output.AppendFormat("<li><a {0} {1} >...</a></li>",
                    config.LabelClass,
                    PageJump(config, config.IndexPagerNum + config.PageNum)
                );
            }

            //处理下一页
            _Output.AppendFormat("<li><a {0} {1} >&nbsp;Down&nbsp;</a></li>",
                config.CurrentPageIndex == config.TotalPages ? config.NotLabelClass : config.LabelClass,
                config.CurrentPageIndex == config.TotalPages ? "" : PageJump(config, config.CurrentPageIndex + 1)
            );

            //最后一页链接
            _Output.AppendFormat("<li><a {0} {1} >&nbsp;End&nbsp;</a></li>", config.LabelClass, PageJump(config, config.TotalPages));

            if (config.IsPost)
            {
                _Output.AppendLine(UpdateHidden(config, config.CurrentPageIndex));
            }
            return _Output.ToString();
        }


        /// <summary>
        /// 生成分页代码 
        /// 只有一页或者没有数据时调用显示
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private static string CreatePagerOnlyOne(PageConfig config)
        {
            const string template = "<li><a {0} >Index</a></li>&nbsp;<li><a {0} >Up</a></li>&nbsp;<li><a {1} >1</a></li>&nbsp;<li><a {0} >Down</a></li>&nbsp;<li><a {0} >End</a></li> ";
            if (config.OnlyPage)
            {
                return string.Format(template, config.NotLabelClass, config.CurrentpageClass);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 生成隐藏域 
        /// 只在表单为POST时调用
        /// </summary>
        /// <param name="config"></param>
        /// <param name="pageindex"></param>
        /// <returns></returns>
        private static string UpdateHidden(PageConfig config, int pageindex)
        {
            const string _Template = " UpdateHidden('{0}','{1}','{2}'); ";
            var _Output = new StringBuilder();

            _Output.Append(" <script>");
            // _Output.Append(" $(function ()");
            //_Output.Append("   {");
            //先插入一个PageIndex
            _Output.AppendFormat(_Template, config.FormId, "pageIndex", "1");

            //插入一个HPageIndex
            _Output.AppendFormat(_Template, config.FormId, "HpageIndex", pageindex.ToString());

            //然后插入一个随机的hidden 防止缓存
            _Output.AppendFormat(_Template, config.FormId, "Hidden_Ajax_Time", DateTime.Now.Ticks.ToString());
            //_Output.Append("   }");
            //_Output.Append(" );");
            _Output.Append(" </script>");

            return _Output.ToString();
        }

        /// <summary>
        /// 输出跳转代码
        /// </summary>
        /// <returns></returns>
        private static string PageJump(PageConfig config, int pageindex)
        {
            return string.Format(config.JumpLink, pageindex);
        }

        #endregion
    }


    /// <summary>
    /// 分页配置
    /// </summary>
    public class PageConfig
    {

        #region 必须传入参数

        int _CurrentPageIndex = 1;
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPageIndex
        {
            get { return _CurrentPageIndex; }
            set { _CurrentPageIndex = value; }
        }

        /// <summary>
        /// 记录总数
        /// </summary>
        public int TotalItemCount
        {
            get;
            set;
        }

        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize
        {
            get;
            set;
        }

        #endregion

        #region 动态属性，根据其他参数运算的结果

        /// <summary>
        /// 
        /// </summary>
        int _HalfPagerNum = -1;

        /// <summary>
        /// PageNum的一半，以下很多公式会用到，所以预先计算
        /// </summary>
        public int HalfPagerNum
        {
            get
            {
                if (_HalfPagerNum == -1)
                {
                    _HalfPagerNum = PageNum / 2;
                }
                return _HalfPagerNum;
            }
        }

        /// <summary>
        /// /
        /// </summary>
        int _IndexPagerNum = -1;
        /// <summary>
        /// 输出页面开始的位置
        /// </summary>
        public int IndexPagerNum
        {

            get
            {
                if (_IndexPagerNum == -1)
                {
                    int _Index;
                    if (CurrentPageIndex < HalfPagerNum + 1)
                    {
                        _Index = 1;
                    }
                    else if (CurrentPageIndex > TotalPages - HalfPagerNum)
                    {
                        _Index = TotalPages - PageNum + 1;
                    }
                    else
                    {
                        _Index = CurrentPageIndex - HalfPagerNum;
                    }
                    _Index = _Index < 1 ? 1 : _Index;
                    _IndexPagerNum = _Index;
                }
                return _IndexPagerNum;
            }

        }


        int _TotalPages = -1;
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages
        {
            get
            {
                int temp = TotalItemCount / PageSize;
                _TotalPages = TotalItemCount % PageSize == 0 ? temp : ++temp;
                return _TotalPages;
            }
        }


        string _JumpLink = "";
        /// <summary>
        /// 跳转模版
        /// </summary>
        public string JumpLink
        {
            get
            {
                if (_JumpLink.Length == 0)
                {
                    if (IsPost)
                    {
                        //如果是post
                        _JumpLink = "href='#' onclick=\"return AjaxToPage('" + FormId + "',{0})\"";
                    }
                    else
                    {

                    }
                }
                return _JumpLink;
            }

        }
        #endregion

        #region 可配置参数

        string _CurrentpageClass = "class='active'";
        /// <summary>
        /// 当前页样式
        /// </summary>
        public string CurrentpageClass
        {
            get { return _CurrentpageClass; }
            set { _CurrentpageClass = value; }
        }


        string _PageClass = "paginate_button";
        /// <summary>
        /// 非当前页样式
        /// </summary>
        public string PageClass
        {
            get { return _PageClass; }
            set { _PageClass = value; }
        }


        string _DivClass = "class='pagination'";
        /// <summary>
        /// 最外层div样式名称
        /// </summary>
        public string DivClass
        {
            get { return _DivClass; }
            set { _DivClass = value; }
        }


        string _LabelClass = "class=''";
        /// <summary>
        ///  首页 上一页  下一页  尾页 可用时候样式
        /// </summary>
        public string LabelClass
        {
            get { return _LabelClass; }
            set { _LabelClass = value; }
        }


        string _NotLabelClass = "class=''";
        /// <summary>
        /// 首页 上一页  下一页  尾页 不可用时样式
        /// </summary>
        public string NotLabelClass
        {
            get { return _NotLabelClass; }
            set { _NotLabelClass = value; }
        }


        bool _OnlyPage = true;
        /// <summary>
        /// 只有一页是否显示
        /// </summary>
        public bool OnlyPage
        {
            get { return _OnlyPage; }
            set { _OnlyPage = value; }
        }


        string _FormId = "from0";
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FormId
        {
            get { return _FormId; }
            set { _FormId = value; }
        }


        bool _IsPost = true;
        /// <summary>
        /// 提交方式
        /// true==Post   false==get
        /// </summary>
        public bool IsPost
        {
            get { return _IsPost; }
            set { _IsPost = value; }
        }


        int _PageNum = 5;
        /// <summary>
        /// 页码显示数量
        /// </summary>
        public int PageNum
        {
            get { return _PageNum; }
            set { _PageNum = value; }
        }
        #endregion
    }
}
