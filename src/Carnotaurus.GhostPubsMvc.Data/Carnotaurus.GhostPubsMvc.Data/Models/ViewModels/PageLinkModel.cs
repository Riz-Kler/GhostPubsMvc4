using System;
using System.Collections.Generic;
using System.Globalization;
using Carnotaurus.GhostPubsMvc.Common.Extensions;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;
using Humanizer;

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
                var fullFilePath = String.Concat(Unc, @"\", "detail.html").ToLower();

                if (!_currentRoot.IsNullOrEmpty())
                {
                    var url = String.Format("http://www.ghostpubs.com/haunted-pub{0}",
                        fullFilePath.Replace(_currentRoot.ToLower(), String.Empty).Replace("\\", "/"));

                    return url.Underscore().Hyphenate().ToLower();
                }

                return fullFilePath.Replace("\\", "/").Underscore().Hyphenate().ToLower();  
            }
        }
  
        public String Unc { get; set; }

        public String Title { get; set; }

        public String Text { get; set; }

        public Int32 Id { get; set; }
 
        public string ControlId {
            get
            {
                return string.Concat("collapse", Id.ToString(CultureInfo.InvariantCulture));
            }
        }

        public List<PageLinkModel> Links { get; set; }
    }
}