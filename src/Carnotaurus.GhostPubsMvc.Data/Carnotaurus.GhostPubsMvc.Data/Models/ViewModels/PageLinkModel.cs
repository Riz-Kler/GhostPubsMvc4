using System;
using System.Collections.Generic;
using Carnotaurus.GhostPubsMvc.Common.Extensions;
using Humanizer;

namespace Carnotaurus.GhostPubsMvc.Data.Models.ViewModels
{
    public class PageLinkModel
    {
        private readonly bool _isStandardLink;

        public PageLinkModel()
        {
        }

        public PageLinkModel(bool isStandardLink)
        {
            _isStandardLink = isStandardLink;
        }

        public String Url
        {
            get
            {
                if (Filename.IsNullOrEmpty()) return String.Empty;

                var pattern = _isStandardLink //_isGeoLink.Contains(@"\uk\")
                    ? "http://www.ghostpubs.com/haunted-pubs/uk/{0}.html"
                    : "http://www.ghostpubs.com/haunted-pubs/{0}.html";

                var url = String.Format(pattern, Filename.Replace("\\", "/"));

                return url;
            }
        }

        public String ExternalUrl { get; set; }

        public String Filename { get; set; }

        public String Title { get; set; }

        public String Text { get; set; }

        public String TotalInWords
        {
            get { return Total.ToWords(); }
        }

        public Int32 Total { get; set; }

        public Int32 Id { get; set; }

        public List<PageLinkModel> Links { get; set; }
    }
}