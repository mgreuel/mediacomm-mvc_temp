using System;

namespace MediaCommMvc.Web.Infrastructure
{public class PagingParameters
    {
        public int CurrentPage { get; set; }

        public int NumberOfPages
        {
            get
            {
                if (this.PageSize <= 0)
                {
                    return 0;
                }

                return (int)Math.Ceiling(this.TotalCount / (decimal)this.PageSize);
            }
        }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public override string ToString()
        {
            return string.Format("CurrentPage: '{0}', PageSize: '{1}', TotalCount: '{2}'", this.CurrentPage, this.PageSize, this.TotalCount);
        }
    }
}