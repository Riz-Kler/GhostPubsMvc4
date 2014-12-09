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


        public int Id { get; set; }

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
        public bool IsRegion
        {
            get
            {
                var result = Type.ToLower() == "region";

                return result;
            }
        }

        [NotMapped]
        public List<String> RegionalLineage
        {
            get
            {
                var list = Lineage;

                list.Remove("ENGLAND");
                list.Remove("ENGLAND AND WALES");
                list.Remove("GREAT BRITAIN");
                list.Remove("UNITED KINGDOM");

                return list;
            }
        }

        [NotMapped]
        public String LineageSignature
        {
            get
            {
                var result = Lineage.SeoFormat();

                return result;
            }
        }

        [NotMapped]
        public List<String> Lineage
        {
            get
            {
                var result = LineageAscending.ReverseItems();

                return result;
            }
        }

        [NotMapped]
        protected List<String> LineageAscending
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

                count = Authoritys.Any() ?
                    // how to get county and metropolian county 
                    Authoritys.Sum(s => s.Orgs.Count(x => x.HauntedStatus.HasValue && x.HauntedStatus.Value)) :
                    // other ones
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

    }
}