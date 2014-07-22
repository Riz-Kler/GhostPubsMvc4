using System;
using System.Collections;
using System.Xml.Serialization;

namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    [XmlRoot("urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class Sitemap
    {
        private ArrayList _map;

        public Sitemap()
        {
            _map = new ArrayList();
        }

        [XmlElement("url")]
        public Location[] Locations
        {
            get
            {
                var items = new Location[_map.Count];
                _map.CopyTo(items);
                return items;
            }
            set
            {
                if (value == null)
                    return;
                var items = value;
                _map.Clear();
                foreach (var item in items)
                    _map.Add(item);
            }
        }

        public int Add(Location item)
        {
            return _map.Add(item);
        }
    }

    // Items in the shopping list
    public class Location
    {
        public enum EChangeFrequency
        {
            Always,
            Hourly,
            Daily,
            Weekly,
            Monthly,
            Yearly,
            Never
        }

        [XmlElement("loc")]
        public string Url { get; set; }

        [XmlElement("changefreq")]
        public EChangeFrequency? ChangeFrequency { get; set; }

        [XmlElement("lastmod")]
        public DateTime? LastModified { get; set; }

        [XmlElement("priority")]
        public double? Priority { get; set; }

        public bool ShouldSerializeChangeFrequency()
        {
            return ChangeFrequency.HasValue;
        }

        public bool ShouldSerializeLastModified()
        {
            return LastModified.HasValue;
        }

        public bool ShouldSerializePriority()
        {
            return Priority.HasValue;
        }
    }
}