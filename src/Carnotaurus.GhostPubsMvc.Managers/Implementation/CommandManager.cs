using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Xml.Linq;
using Carnotaurus.GhostPubsMvc.Common.Bespoke;
using Carnotaurus.GhostPubsMvc.Common.Helpers;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;
using Carnotaurus.GhostPubsMvc.Data.Models;
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

        public void UpdateCounty(Org org, County  match)
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

        public XElement ReadXElement(Org missingInfoOrg)
        {
            // source correct address, using google maps api or similar

            // E.G., https://maps.googleapis.com/maps/api/geocode/xml?address=26%20Smithfield,%20London,%20Greater%20London,%20EC1A%209LB,%20uk&sensor=true&key=AIzaSyC2DCdkPGBtsooyft7sX3P9h2f4uQvLQj0

            var key = ConfigurationHelper.GetValueAsString("GoogleMapsApiKey");
            // "AIzaSyC2DCdkPGBtsooyft7sX3P9h2f4uQvLQj0";

            var requestUri = ("https://maps.google.com/maps/api/geocode/xml?address="
                              + missingInfoOrg.TradingName
                              + ", "
                              + missingInfoOrg.Address
                              + ", "
                              + missingInfoOrg.Postcode
                              + ", UK&sensor=false&key=" + key);

            var xdoc = XDocument.Load(requestUri);

            var xElement = xdoc.Element("GeocodeResponse");

            return xElement;
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


        //protected String PrepareModel(OrgOutputModel data)
        //{
        //    var output = this.PrepareView(data, data.Action);

        //    return output;
        //}

        //public void WriteLines(OrgOutputModel entities)
        //{
        //    var model = PrepareModel(entities);

        //    // write region file
        //    System.IO.File.WriteAllText(entities.Unc + @"\" + "detail.html", model);
        //}


        //public CommandResult SavePayment(PaymentInputModel model)
        //{
        //    var result = model.ValidateModel();

        //    if (!result.Success) return result;

        //    var data = BuildPaymentCapture(model);

        //    result = SavePayment(data);

        //    return result;
        //}

        //public CommandResult SavePayment(PaymentCapture model)
        //{
        //    if (model.Address1.IsNullOrEmpty())
        //    {
        //        model.Address1 = "Unspecified";
        //    }

        //    if (model.Town.IsNullOrEmpty())
        //    {
        //        model.Town = "Unspecified";
        //    }

        //    if (model.PaymentFrequency == 0)
        //    {
        //        model.PaymentFrequency = 1;
        //    }

        //    var result = model.Id > 0
        //        ? ModifyPayment(model)
        //        : SavePayment2(model);

        //    if (result != null && result.Success)
        //    {
        //        _writer.SaveChanges();
        //    }

        //    return result;
        //}

        //public CommandResult SavePaymentReturn(PaymentCapture model, Boolean isSuccess)
        //{
        //    if (model.HasReturnedDate)
        //    {
        //        return new CommandResult(String.Empty, "Already stored");
        //    }

        //    if (isSuccess)
        //    {
        //        model.SuccessDate = DateTime.Now;
        //    }
        //    else
        //    {
        //        model.CancelDate = DateTime.Now;
        //    }

        //    // save date
        //    return SavePayment(model);
        //}

        //public CommandResult ArchiveUser(UserProfile command)
        //{
        //    if (command == null) return new CommandResult(String.Empty, "No user specified");

        //    var result = new CommandResult();

        //    _writer.Archive(command);

        //    if (!result.Success) return result;

        //    _writer.SaveChanges();

        //    return result;
        //}

        //public CommandResult AddUser(CreateUserProfileInputModel model)
        //{
        //    var result = model.ValidateModel();

        //    if (result != null && !result.Success) return result;

        //    var command = BuildUserProfile(model);

        //    result = AddUser(command);

        //    if (result != null && !result.Success) return result;

        //    _writer.SaveChanges();

        //    return result;
        //}

        //public CommandResult AddRoleToUser(UserProfile userProfile, Role role)
        //{
        //    if (role == null) return new CommandResult(String.Empty, "No role specified");

        //    if (userProfile == null) return new CommandResult(String.Empty, "No user specified");

        //    var result = new CommandResult();

        //    if (!userProfile.Roles.Contains(role))
        //    {
        //        userProfile.Roles.Add(role);
        //    }

        //    if (!result.Success) return result;

        //    _writer.SaveChanges();

        //    return result;
        //}

        //public CommandResult ModifyUser(EditUserProfileInputModel model)
        //{
        //    var result = model.ValidateModel();

        //    if (result != null && !result.Success) return result;

        //    var command = BuildUserProfile(model);

        //    if (command != null && command.Address1.IsNullOrEmpty())
        //    {
        //        command.Address1 = "Unspecified";
        //    }

        //    if (command != null && command.Town.IsNullOrEmpty())
        //    {
        //        command.Town = "Unspecified";
        //    }

        //    if (command != null && command.PaymentFrequency == 0)
        //    {
        //        command.PaymentFrequency = 1;
        //    }

        //    result = ModifyUser(command);

        //    if (result != null && !result.Success) return result;

        //    _writer.SaveChanges();

        //    return result;
        //}

        //public CommandResult NotifyLostPassword(LostPasswordModel model, String url, String token, UserProfile user)
        //{
        //    var result = model.ValidateModel();

        //    if (result != null && !result.Success) return result;

        //    if (model == null) return new CommandResult(String.Empty, "No model specified");

        //    if (url == null) return new CommandResult(String.Empty, "No url specified");

        //    if (token == null) return new CommandResult(String.Empty, "No token specified");

        //    // Generae password token that will be used in the email link to authenticate user
        //    var message = CreateLostPasswordMailMessage(model, url, token, user);

        //    // Attempt to send the email
        //    return _mailer.Send(message, null);
        //}

        //public CommandResult Approve(UserProfile userProfile, Role role, String url)
        //{
        //    if (role == null) return new CommandResult(null, "Role not specified");

        //    if (userProfile == null) return new CommandResult(null, "User not specified");

        //    if (userProfile.Postcode.IsNullOrEmpty()) return new CommandResult("Postcode", "No postcode specified");

        //    if (userProfile.ClientReference == 0)
        //        return new CommandResult("ClientReference", "No client ref specified");

        //    userProfile.DenyReason = String.Empty;

        //    userProfile.ApprovalStatus = ApprovalStatusEnum.Approved;

        //    AddRoleToUser(userProfile, role);

        //    // send approval email from ClearDebt to the user
        //    // Generate password token that will be used in the email link to authenticate user
        //    var message = BuildApprovalMailMessage(userProfile, url);

        //    // Attempt to send the email
        //    return _mailer.Send(message, null);
        //}

        //public CommandResult NotifyRegistration(RegistrationInputModel model, String url)
        //{
        //    var result = model.ValidateModel();

        //    if (!result.Success) return result;

        //    if (model == null) return new CommandResult(String.Empty, "Model is null");

        //    // Attempt to send the advisor email
        //    // send an email to notify Laura to say that this user has registered and needs approving to user the portal
        //    // generate password token that will be used in the email link to authenticate user
        //    var advisorMessage = CreateUserApprovalRequestedMailMessage(model, url);
        //    var sendMessage = _mailer.Send(advisorMessage, null);
        //    if (!sendMessage.Success)
        //        return new CommandResult(String.Empty, "Failed to send the message to the advisor");

        //    // Attempt to send the user email
        //    var userMessage = CreateUserThankYouMailMessage(model, url);
        //    var sendUserMessage = _mailer.Send(userMessage, null);
        //    return !sendUserMessage.Success
        //        ? new CommandResult(String.Empty, "Failed to send the message to the user")
        //        : new CommandResult();
        //}

        //public CommandResult Decline(UserProfile userProfile, Role role, String reason)
        //{
        //    if (userProfile == null) return new CommandResult(null, "User not specified");

        //    if (reason.IsNullOrEmpty()) return new CommandResult(null, "No decline reason specified");

        //    userProfile.DenyReason = reason;

        //    userProfile.ApprovalStatus = ApprovalStatusEnum.Declined;

        //    var removed = RemoveUserFromRole(userProfile, role);

        //    if (!removed.Success) return removed;

        //    // send decline email from ClearDebt to the user
        //    // Generate password token that will be used in the email link to authenticate user

        //    var message = BuildDeclinedMailMessage(userProfile, null);

        //    // Attempt to send the email
        //    return _mailer.Send(message, null);
        //}

        //public CommandResult Clear(UserProfile user, Role role)
        //{
        //    if (user == null) return new CommandResult(String.Empty, "No user specified");

        //    if (user.UserName.IsNullOrEmpty()) return new CommandResult(String.Empty, "No username specified");

        //    user.DenyReason = String.Empty;

        //    user.ApprovalStatus = ApprovalStatusEnum.Pending;

        //    return RemoveUserFromRole(user, role);
        //}

        //public CommandResult RemoveUserFromRole(UserProfile user, Role role)
        //{
        //    if (user == null) return new CommandResult(String.Empty, "No user specified");

        //    if (role == null) return new CommandResult(String.Empty, "No role specified");

        //    var result = new CommandResult();

        //    if (user.Roles.Contains(role))
        //    {
        //        user.Roles.Remove(role);
        //    }

        //    if (result.Success)
        //    {
        //        _writer.SaveChanges();
        //    }

        //    return result;
        //}

        //public CommandResult Suspend(UserProfile userProfile, Role role, string reason)
        //{
        //    if (userProfile == null) return new CommandResult(null, "User not specified");

        //    if (reason.IsNullOrEmpty()) return new CommandResult(null, "No suspend reason specified");

        //    userProfile.DenyReason = reason;

        //    userProfile.ApprovalStatus = ApprovalStatusEnum.Suspended;

        //    var removed = RemoveUserFromRole(userProfile, role);

        //    if (!removed.Success) return removed;

        //    // send decline email from ClearDebt to the user
        //    // Generate password token that will be used in the email link to authenticate user

        //    var message = BuildSuspendedMailMessage(userProfile, null);

        //    // Attempt to send the email
        //    return _mailer.Send(message, null);
        //}

        //public CommandResult ModifyPayment(PaymentCapture command)
        //{
        //    if (command == null) return new CommandResult(String.Empty, "No user specified");

        //    var result = new CommandResult();

        //    // populate "loaded" with properties from "command"
        //    var loaded = _writer.Load<PaymentCapture>(command.Id);

        //    loaded.Surname = command.Surname;
        //    loaded.CreatedDate = command.CreatedDate;
        //    loaded.ClientBranch = command.ClientBranch;
        //    loaded.Postcode = command.Postcode;
        //    loaded.ClientReference = command.ClientReference;
        //    loaded.CreatedBy = command.CreatedBy;
        //    loaded.PaymentAmount = command.PaymentAmount;
        //    loaded.PaymentFrequency = command.PaymentFrequency;
        //    loaded.Address1 = command.Address1;
        //    loaded.Address2 = command.Address2;
        //    loaded.Address3 = command.Address3;
        //    loaded.Town = command.Town;
        //    loaded.IpAddress = command.IpAddress;
        //    loaded.SuccessDate = command.SuccessDate;
        //    loaded.CancelDate = command.CancelDate;
        //    loaded.CartRef = command.CartRef;
        //    loaded.CartUnique = command.CartUnique;
        //    loaded.TransRef = command.TransRef;

        //    return result;
        //}


        //public CommandResult AddUser(UserProfile command)
        //{
        //    if (command == null) return new CommandResult(String.Empty, "No user specified");

        //    var result = new CommandResult();

        //    // Card #479 - (1) On register and create new user, disallow Client Reference if already used on Client Portal
        //    command.Address1 = String.Empty;
        //    command.Address2 = String.Empty;
        //    command.Address3 = String.Empty;
        //    command.Town = String.Empty;

        //    _writer.Insert(command);

        //    return result;
        //}

        //public CommandResult ModifyUser(UserProfile command)
        //{
        //    if (command == null) return new CommandResult(String.Empty, "No user specified");

        //    var result = new CommandResult();

        //    // populate "loaded" with properties from "command"
        //    var loaded = _writer.Load<UserProfile>(command.Id);

        //    loaded.UserName = command.UserName;
        //    loaded.Forename = command.Forename;
        //    loaded.Surname = command.Surname;
        //    loaded.CreatedDate = command.CreatedDate;
        //    loaded.ClientBranch = command.ClientBranch;
        //    loaded.Postcode = command.Postcode;
        //    loaded.ClientReference = command.ClientReference;
        //    loaded.CreatedBy = command.CreatedBy;
        //    loaded.PaymentAmount = command.PaymentAmount;
        //    loaded.PaymentFrequency = command.PaymentFrequency;
        //    loaded.Address1 = command.Address1;
        //    loaded.Address2 = command.Address2;
        //    loaded.Address3 = command.Address3;
        //    loaded.Town = command.Town;

        //    return result;
        //}

        //public CommandResult DeleteUser(UserProfile command)
        //{
        //    if (command == null) return new CommandResult(String.Empty, "No user specified");

        //    var result = new CommandResult();

        //    _writer.Delete(command);

        //    return result;
        //}

        //public CommandResult SavePayment2(PaymentCapture command)
        //{
        //    var result = new CommandResult();

        //    _writer.Insert(command);

        //    return result;
        //}

        //private static MailMessage CreateLostPasswordMailMessage(LostPasswordModel model, String url, String token,
        //    UserProfile user)
        //{
        //    var responseModel = BuildResetPasswordMailResponseModel(model, url, token, user);

        //    return ModelBuilderHelper.BuildMailMessage(responseModel);
        //}

        //private static MailResponseModel BuildResetPasswordMailResponseModel(LostPasswordModel model, String url,
        //    String token,
        //    UserProfile user)
        //{
        //    if (user == null) return null;

        //    if (user.UserName.IsNullOrEmpty()) return null;

        //    if (model == null) return null;

        //    var responseModel = new MailResponseModel
        //    {
        //        Token = token,
        //        Subject = Resources.EmailResetSubject,
        //        From = Resources.EmailGeneralFrom,
        //        To = model.UserName,
        //        Link = string.Format("<a href='{0}'>Reset Password</a>", url)
        //    };

        //    // Generate the html link sent via email

        //    var body = Resources.EmailResetBody;

        //    responseModel.Body = String.Format(body, user.Forename, responseModel.Link);

        //    return responseModel;
        //}

        //private MailMessage BuildApprovalMailMessage(UserProfile user, String actionLink)
        //{
        //    var responseModel = BuildApprovalMailResponseModel(user, actionLink);

        //    return ModelBuilderHelper.BuildMailMessage(responseModel);
        //}

        //private MailMessage BuildDeclinedMailMessage(UserProfile user, String actionLink)
        //{
        //    var responseModel = BuildDeclinedMailResponseModel(user);

        //    return ModelBuilderHelper.BuildMailMessage(responseModel);
        //}

        //private MailMessage BuildSuspendedMailMessage(UserProfile user, String actionLink)
        //{
        //    var responseModel = BuildSuspendedMailResponseModel(user);

        //    return ModelBuilderHelper.BuildMailMessage(responseModel);
        //}

        //private MailResponseModel BuildApprovalMailResponseModel(UserProfile model, String url)
        //{
        //    var responseModel = new MailResponseModel
        //    {
        //        Subject = String.Format(Resources.EmailApprovedSubject, model.Forename),
        //        From = Resources.EmailGeneralFrom,
        //        To = model.UserName,
        //        Link = string.Format("<a href='{0}'>Login link</a>", url)
        //    };

        //    // Generate the html link sent via email

        //    var body = Resources.EmailApprovedBody;

        //    responseModel.Body = String.Format(body, model.Forename, responseModel.Link);

        //    return responseModel;
        //}


        //private static MailResponseModel BuildDeclinedMailResponseModel(UserProfile model)
        //{
        //    var responseModel = new MailResponseModel
        //    {
        //        Subject = String.Format(Resources.EmailDeclinedSubject, model.Forename),
        //        From = Resources.EmailGeneralFrom,
        //        To = model.UserName,
        //        Link = String.Empty
        //    };

        //    var body = Resources.EmailDeclinedBody;

        //    responseModel.Body = String.Format(body, model.Forename, model.DenyReason);

        //    return responseModel;
        //}

        //private static MailResponseModel BuildSuspendedMailResponseModel(UserProfile model)
        //{
        //    var responseModel = new MailResponseModel
        //    {
        //        Subject = String.Format(Resources.EmailSuspendedSubject, model.Forename),
        //        From = Resources.EmailGeneralFrom,
        //        To = model.UserName,
        //        Link = String.Empty
        //    };

        //    var body = Resources.EmailSuspendedBody;

        //    responseModel.Body = String.Format(body, model.Forename, model.DenyReason);

        //    return responseModel;
        //}

        //private static MailResponseModel BuildRequestedApprovalMailResponseModel(RegistrationInputModel model,
        //    String url)
        //{
        //    var responseModel = new MailResponseModel
        //    {
        //        Subject = String.Format(Resources.EmailCreatedSubject, model.Forename),
        //        From = Resources.EmailGeneralFrom,
        //        To = Resources.EmailCreatedTo,
        //        Link = string.Format("<a href='{0}'>Approval link</a>", url)
        //    };

        //    // Generate the html link sent via email

        //    responseModel.Body = Resources.EmailCreatedBody + responseModel.Link;

        //    return responseModel;
        //}

        //private static MailResponseModel BuildThankYouMailResponseModel(RegistrationInputModel model, String url)
        //{
        //    var responseModel = new MailResponseModel
        //    {
        //        Subject = String.Format(Resources.EmailThankYouSubject, model.Forename),
        //        From = Resources.EmailGeneralFrom,
        //        To = model.UserName,
        //        Link = String.Empty
        //    };

        //    // Generate the html link sent via email

        //    var body = Resources.EmailThankYouBody;

        //    responseModel.Body = String.Format(body, model.Forename);

        //    return responseModel;
        //}


        //private static MailMessage CreateUserThankYouMailMessage(RegistrationInputModel model, String url)
        //{
        //    var responseModel = BuildThankYouMailResponseModel(model, url);

        //    return ModelBuilderHelper.BuildMailMessage(responseModel);
        //}

        //private static MailMessage CreateUserApprovalRequestedMailMessage(RegistrationInputModel model, String url)
        //{
        //    var responseModel = BuildRequestedApprovalMailResponseModel(model, url);

        //    return ModelBuilderHelper.BuildMailMessage(responseModel);
        //}

        //private PaymentCapture BuildPaymentCapture(PaymentInputModel model)
        //{
        //    var data = new PaymentCapture
        //    {
        //        ClientBranch = (Int32) model.ClientBranch,
        //        ClientReference = model.ClientReference ?? 0,
        //        PaymentAmount = Convert.ToDecimal(model.PaymentAmount),
        //        PaymentFrequency = (Int32) model.PaymentFrequency,
        //        Postcode = model.Postcode,
        //        Surname = model.Surname,
        //        // add created date and created by field
        //        CreatedDate = DateTime.Now,
        //        CreatedBy = CurrentUserName,
        //        Address1 = model.Address1,
        //        Address2 = model.Address2,
        //        Address3 = model.Address3,
        //        Town = model.Town,
        //        CartRef = model.CartRef,
        //        CartUnique = model.CartUnique,
        //        AdvisorName = model.AdvisorName,
        //        AdvisorTel = model.AdvisorTel,
        //        UserId = model.UserRef.ToInt32()
        //    };

        //    return data;
        //}

        //private static UserProfile BuildUserProfile(EditUserProfileInputModel model)
        //{
        //    var data = new UserProfile
        //    {
        //        Id = model.Id,
        //        ClientBranch = (Int32) model.ClientBranch,
        //        ClientReference = model.ClientReference ?? 0,
        //        Postcode = model.Postcode,
        //        Surname = model.Surname,
        //        Forename = model.Forename,
        //        UserName = model.UserName,
        //        // add created date and created by field
        //        CreatedDate = model.CreatedDate,
        //        CreatedBy = model.CreatedBy,
        //        PaymentAmount = model.PaymentAmount,
        //        PaymentFrequency = (Int32) model.PaymentFrequency,
        //        Address1 = model.Address1,
        //        Address2 = model.Address2,
        //        Address3 = model.Address3,
        //        Town = model.Town,
        //        ModifiedBy = model.ModifiedBy,
        //        ModifiedDate = model.ModifiedDate
        //    };

        //    return data;
        //}

        //private UserProfile BuildUserProfile(CreateUserProfileInputModel model)
        //{
        //    var data = new UserProfile
        //    {
        //        ClientBranch = (Int32) model.ClientBranch,
        //        ClientReference = model.ClientReference ?? 0,
        //        Postcode = model.Postcode,
        //        Surname = model.Surname,
        //        Forename = model.Forename,
        //        UserName = model.UserName,
        //        // add created date and created by field
        //        CreatedDate = DateTime.Now,
        //        CreatedBy = CurrentUserName,
        //        PaymentAmount = model.PaymentAmount,
        //        PaymentFrequency = (int) model.PaymentFrequency,
        //    };

        //    return data;
        //}
    }
}