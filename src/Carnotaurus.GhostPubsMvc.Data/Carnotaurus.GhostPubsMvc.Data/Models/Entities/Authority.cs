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
        public virtual List<Org> HauntedOrgs
        {
            get
            {
                var pubs = new List<Org>();

                if (IsDistrict || IsUnitary || IsLondonBorough || IsOutsideUnitedKingdom)
                {
                    pubs.AddRange(Orgs.Where(o => o.IsHauntedPub).ToList());
                }
                else if (IsCounty)
                {
                    var haunted = Authoritys.SelectMany(a => a.Orgs, (a, b) => new { a, b })
                        .Where(@t => @t.b.IsHauntedPub)
                        .Select(@t => @t.b);

                    pubs.AddRange(haunted);
                }
                else if (IsRegion || IsEngland || IsCrossBorderArea)
                {
                    var orgs = Authoritys.SelectMany(s => s.HauntedOrgs).ToList();
                    pubs.AddRange(orgs);
                }
                else
                {
                    var many = Authoritys.Where(s => s.Orgs.All(t => t.IsHauntedPub))
                        .SelectMany(u => u.Orgs)
                        .ToList();

                    var orgs = Orgs.Where(t => t.IsHauntedPub)
                        .ToList();

                    pubs = Authoritys.Any() ? many : orgs;
                }

                return pubs.ToList();
            }
        }

        [NotMapped]
        public bool IsCrossBorderArea
        {
            get
            {
                var result = Type == "Cross border area";

                if (result)
                {
                    var q = "test";
                }

                return result;
            }
        }

        [NotMapped]
        public bool IsEngland
        {
            get { return Name == "England"; }
        }

        [NotMapped]
        public string QualifiedName
        {
            get { return String.Format("{0} {1}", Name, Type.Replace("Met ", String.Empty)); }
        }

        [NotMapped]
        public string CleanQualifiedName
        {
            get { return QualifiedName.Clean(); }
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
                var result = CleanQualifiedName.ToLower().EndsWith("-district");

                return result;
            }
        }

        [NotMapped]
        public bool IsUnitary
        {
            get
            {
                var result = CleanQualifiedName.ToLower().EndsWith("-ua");

                return result;
            }
        }

        [NotMapped]
        public bool IsLondonBorough
        {
            get
            {
                var result = CleanQualifiedName.ToLower().EndsWith("-london-borough");

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
        public Boolean IsOutsideUnitedKingdom
        {
            get
            {
                var excluded = false;

                var lastAncestor = this;

                if (lastAncestor.IsCrownDependency) return true;

                while (lastAncestor != null
                       && lastAncestor.ParentId != 0)
                {
                    var currentAncestor = lastAncestor.ParentAuthority;

                    if (currentAncestor != null)
                    {
                        if (!currentAncestor.Name.IsNullOrEmpty()
                            && currentAncestor.IsCrownDependency)
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
                    Authoritys.Sum(s => s.HauntedOrgs.Count())
                    : // other ones
                    Orgs.Count(x => x.IsHauntedPub);

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
        protected bool IsCrownDependency
        {
            get
            {
                // todo - dpc - this should be in the database?
                var result = Code != null && (Code == "JE" || Code == "IOM" || Code == "GUR");
                return result;
            }
        }

        [NotMapped]
        public string DetailedName
        {
            get
            {
                string result;
                if (ParentAuthority != null)
                {
                    result = QualifiedName + " in " + ParentAuthority.QualifiedName;
                }
                else
                {
                    result = QualifiedName;
                }

                return result;
            }
        }

        [NotMapped]
        public string LongName
        {
            get
            {
                string result;
                if (ParentAuthority != null)
                {
                    result = string.Format("{0} : {1}", ParentAuthority.QualifiedName, QualifiedName);
                }
                else
                {
                    result = QualifiedName;
                }

                return result;
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
                Filename = CleanQualifiedName
            };

            if (ParentAuthority != null)
            {
                var ints = ParentAuthority.Authoritys
                    .Where(h => h.HasHauntedOrgs && !h.IsCrossBorderArea)
                    .OrderBy(o => o.QualifiedName)
                    .Select(s => s.Id).ToList();

                var nextId = ints.FindNext(Id);

                if (nextId.HasValue)
                {
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
                    Filename = sibbling.CleanQualifiedName
                };
            }

            return result;
        }
    }
}