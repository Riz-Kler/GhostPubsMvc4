﻿using System;
using System.Linq;
using System.Security.Principal;
using System.Xml.Linq;
using Carnotaurus.GhostPubsMvc.Common.Bespoke.Enumerations;
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

        public void UpdateCounty(Org org, County match)
        {
            org.CountyId = match.Id;
        }

        public void Save()
        {
            _writer.SaveChanges();
        }

        public ResultTypeEnum UpdateOrganisation(Org missingInfoOrg, XElement xElement)
        {
            var isSuccess = ResultTypeEnum.Fail;

            if (xElement == null || xElement.Value.Contains("OVER_QUERY_LIMIT"))
            {
                return isSuccess;
            }

            missingInfoOrg.GoogleMapData = xElement.ToString();

            missingInfoOrg.Modified = DateTime.Now;

            missingInfoOrg.Tried = 1;

            if (xElement.Value.Contains("ZERO_RESULTS"))
            {
                isSuccess = ResultTypeEnum.NoResults;

                return isSuccess;
            }

            var result = xElement.Element("result");

            if (result == null) return isSuccess;

            isSuccess = ResultTypeEnum.Success;

            UpdateGeocodes(result, missingInfoOrg);

            UpdateLocality(result, missingInfoOrg);

            UpdateTown(result, missingInfoOrg);

            return isSuccess;
        }

        public string UpdateAdministrativeAreaLevels(XContainer result, Org org)
        {
            if (result == null) throw new ArgumentNullException("result");

            var countyResult =
                result.Elements("address_component")
                    .FirstOrDefault(x => x.Value.EndsWith("administrative_area_level_2political"));

            if (countyResult == null || countyResult.FirstNode == null) return null;

            var inner = countyResult.FirstNode as XElement;

            if (inner == null) return null;

            var outer = inner.Value;

            org.AdministrativeAreaLevel2 = outer;

            return outer;
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
                result.Elements("address_component").FirstOrDefault(x => x.Value.EndsWith("localitypolitical"));

            if (match == null || match.FirstNode == null) return;

            var firstResult = match.FirstNode as XElement;

            if (firstResult == null) return;

            org.Locality = firstResult.Value;
        }

        private static void UpdateGeocodes(XContainer result, Org org)
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