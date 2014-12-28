using System;
using System.Collections.Generic;
using System.Linq;

namespace Carnotaurus.GhostPubsMvc.Data.Models.ViewModels
{
    [Serializable]
    public class PageLinkKeyedCollection : List<KeyValuePair<string, PageLinkModel>>
    {
        public List<PageLinkModel> Links { get; set; }

        public PageLinkModel Last { get; set; }

        public KeyValuePair<string, PageLinkModel> NextSibling { get; set; }

        public PageLinkKeyedCollection(IEnumerable<KeyValuePair<string, PageLinkModel>> range, string key)
        {
            this.AddRange(range);

            Links = this
                .Where(x => x.Key.Equals(key))
                .Select(x => x.Value)
                .ToList();

            Last = Links.Last();

            NextSibling = FindNextSibling();
        }

        private KeyValuePair<string, PageLinkModel> FindNextSibling()
        {
            if (this == null) throw new ArgumentNullException("list");

            var findIndex = this.FindLastIndex(i => i.Value.Url == Last.Url);

            var nextIndex = findIndex + 1;

            var maxIndex = this.Count;

            if (nextIndex >= maxIndex)
            {
                nextIndex = 0;
            }

            var result = this[nextIndex];

            return result ;
        }

    }
}