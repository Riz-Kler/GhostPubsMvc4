﻿using System;
using System.Collections.Generic;
using System.Linq;
using Carnotaurus.GhostPubsMvc.Common.Bespoke.Enumerations;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;
using Castle.Core.Internal;

namespace Carnotaurus.GhostPubsMvc.Data.Models.ViewModels
{
    [Serializable]
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

        public String MetaDescription { get; set; }
 
        public String ArticleDescription { get; set; }

        public String Link { get; set; }

        public String Tag { get; set; }

        public List<PageLinkModel> OtherNames { get; set; }

        public List<PageLinkModel> PageLinks { get; set; }

        public String Unc { get; set; }

        public String Url
        {
            get
            {
                var result = String.Concat(Unc, @"\", "detail.html").ToLower();

                if (!_currentRoot.IsNullOrEmpty())
                {
                    result = String.Format("http://www.ghostpubs.com/haunted-pub{0}",
                        result.Replace(_currentRoot.ToLower(), String.Empty).Replace("\\", "/"));
                }
                
                return result;
            }
        }

        public KeyValuePair<String, String> Parent { get; set; }

        public List<String> Tags { get; set; }

        public Int32 Total { get; set; }

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
                    Url.ToLower(),
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