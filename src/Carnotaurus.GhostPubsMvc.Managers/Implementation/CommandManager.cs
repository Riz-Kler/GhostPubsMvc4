using System;
using System.Linq;
using System.Security.Principal;
using System.Xml.Linq;
using Carnotaurus.GhostPubsMvc.Common.Extensions;
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

        public void UpdateAuthority(Org org, Int32 id)
        {
            org.AuthorityId = id;
            org.Modified = DateTime.Now;
        }

        public void Save()
        {
            try
            {
                _writer.SaveChanges();
            }
            catch (Exception ex)
            {
                var message = ex.InnerException;
            }
        }

        public void UpdateOrgFromGoogleResponse(Org org, XContainer element)
        {
            var result = element.Element("result");

            UpdateOrgFromGoogleResponse(result, org);
        }

        public void UpdateOrgFromLaApiResponse(Org org, XContainer result)
        {
            if (result == null) throw new ArgumentNullException("result");

            org.LaTried = true;

            var administrative = result.Element("administrative");

            if (administrative == null) return;

            var council = administrative.Element("council");

            if (council == null) return;

            var code = council.Element("code");

            if (code == null) return;

            // Card #389 - St Albans
            if (code.Value == "E07000100")
            {
                code.Value = "E07000240";
            }

            // Card #389 - Glasgow City
            if (code.Value == "S12000043")
            {
                code.Value = "S12000046";
            }

            org.LaCode = code.Value;

            org.Modified = DateTime.Now;
        }

        private void UpdateOrgFromGoogleResponse(XContainer result, Org org)
        {
            if (result == null) throw new ArgumentNullException("result");

            org.Modified = DateTime.Now;

            org.Tried = true;

            UpdateGeocodes(result, org);

            UpdateLocality(result, org);

            UpdateTown(result, org);
        }

        private static void UpdateTown(XContainer result, Org org)
        {
            if (result == null) throw new ArgumentNullException("result");

            var townResult =
                result.Elements("address_component").FirstOrDefault(x => x.Value.EndsWith("postal_town"));

            if (townResult == null || townResult.FirstNode == null) return;

            var firstResult = townResult.FirstNode as XElement;

            if (firstResult != null)
            {
                org.Modified = DateTime.Now;
            }
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
            org.Modified = DateTime.Now;
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
            {
                org.Lat = lat.Value.ToNullableDouble();
                org.Modified = DateTime.Now;
            }

            var lng = locationElement.Element("lng");
            if (lng != null)
            {
                org.Lon = lng.Value.ToNullableDouble();
                org.Modified = DateTime.Now;
            }
        }
    }
}