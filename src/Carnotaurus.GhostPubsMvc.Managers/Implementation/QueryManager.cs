using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Carnotaurus.GhostPubsMvc.Data;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;
using Carnotaurus.GhostPubsMvc.Data.Models;
using Carnotaurus.GhostPubsMvc.Managers.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Managers.Implementation
{
    public class QueryManager : IQueryManager
    {
        private readonly IReadStore _reader;

        public QueryManager(IReadStore reader)
        {
            _reader = reader;
        }

        public IEnumerable<Org> GetMissingInfoOrgsToUpdate()
        {
            var orgs = _reader.Items<Org>();

            var results = orgs
                    .Where(f =>
                        f != null
                        && f.Address != null
                        && f.Postcode != null
                        && f.AddressTypeId == 1
                        && f.CountyId != null
                        && f.Tried == null
                    )
                    .ToList();

            return results;
        }
         
        public County GetCounty( string outer)
        {
            var orgs = _reader.Items<County>();
            
            var results = orgs.FirstOrDefault(x => x.Name == outer);
            
            return results;
        }

        //public PaymentInputModel GetPaymentDefaults(PaymentInputModel model, String email)
        //{
        //    var userProfile = FindUser(email);

        //    if (userProfile != null)
        //    {
        //        model.Postcode = userProfile.Postcode;

        //        model.Surname = userProfile.Surname;

        //        model.ClientBranch = (ClientBranchEnum) userProfile.ClientBranch;

        //        model.Postcode = userProfile.Postcode;

        //        model.ClientReference = userProfile.ClientReference;

        //        model.PaymentAmount = userProfile.PaymentAmount;

        //        model.PaymentFrequency = (PaymentFrequencyEnum) userProfile.PaymentFrequency;

        //        model.Address1 = userProfile.Address1;
        //        model.Address2 = userProfile.Address2;
        //        model.Address3 = userProfile.Address3;
        //        model.Town = userProfile.Town;

        //        model.CartUnique = Guid.NewGuid();

        //        model.CartRef = string.Format("{0}-{1}-{2}{3}{4}{5}{6}{7}",
        //            userProfile.Id.ToString(CultureInfo.InvariantCulture),
        //            model.CartUnique.Value.RemoveSpecialCharacters(),
        //            DateTime.Now.Year,
        //            DateTime.Now.Month,
        //            DateTime.Now.Day,
        //            DateTime.Now.Hour,
        //            DateTime.Now.Minute,
        //            DateTime.Now.Second
        //            );

        //        model.AdvisorName = userProfile.AdvisorName;

        //        model.AdvisorTel = userProfile.AdvisorTel;

        //        model.UserRef = userProfile.Id.ToString(CultureInfo.InvariantCulture);
        //    }

        //    model.PaymentFrequency = PaymentFrequencyEnum.Once;

        //    return model;
        //}

        //public int GetTotalUserProfiles(UserSearchListInputModel model)
        //{
        //    var results = _reader.Items<UserProfile>()
        //        .Total(model.Search, model.ApprovalStatusEnum);

        //    return results;
        //}

        //public PaymentCapture GetLatestPaymentCapture(String username, CartRefModel model)
        //{
        //    // look it up and get last payment save success date

        //    var user = FindUser(username);

        //    if (model == null) return null;

        //    if (!model.UserRef.HasValue || user.Id != model.UserRef.Value) return null;

        //    if (!model.CartUnique.HasValue) return null;

        //    if (model.DateTimeString.IsNullOrEmpty()) return null;

        //    // find the payment
        //    var payment = FindLatestPaymentByCartRef(model);

        //    return payment;
        //}

        //public int GetTotalPayments(PaymentSearchListInputModel model)
        //{
        //    var results = _reader.Items<PaymentCapture>()
        //        .Total(model.Search);

        //    return results;
        //}

        //public UserProfile FindUserByClientRefAndClientBranch(int clientRef, ClientBranchEnum clientBranch)
        //{
        //    // , ClientBranchEnum clientBranch
        //    return FindUserByClientRefAndClientBranch(clientRef, (Int32) clientBranch);
        //}

        //public UserProfile FindUser(Int32 id)
        //{
        //    var profiles = _reader.Items<UserProfile>();

        //    var selected = profiles.FirstOrDefault(x => x.Id == id);

        //    return selected;
        //}

        //public UserProfile FindUser(string username)
        //{
        //    var profiles = _reader.Items<UserProfile>();

        //    var selected = profiles.FirstOrDefault(x => x.UserName == username);

        //    return selected;
        //}

        //public Role FindRole(string rolename)
        //{
        //    var results = GetAllRoles();

        //    var result = results.FirstOrDefault(x => x.RoleName == rolename);

        //    return result;
        //}

        //public List<Role> GetAllRoles()
        //{
        //    var results = _reader.Items<Role>().ToList();

        //    return results;
        //}

        //public List<UserProfile> GetAllUserProfiles()
        //{
        //    var results = _reader.Items<UserProfile>().ToList();

        //    return results;
        //}

        //public List<UserProfile> GetPagedUserProfiles(UserSearchListInputModel model)
        //{
        //    var results = _reader.Items<UserProfile>()
        //        .Page(model.CurrentPage, model.PageSize, model.Search, model.ApprovalStatusEnum)
        //        .ToList();

        //    return results;
        //}

        //public PaymentCapture FindLatestPaymentByCartRef(CartRefModel model)
        //{
        //    var results = _reader.Items<PaymentCapture>();

        //    var selected =
        //        results.Where(x =>
        //            x.UserId == model.UserRef
        //            && x.CartRef != null
        //            && x.CartUnique.HasValue
        //            && x.CartUnique.Value == model.CartUnique.Value
        //            )
        //            .OrderByDescending(x => x.CreatedDate)
        //            .FirstOrDefault();

        //    return selected;
        //}

        //public List<PaymentCapture> GetPagedPayments(PaymentSearchListInputModel model)
        //{
        //    var results = _reader.Items<PaymentCapture>()
        //        .OrderByDescending(x => x.CreatedDate)
        //        .Page(model.CurrentPage, model.PageSize, model.Search)
        //        .ToList();

        //    return results;
        //}

        //public UserProfile FindUserByClientRefAndClientBranch(int clientRef, int clientBranch)
        //{
        //    var profiles = _reader.Items<UserProfile>();

        //    var selected = profiles.FirstOrDefault(x => x.ClientReference == clientRef && x.ClientBranch == clientBranch);

        //    return selected;
        //}
    }
}