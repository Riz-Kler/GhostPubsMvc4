using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using Carnotaurus.GhostPubsMvc.Common.Extensions;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;
using Carnotaurus.GhostPubsMvc.Data.Models.ViewModels;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Entities
{
    public class Org : IEntity
    {
        public Org()
        {
            //BookItems = new List<BookItem>();
            Notes = new List<Note>();
            Tags = new List<Tag>();
        }

        [NotMapped]
        public Boolean HasTriedAllApis
        {
            get
            {
                return (
                    LaTried & Tried);
            }
        }

        [NotMapped]
        public string CountyPath
        {
            get { return TownPath.BeforeLast(@"\"); }
        }


        [NotMapped]
        public string RegionPath
        {
            get { return CountyPath.BeforeLast(@"\"); }
        }


        [NotMapped]
        public string TownPath { get; set; }

        [NotMapped]
        public string Path
        {
            get
            {
                if (TownPath == null)
                {
                    TownPath = string.Empty;
                }

                var current = BuildPath(TownPath, Id.ToString(CultureInfo.InvariantCulture), TradingName);

                return current;
            }
        }

        [NotMapped]
        public string PostcodePrimaryPart
        {
            get
            {
                var prepare = Postcode.Trim();

                var output = String.Empty;

                if (prepare.Length > 2)
                {
                    output = prepare
                        .Substring(0, prepare.Length - 3)
                        .Trim();
                }

                return output;
            }
        }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime? Deleted { get; set; }
        public int? AddressTypeId { get; set; }
        public int? AuthorityId { get; set; }
        public int? ParentId { get; set; }
        public bool? TradingStatus { get; set; }
        public bool? HauntedStatus { get; set; }
        public string TradingName { get; set; }
        public string AlternateName { get; set; }
        public string SimpleName { get; set; }
        public string Locality { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Twitter { get; set; }
        public string Email { get; set; }
        public string Facebook { get; set; }
        public string Website { get; set; }
        public int? OsX { get; set; }
        public int? OsY { get; set; }
        public double? Lat { get; set; }
        public double? Lon { get; set; }
        public bool Tried { get; set; }
        public string GoogleMapData { get; set; }

        public bool LaTried { get; set; }
        public string LaData { get; set; }
        public string LaCode { get; set; }

        public virtual AddressType AddressType { get; set; }
        // this slows down retreival tragically
        //   public virtual ICollection<BookItem> BookItems { get; set; }
        public virtual Authority Authority { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }

        [NotMapped]
        public virtual String UncRelTownPath
        {
            get
            {
                var items = Authority.Lineage;

                var result = items.ExtractUnc();

                result = string.Format("{0}\\{1}", result, Town);

                return result;
            }
        }


        public int Id { get; set; }

        public String BuildPath(params String[] builder)
        {
            var output = String.Empty;

            foreach (var b in builder)
            {
                if (output == String.Empty)
                {
                    output = b.ToLower().SeoFormat();
                }
                else
                {
                    output = string.Format("{0}\\{1}", output, b.ToLower().SeoFormat());
                }
            }

            return output;
        }

        public PageLinkModel ExtractLink(String currentRoot)
        {
            var info = new PageLinkModel(currentRoot)
            {
                Id = Id,
                Text = TradingName,
                Title = string.Format("{0}, {1}", TradingName, Postcode),
                Unc = Path,
            };

            return info;
        }

        public PageLinkModel ExtractFullLink()
        {

            var result = Authority.Lineage;

            result.Add(Town);

            result.Add(Id.ToString(CultureInfo.InvariantCulture));

            result.Add(TradingName);

            var extractUnc = result.ExtractUnc();

            extractUnc = string.Format(@"http://www.ghostpubs.com/haunted-pubs/{0}", extractUnc);

            var info = new PageLinkModel
            {
                Id = Id,
                Text = TradingName,
                Title = string.Format("{0}, {1}", TradingName, Postcode),
                Unc = extractUnc
            };

            return info;

            return null;
        }
    }
}