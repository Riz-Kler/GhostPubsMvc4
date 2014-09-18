﻿using System;
using System.Collections.Generic;
using Carnotaurus.GhostPubsMvc.Common.Extensions;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;
using Humanizer;

namespace Carnotaurus.GhostPubsMvc.Data.Models.ViewModels
{
    public class PageLinkModel
    {
        private readonly String _currentRoot = String.Empty;

        public PageLinkModel(String currentRoot)
        {
            _currentRoot = currentRoot;
        }

        public String Url
        {
            get
            {
                var fullFilePath = String.Concat(Unc, @"\", "detail.html");

                if (!_currentRoot.IsNullOrEmpty())
                {
                    var url = String.Format("http://www.ghostpubs.com/haunted_pub{0}",
                        fullFilePath.Replace(_currentRoot, String.Empty).Replace("\\", "/"));

                    return url.Underscore();
                }

                return String.Empty;
            }
        }

        public String Unc { get; set; }

        public String Title { get; set; }

        public String Text { get; set; }

        public Int32 Id { get; set; }

        public IEnumerable<Org> Links { get; set; }
    }
}