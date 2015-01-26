using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Carnotaurus.GhostPubsMvc.Common.Extensions;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;
using Carnotaurus.GhostPubsMvc.Data.Models.ViewModels;
using Humanizer;

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
        public int Population { get; set; }
        public int Hectares { get; set; }
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
                var result = HauntedPubCount.IsAboveZero();

                return result;
            }
        }

        [NotMapped]
        public int HauntedPubCount
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
        public double DensityInHectares
        {
            get
            {
                var density = Population / (double)Hectares;

                return density;
            }
        }

        [NotMapped]
        public int DensityInSquareMiles
        {
            get
            {
                var result = (Population / (double)AreaSizeInSquareMiles).ToInt32();

                return result;
            }
        }

        [NotMapped]
        public int DensityPerHauntedPub
        {
            get
            {
                var result = 0;

                if (HauntedPubCount <= 0) return result;

                result = (DensityInSquareMiles / (double)HauntedPubCount)
                    .ToInt32();

                return result;
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
        public int AreaSizeInSquareMiles
        {
            get
            {
                var result = (Hectares / 258.999).ToInt32();

                return result;
            }
        }

        [NotMapped]
        public int PeoplePerHauntedPub
        {
            get { return (Population / (double)HauntedPubCount).ToInt32(); }
        }

        [NotMapped]
        public int AreaSizeInSquareMilesPerHauntedPub
        {
            get { return (AreaSizeInSquareMiles / (double)HauntedPubCount).ToInt32(); }
        }

        [NotMapped]
        public string Summary
        {
            get
            {
                string result;

                try
                {
                    result =
                        string.Format(
                            "In approximate terms: {0} has " +
                            "It has a population of {1} people. " +
                            "It occupies {2} square miles. " +
                            "So, that's a density of {3} people per square mile. " +
                            "More interestingly in terms of haunted pubs, it has {4} such pubs. " +
                            "So that's {5} people per haunted pub. " +
                            "Further, that's {6} area size in square miles per haunted pub. " +
                            "Considering it's density for haunted pubs, there are {7} per person for every square mile. " +
                            "There lower the value, the more haunted it is in terms of haunted pubs. " +
                            "However, how does this compare to elsewhere? " +
                            "Well, we'll tell you later. Why not revisit us in a few days to find? ",
                            FullyQualifiedName,
                            Population.ToWords(),
                            AreaSizeInSquareMiles.ToWords(),
                            DensityInSquareMiles.ToWords(),
                            HauntedPubCount.ToWords(),
                            PeoplePerHauntedPub.ToWords(),
                            AreaSizeInSquareMilesPerHauntedPub.ToWords(),
                            DensityPerHauntedPub.ToWords()
                            )
                        ;
                }
                catch (Exception ex)
                {
                    var m = ex.InnerException;
                    result = string.Empty;
                }

                return result;
            }
        }

        [NotMapped]
        public string FullyQualifiedName
        {
            get
            {
                const string pattern = "{0} in {1}";

                var result = GetFullyQualifiedName(pattern);

                return result;
            }
        }

        [NotMapped]
        public string FullyQualifiedNameParentFirst
        {
            get
            {
                const string pattern = "{1} : {0}";

                var result = GetFullyQualifiedName(pattern);

                return result;
            }
        }

        public int Id { get; set; }

        private string GetFullyQualifiedName(string pattern)
        {
            var result = ParentAuthority != null
                ? string.Format(pattern,
                    QualifiedName,
                    ParentAuthority.QualifiedName)
                : QualifiedName;

            return result;
        }

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

            if (sibbling == null) return result;

            result = new PageLinkModel
            {
                Text = sibbling.QualifiedName,
                Title = sibbling.QualifiedName,
                Filename = sibbling.CleanQualifiedName
            };

            return result;
        }
    }
}