using System;
using System.Collections.Generic;
using System.Globalization;
using Carnotaurus.GhostPubsMvc.Common.Extensions;

namespace Carnotaurus.GhostPubsMvc.Data.Models.ViewModels
{
    [Serializable]
    public class PageLinkModel
    {
        private readonly String _currentRoot = String.Empty;

        public PageLinkModel()
        {
        }

        public PageLinkModel(String currentRoot)
        {
            _currentRoot = currentRoot;
        }

        public String Url
        {
            get
            {
                if (Filename.IsNullOrEmpty()) return String.Empty;

                var pattern = _currentRoot.Contains(@"\uk\")
                    ? "http://www.ghostpubs.com/haunted-pubs/uk/{0}.html"
                    : "http://www.ghostpubs.com/haunted-pubs/{0}.html";

                var url = String.Format(pattern, Filename.Replace("\\", "/"));

                return url.Dashify().ToLower();
            }
        }
         
        public String Filename { get; set; }

        public String Title { get; set; }

        public String Text { get; set; }

        public Int32 Id { get; set; }

        public List<PageLinkModel> Links { get; set; }
    }
}