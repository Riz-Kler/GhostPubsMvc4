using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Carnotaurus.GhostPubsMvc.Common.Extensions;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;
using Carnotaurus.GhostPubsMvc.Data.Models.ViewModels;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Entities
{
    [Table("Org", Schema = "Organisation")]
    public class Org : IEntity
    {
        public Org()
        {
            this.Tags = new HashSet<Tag>();
        }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime? Deleted { get; set; }

        public int? AddressTypeId { get; set; }
        public int? AuthorityId { get; set; }
        public int? ParentId { get; set; }
        public bool TradingStatus { get; set; }
        public bool HauntedStatus { get; set; }
        public string TradingName { get; set; }
        public string SimpleName { get; set; }
        public string Locality { get; set; }
        public string Postcode { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Twitter { get; set; }
        public string Email { get; set; }
        public string Facebook { get; set; }
        public string Website { get; set; }
        public double? Lat { get; set; }
        public double? Lon { get; set; }
        public bool Tried { get; set; }
        public string GoogleMapData { get; set; }

        public bool LaTried { get; set; }
        public string LaData { get; set; }
        public string LaCode { get; set; }
        public string Description { get; set; }

        public virtual AddressType AddressType { get; set; }
        public virtual Authority Authority { get; set; }
         public virtual ICollection<Tag> Tags { get; set; }

        // this slows down retreival tragically
        //   public virtual ICollection<BookItem> BookItems { get; set; }
        public int Id { get; set; }

        #region Unmapped properties

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

        [NotMapped]
        public string QualifiedLocalityDashified
        {
            get
            {
                var result = QualifiedLocality.Clean();

                return result;
            }
        }

        [NotMapped]
        public string QualifiedLocality
        {
            get
            {
                var result = Locality.In(Authority.QualifiedName);

                return result;
            }
        }

        [NotMapped]
        public string SeoDescription
        {
            get
            {
                // var result = Notes.Select(x => x.Text).JoinWithSpace();
                var result = Description;

                return result.SeoMetaDescriptionTruncate();
            }
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
        public List<string> Sections
        {
            get
            {
                var sections = new List<string>();

                sections.AddRange(NameExtended);
                sections.Add(Authority.ParentAuthority.Name);
                sections.AddRange(Authority.ParentAuthority.LevelsAscending);
                sections.Add("Haunted Pubs");
                sections.Add("Inns");
                sections.Add("Hotels");
                sections.AddRange(Tags.Select(x => x.Feature.Name));

                return sections.ToList();
            }
        }

        [NotMapped]
        public string Title
        {
            get
            {
                var items = NameExtended;

                var result = string.Format("{0} | {1} Haunted, {2}", items.JoinWithComma(), TradingName, PostcodePrimaryPart);

                return result;
            }
        }

        [NotMapped]
        public string JumboTitle
        {
            get
            {
                var items = NameExtended;

                var result = string.Format("{0}", items.JoinWithComma());

                return result;
            }
        }

        [NotMapped]
        public List<string> NameExtended
        {
            get
            {
                var items = new List<String>();

                items.AddIf(TradingName);

                items.AddIf(Locality);

                items.AddIf(Authority.Name);
                return items;
            }
        }

        [NotMapped]
        public string Filename
        {
            get
            {
                var dash = string.Format("{0} {1} {2}", Id, TradingName, Locality).Clean();

                return dash;
            }
        }

        [NotMapped]
        public string GeoPath
        {
            get
            {
                var result = string.Empty;

                try
                {
                    result = string.Format("{0}\\{1}", Authority.Levels.JoinWithBackslash(), Authority.QualifiedName);
                }
                catch (Exception ex)
                {
                    var q = ex.Message;
                }

                return result;
            }
        }

        [NotMapped]
        public bool IsHauntedPub
        {
            get
            {
                var result = HauntedStatus
                             && Locality != null
                             && AddressTypeId == 1;

                return result;
            }
        }

        // 
        [NotMapped]
        public bool IsOutsideUnitedKingdom
        {
            get
            {
                var result = Authority.IsOutsideUnitedKingdom;

                return result;
            }
        }

        #endregion Unmapped properties

        #region Methods

        public PageLinkModel ExtractLink()
        {
            var info = new PageLinkModel
            {
                Id = Id,
                Text = TradingName,
                Title = string.Format("{0}, {1}", TradingName, Postcode),
                Filename = Filename,
            };

            return info;
        }

        public PageLinkModel ExtractNextLink()
        {
            if (TradingName.IsNullOrEmpty()) throw new ArgumentNullException("TradingName");

            Org sibbling = null;

            var result = new PageLinkModel
            {
                Text = TradingName,
                Title = TradingName,
                Filename = Filename
            };

            var ints = Authority.Orgs
                .Where(h => h.HauntedStatus)
                .OrderBy(o => o.QualifiedLocality)
                .Select(s => s.Id).ToList();

            var nextId = ints.FindNext(Id);

            if (nextId.HasValue)
            {
                sibbling = Authority.Orgs
                    .FirstOrDefault(x => x.Id == nextId.Value);
            }

            if (sibbling != null)
            {
                result = new PageLinkModel
                {
                    Text = sibbling.TradingName,
                    Title = sibbling.TradingName,
                    Filename = sibbling.Filename
                };
            }

            return result;
        }

        #endregion Methods
    }
}