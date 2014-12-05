using System.Collections.Generic;

namespace Carnotaurus.GhostPubsMvc.Common.Result
{
    public class QueryResult<T>
    {
        /// <summary>
        ///     The Items returned from your query
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        ///     The Page Number That This result set releates to. First Page should be 1
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        ///     The Page Size Requested
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        ///     Total number of items on the server matching the query
        /// </summary>
        public int ItemCount { get; set; }

        /// <summary>
        ///     The total number of pages available on the server
        /// </summary>
        public int PageCount
        {
            get
            {
                if (PageSize == 0) return 0;

                var pages = (ItemCount/PageSize);

                if ((ItemCount%PageSize) > 0) pages++;

                return pages;
            }
        }
    }
}