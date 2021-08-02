using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAsyncWebApp.Models
{
    public class PageList<T> : List<T>
    {
        public PageModel PagerModel { get; set; }
    }

    /// <summary>
    /// 分页方法
    /// </summary>
    public class PageModel
    {
        private int _PageIndex = 1;

        public int PageIndex
        {
            get { return _PageIndex; }
            set { _PageIndex = value; }
        }

        private int _PageSize = 100;

        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize
        {
            get { return _PageSize; }
            set { _PageSize = value; }
        }

        private int _PageCount = 1;

        /// <summary>
        /// 页数
        /// </summary>
        public int PageCount
        {
            get { return _PageCount; }
            set { _PageCount = value; }
        }


        /// <summary>
        /// 开始索引
        /// </summary>
        public int StartIndex
        {
            get { return _PageIndex * _PageCount + 1; }
        }


        private int _Count = 0;

        /// <summary>
        /// 总记录
        /// </summary>
        public int Count
        {
            get { return _Count; }
            set { _Count = value; }
        }

    }
}
