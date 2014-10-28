﻿using System;
using System.Linq;
using System.Security.Principal;
using System.Xml.Linq;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;
using Carnotaurus.GhostPubsMvc.Managers.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Managers.Implementation
{
    public class CommandManager : ICommandManager
    {
        private readonly IMailManager _mailer;
        private readonly IMailSender _sender;
        private readonly IWriteStore _writer;

        public CommandManager(IWriteStore writeStore, IMailManager mailer, IMailSender sender)
        {
            _writer = writeStore;
            _sender = sender;
            _mailer = mailer;
        }

        public String CurrentUserName
        {
            get
            {
                var i = WindowsIdentity.GetCurrent();

                return i != null ? i.Name : String.Empty;
            }
        }

        public void UpdateCounty(Org org, int id)
        {
            org.CountyId = id;
        }

        public void Save()
        {
            _writer.SaveChanges();
        }

        public void UpdateOrgFromGoogleResponse(Org org, XContainer element, CountyAdminPair countyAdmin)
        {
            var result = element.Element("result");

            UpdateOrgFromGoogleResponse(result, org, countyAdmin);
        }

        public void UpdateCouncil(Org org, int match)
        {
            // todo - dpc - come back - org.CouncilId = match;
        }

        public void UpdateOrgFromLaApiResponse(Org org, XContainer result)
        {
            if (result == null) throw new ArgumentNullException("result");

            var administrative = result.Element("administrative");

            if (administrative == null) return;

            var council = administrative.Element("council");

            if (council == null) return;

            var code = council.Element("code");

            if (code != null)
                org.LaCode = code.Value;
        }

        private void UpdateOrgFromGoogleResponse(XContainer result, Org org, CountyAdminPair countyAdmin)
        {
            if (result == null) throw new ArgumentNullException("result");

            UpdateGeocodes(result, org);

            UpdateLocality(result, org);

            UpdateTown(result, org);

            // administrative

            if (countyAdmin != null)
            {
                org.AdministrativeAreaLevel2 = countyAdmin.AdminLevelTwo;

                if (countyAdmin.CountyId.HasValue)
                {
                    UpdateCounty(org, countyAdmin.CountyId.Value);
                }
            }
        }

        private static void UpdateTown(XContainer result, Org org)
        {
            if (result == null) throw new ArgumentNullException("result");

            var townResult =
                result.Elements("address_component").FirstOrDefault(x => x.Value.EndsWith("postal_town"));

            if (townResult == null || townResult.FirstNode == null) return;

            var firstResult = townResult.FirstNode as XElement;

            if (firstResult != null) org.Town = firstResult.Value;
        }

        private static void UpdateLocality(XContainer result, Org org)
        {
            if (result == null) throw new ArgumentNullException("result");

            var match =
                result.Elements("address_component")
                    .FirstOrDefault(x => x.Value.EndsWith("localitypolitical"));

            if (match == null || match.FirstNode == null) return;

            var firstResult = match.FirstNode as XElement;

            if (firstResult == null) return;

            org.Locality = firstResult.Value;
        }


        public void UpdateGeocodes(XContainer result, Org org)
        {
            if (result == null) throw new ArgumentNullException("result");

            var element = result.Element("geometry");

            if (element == null) return;

            var locationElement = element.Element("location");

            if (locationElement == null) return;

            var lat = locationElement.Element("lat");
            if (lat != null)
                org.Lat = Convert.ToDouble(lat.Value);

            var lng = locationElement.Element("lng");
            if (lng != null)
                org.Lon = Convert.ToDouble(lng.Value);
        }
    }
}