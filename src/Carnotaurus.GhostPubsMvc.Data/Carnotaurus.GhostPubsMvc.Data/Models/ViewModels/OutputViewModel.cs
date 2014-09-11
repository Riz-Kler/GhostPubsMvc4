using System;
using System.Collections.Generic;
using Carnotaurus.GhostPubsMvc.Common.Bespoke.Enumerations;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data.Models.ViewModels
{
    public class OutputViewModel : IOutputViewModel
    {
        private readonly String _currentRoot = String.Empty;

        public OutputViewModel(String currentRoot)
        {
            PageLinks = new List<PageLinkModel>();
            Tags = new List<String>();
            _currentRoot = currentRoot;
        }

        public String Lat { get; set; }

        public String Lon { get; set; }

        public Breadcrumb Lineage { get; set; }

        public String Description { get; set; }

        public String Link { get; set; }

        public String Tag { get; set; }

        public List<PageLinkModel> OtherNames { get; set; }

        public List<PageLinkModel> PageLinks { get; set; }

        public String Unc { get; set; }

        public String Url
        {
            get
            {
                var fullFilePath = String.Concat(Unc, @"\", "detail.html");

                var url = String.Format("http://www.ghostpubs.com/haunted_pub{0}",
                    fullFilePath.Replace(_currentRoot, String.Empty).Replace("\\", "/"));

                return url;
            }
        }

        public KeyValuePair<String, String> Parent { get; set; }

        public List<String> Tags { get; set; }

        public Int32? Total { get; set; }

        public String Priority { get; set; }

        public OutputViewModel Previous { get; set; }

        public String SitemapItem
        {
            get
            {
                //<url>
                //<loc>http://www.mypubguide.com/dne/app10/pages/pub/pub-95584.aspx</loc>
                //<lastmod>2012-01-12T22:06:02+00:00</lastmod>
                //<changefreq>daily</changefreq>
                //<priority>0.9</priority>
                //</url>

                const string pattern =
                    "<url><loc>{0}</loc><lastmod>{1}</lastmod><changefreq>{2}</changefreq><priority>{3}</priority></url>";

                var output = String.Format(pattern,
                    Url,
                    DateTime.UtcNow.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'+00:00'"),
                    "daily",
                    Priority
                    );

                return output;
            }
        }

        public String JumboTitle { get; set; }

        public PageTypeEnum Action { get; set; }
    }
}