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
        public string PostalTown { get; set; }
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
        public virtual Authority Authority { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
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
                var result = QualifiedLocality.Dashify();

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
        public string DescriptionFromNotes
        {
            get
            {
                var result = Notes.Select(x => x.Text).JoinWithSpace();

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

                var result = string.Format("{0} | {1} Ghost", items.JoinWithComma(), TradingName);

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

                items.AddIf(PostalTown);

                items.AddIf(Authority.Name);
                return items;
            }
        }

        [NotMapped]
        public string Filename
        {
            get
            {
                var dash = string.Format("{0} {1} {2}", Id, TradingName, Locality).Dashify();

                return dash;
            }
        }

        [NotMapped]
        public string GeoPath
        {
            get { return Authority.Levels.JoinWithBackslash(); }
        }

        #endregion Unmapped properties

        #region Methods

        public PageLinkModel ExtractLink(String currentRoot)
        {
            if (currentRoot == null) throw new ArgumentNullException("currentRoot");

            var info = new PageLinkModel
            {
                Id = Id,
                Text = TradingName,
                Title = string.Format("{0}, {1}", TradingName, Postcode),
                Filename = Filename,
            };

            return info;
        }


        public PageLinkModel ExtractFullLink()
        {
            var result = Authority.Levels;

            result.Add(Locality);

            result.Add(Id.ToString(CultureInfo.InvariantCulture));

            result.Add(TradingName);

            var extractFilename = result.ExtractFilename();

            extractFilename = string.Format(@"http://www.ghostpubs.com/haunted-pubs/{0}", extractFilename);

            var info = new PageLinkModel
            {
                Id = Id,
                Text = TradingName,
                Title = string.Format("{0}, {1}", TradingName, Postcode),
                Filename = extractFilename
            };

            return info;
        }


        public PageLinkModel GetNextLink()
        {
            if (TradingName.IsNullOrEmpty()) throw new ArgumentNullException("TradingName");

            Org sibbling = null;

            // todo - dpc - create a default
            var result = new PageLinkModel
            {
                Text = TradingName,
                Title = TradingName,
                Filename = Filename
            };

            var ints = Authority.Orgs
                .Where(h => h.HauntedStatus.HasValue && h.HauntedStatus.Value)
                .OrderBy(o => o.QualifiedLocality)
                .Select(s => s.Id).ToList();

            if (ints.Count() > 1)
            {
                var findIndex = ints.FindIndex(i => i == Id);

                var nextIndex = findIndex + 1;

                var maxIndex = ints.Count;

                if (nextIndex == maxIndex)
                {
                    nextIndex = 0;
                }

                var nextId = ints[nextIndex];

                sibbling = Authority.Orgs
                    .FirstOrDefault(x => x.Id == nextId);
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