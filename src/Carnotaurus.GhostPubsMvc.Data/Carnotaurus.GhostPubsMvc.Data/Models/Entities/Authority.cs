using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Carnotaurus.GhostPubsMvc.Common.Extensions;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

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
            get { return string.Format("{0} {1}", Name, Type.Replace("Met ", String.Empty)); }
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
        public List<String> RegionalLineage
        {
            get
            {
                var list = Levels;

                list.Remove("England");
                list.Remove("England and Wales");
                list.Remove("Great Britain");
                list.Remove("United Kingdom");

                return list;
            }
        }

        [NotMapped]
        public List<String> Levels
        {
            get
            {
                var result = LevelsAscending.ReverseItems();

                return result;
            }
        }

        [NotMapped]
        public List<String> LevelsAscending
        {
            get
            {
                var lastAncestor = this;

                var list = new List<String>();

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

        //      && x.ParentAuthority.IsRegion
        //        && x.Authoritys.Any(y => y.HasHauntedOrgs)
        // todo - dpc - perform count
        [NotMapped]
        public bool HasHauntedOrgs
        {
            get
            {
                var isHaunted = CountHauntedOrgs > 0;

                return isHaunted;
            }
        }

        [NotMapped]
        public int CountHauntedOrgs
        {
            get
            {
                var count = 0;

                count = Authoritys.Any()
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

        public int Id { get; set; }
    }
}