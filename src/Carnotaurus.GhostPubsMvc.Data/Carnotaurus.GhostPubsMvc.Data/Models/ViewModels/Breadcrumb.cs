﻿using System;

namespace Carnotaurus.GhostPubsMvc.Data.Models.ViewModels
{
    [Serializable]
    public class Breadcrumb
    {
        public PageLinkModel Region { get; set; }
        public PageLinkModel County { get; set; }
        public PageLinkModel Locality { get; set; }
        public PageLinkModel Pub { get; set; }
    }
}