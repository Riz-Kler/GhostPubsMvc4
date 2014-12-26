using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Carnotaurus.GhostPubsMvc.Common.Extensions;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;
using Carnotaurus.GhostPubsMvc.Data.Models.ViewModels;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Entities
{
    public class Authority : IEntity
    {
        public Authority()
        {
            Orgs = new List<Org>();
            Authoritys = new List<Authority>();
        }

        public string Name { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public int? Population { get; set; }
        public int? Hectares { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Deleted { get; set; }

        // recursive fix
        public int ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual Authority ParentAuthority { get; set; }

        public virtual ICollection<Org> Orgs { get; set; }

        public virtual ICollection<Authority> Authoritys { get; set; }

        [NotMapped]
        public string QualifiedName
        {
            get { return String.Format("{0} {1}", Name, Type.Replace("Met ", String.Empty)); }
        }

        [NotMapped]
        public string QualifiedNameDashified
        {
            get { return QualifiedName.Dashify(); }
        }

        [NotMapped]
        public bool IsRegion
        {
            get
            {
                var result = Type.ToLower() == "region";

                return result;
            }
        }

        [NotMapped]
        public bool IsDistrict
        {
            get
            {
                var result = Type.ToLower().Contains("district");

                return result;
            }
        }

        [NotMapped]
        public bool IsCounty
        {
            get
            {
                var result = Type.ToLower().Contains("county");

                return result;
            }
        }

        [NotMapped]
        public List<string> RegionalLineage
        {
            get
            {
                var list = Levels;

                list.Remove("England");
                list.Remove("England and Wales");
                list.Remove("Great Britain");
                list.Remove("United Kingdom");
                list.Remove("British Isles");

                return list;
            }
        }

        [NotMapped]
        public List<string> Levels
        {
            get
            {
                var result = LevelsAscending.ReverseItems();

                return result;
            }
        }

        [NotMapped]
        public List<string> LevelsAscending
        {
            get
            {
                var lastAncestor = this;

                var list = new List<string>();

                while (lastAncestor != null && lastAncestor.ParentId != 0)
                {
                    var currentAncestor = lastAncestor.ParentAuthority;

                    if (currentAncestor != null)
                    {
                        if (!currentAncestor.Name.IsNullOrEmpty())
                        {
                            list.Add(currentAncestor.Name);
                        }
                        else
                        {
                            break;
                        }
                    }

                    lastAncestor = currentAncestor;
                }

                return list;
            }
        }


        [NotMapped]
        public Boolean IsDerivedFromExcludedArea
        {
            get
            {
                var excluded = false;

                var lastAncestor = this;

                if (lastAncestor.IsExcluded) return true;

                while (lastAncestor != null
                    && lastAncestor.ParentId != 0)
                {
                    var currentAncestor = lastAncestor.ParentAuthority;

                    if (currentAncestor != null)
                    {
                        if (!currentAncestor.Name.IsNullOrEmpty()
                            && currentAncestor.IsExcluded)
                        {
                            excluded = true;
                        }
                        else
                        {
                            break;
                        }
                    }

                    lastAncestor = currentAncestor;
                }

                return excluded;
            }
        }

        [NotMapped]
        public bool HasHauntedOrgs
        {
            get
            {
                var result = CountHauntedOrgs.IsAboveZero();

                return result;
            }
        }

        [NotMapped]
        public int CountHauntedOrgs
        {
            get
            {
                var count = Authoritys.Any()
                    ? // how to get county and metropolian county 
                    Authoritys.Sum(s => s.Orgs.Count(x => x.HauntedStatus.HasValue && x.HauntedStatus.Value))
                    : // other ones
                    Orgs.Count(x => x.HauntedStatus.HasValue && x.HauntedStatus.Value);

                return count;
            }
        }

        [NotMapped]
        public double Density
        {
            get
            {
                var density = 0.0;

                if (!Population.HasValue || !Hectares.HasValue) return density;

                density = (double)Population / (double)Hectares;

                return density;
            }
        }

        [NotMapped]
        protected bool IsExcluded
        {
            get
            {
                // todo - dpc - this should be in the database?
                return Code != null && (Code == "JE" || Code == "IOM" || Code == "GUR");
                //return IsRegion && Authoritys.Any();
            }
        }

        public int Id { get; set; }

        public PageLinkModel ExtractNextLink()
        {
            if (QualifiedName.IsNullOrEmpty()) throw new ArgumentNullException("QualifiedName");

            Authority sibbling = null;

            // create a default
            var result = new PageLinkModel
            {
                Text = QualifiedName,
                Title = QualifiedName,
                Filename = QualifiedNameDashified
            };

            if (ParentAuthority != null)
            {
                var ints = ParentAuthority.Authoritys
                    .Where(h => h.HasHauntedOrgs)
                    .OrderBy(o => o.QualifiedName)
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

                    sibbling = ParentAuthority.Authoritys
                        .FirstOrDefault(x => x.Id == nextId);
                }
            }

            if (sibbling != null)
            {
                result = new PageLinkModel
                {
                    Text = sibbling.QualifiedName,
                    Title = sibbling.QualifiedName,
                    Filename = sibbling.QualifiedNameDashified
                };
            }

            return result;
        }
    }
}