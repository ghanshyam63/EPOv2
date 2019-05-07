namespace EPOv2.Business
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using System.IO;
    using System.Linq;
    using System.Web;

    using DomainModel.DataContext;
    using DomainModel.Entities;

    using EPOv2.Business.Interfaces;
    using EPOv2.Repositories.Interfaces;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    using Attribute = Interfaces.Attribute;

    public class AD : IAd
    {
        #region Global Variable

        public enum Direction { Up, Down};
        public int cnt { get; set; }
        public string CurrentUserName;//UserPrincipal.Current.DisplayName;
        public UserManager<User> UserManager = new UserManager<User>(new UserStore<User>(new PurchaseOrderContext()));
        public string CurrentFullName;

        public int CurrentEmployeeId { get; set; }

        public User CurrentUser { get; set; }

        private List<string> _currentUserRoles; 
        public List<string> CurrentUserRoles { get; set; }
        //{
        //    get
        //    {
        //        this._currentUserRoles = this.GetCurrentUserRoles();
        //        return this._currentUserRoles;
        //    }
        //    set
        //    {
        //        this._currentUserRoles = value;
        //    }
        //}

        private readonly IUserRepository _userRepository;
        
        private static readonly PrincipalContext PrincipalContext= new PrincipalContext(ContextType.Domain,
            "oneharvest.com.au");

       // private UserPrincipal _userPrincipal = new UserPrincipal(PrincipalContext);

     
        
        public AD(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            CurrentUserName = HttpContext.Current.User.Identity.GetUserName().Replace("ONEHARVEST\\", "");
            CurrentUser = this.GetUser(this.CurrentUserName);
            CurrentFullName = this.GetCurrentFullName();
            CurrentEmployeeId = Convert.ToInt32(GetADUserEmpIDbyUserName(CurrentUserName));
            if (CurrentUser == null)
            {
                return;
            }
            this.CurrentUserRoles = this.GetCurrentUserRoles();
            //this.LogInits();
        }

        #endregion

        private void LogInits()
        {
            
            var filePath = @"e:\instranetAd-log.txt";
            using (StreamReader sr = File.OpenText(filePath))
            {
                string s = "";
                s = sr.ReadLine();
                this.cnt = Convert.ToInt32(s);
            }
            this.cnt++;
            using (var stream = new FileStream(filePath, FileMode.Truncate))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(this.cnt++);
                }
            }
        }

        #region Main Methods

        public string GetCurrentFullName()
        {
           var currentFullName = this.GetADNamebyLogin(this.CurrentUserName);
           return currentFullName;
        }

        public List<string> GetCurrentUserRoles()
        {
            CurrentUserRoles = CurrentUser!=null ? UserManager.GetRoles(CurrentUser.Id).ToList() : new List<string>();
            return CurrentUserRoles;
        }

        public User GetUser(string login)
        {
            var user = this._userRepository.Get(x => x.UserName == login).FirstOrDefault();
            return user;
        }

        public string GetUserIdByLogin(string login)
        {
            //var result = this._db.Users.Where(x => x.UserName == login).Select(x => x.Id).FirstOrDefault();
            var result = this._userRepository.Get(x => x.UserName == login).Select(x => x.Id).FirstOrDefault();
            return result;
        }


        //public string GetUserNameBy
      

        #endregion

        #region GET something

        public string GetADName()
        {
            //var pContext = new PrincipalContext(ContextType.Domain, "oneharvest.com.au");
            var us = UserPrincipal.Current.Name + "|" + UserPrincipal.Current.DisplayName + "|" +
                     UserPrincipal.Current.SamAccountName + "|" + UserPrincipal.Current.UserPrincipalName;
            return us;
        }

        public List<string> GetAllADUsersName()
        {
            var r=this.GetAllADUsers();
            var usersLst = r.Select(x => x.Name).ToList();
            return usersLst;
        }

        /// <summary>
        /// Get All Users from AD who has the Email
        /// </summary>
        /// <returns></returns>
        public List<UserPrincipal> GetAllADUsers()
        {
            var principalContext = new PrincipalContext(ContextType.Domain, "oneharvest.com.au");
            var userPrincipal = new UserPrincipal(principalContext);
            var search = new PrincipalSearcher(userPrincipal);
            var result = search.FindAll();
            var r =
                result.Cast<UserPrincipal>().Where(principal => !String.IsNullOrWhiteSpace(principal.EmailAddress))
                    .OrderBy(x => x.Name)
                    .ToList();
            return r;
        }

        public List<UserPrincipal> GetAllADUsersWithoutPhone()
        {
            var principalContext = new PrincipalContext(ContextType.Domain, "oneharvest.com.au");
            var userPrincipal = new UserPrincipal(principalContext);
            var search = new PrincipalSearcher(userPrincipal);
            var result = search.FindAll();
            var r =
                result.Cast<UserPrincipal>().Where(principal => String.IsNullOrWhiteSpace(principal.VoiceTelephoneNumber))
                    .OrderBy(x => x.Name)
                    .ToList();
            return r;
        }

        public string GetADUserLoginbyName(string userName)
        {
            var result = UserPrincipal.FindByIdentity(PrincipalContext, userName);
            if (result != null)
            {
                return result.SamAccountName;
            }
            return String.Empty;
        }

       public string GetADUserAttributebyUserName(string userName, Attribute attr)
        {
            var attribute = String.Empty;
            switch (attr)
            {
                case Attribute.EmployeeID:
                    attribute = "employeeID";
                    break;
                case Attribute.PhoneMobile:
                    attribute = "mobile";
                    break;
                case Attribute.PhoneWork:
                    attribute = "telephoneNumber";
                    break;
                case Attribute.whenChanged:
                    attribute = "whenChanged";
                    break;
                //case Attribute.Address:
                //    attribute = "telephoneNumber";
                //    break;
                //case Attribute.PostCode:
                //    attribute = "telephoneNumber";
                //    break;
                //case Attribute.City:
                //    attribute = "telephoneNumber";
                //    break;
                //case Attribute.State:
                //    attribute = "telephoneNumber";
                //    break;
                default:
                    break;
            }
            var result = UserPrincipal.FindByIdentity(PrincipalContext, userName);
            if (result == null) return String.Empty;
            var de = result.GetUnderlyingObject() as DirectoryEntry;
            return de != null && de.Properties.Contains(attribute) ? de.Properties[attribute].Value.ToString() : String.Empty;
        }

        public List<GroupPrincipal> TryToFindUserGroups(string userName)
        {
            List<GroupPrincipal> result = new List<GroupPrincipal>();

            // establish domain context
           // PrincipalContext yourDomain = new PrincipalContext(ContextType.Domain);

            // find your user
            UserPrincipal user = UserPrincipal.FindByIdentity(PrincipalContext, userName);

            // if found - grab its groups
            if (user != null)
            {
                PrincipalSearchResult<Principal> groups = user.GetAuthorizationGroups();

                // iterate over all groups
                foreach (Principal p in groups)
                {
                    // make sure to add only group principals
                    if (p is GroupPrincipal)
                    {
                        result.Add((GroupPrincipal)p);
                    }
                }
            }
            return result;
        }

        public string GetADUserEmpIDbyUserName(string userName)
        {
            const string attribute = "employeeID";
            var result = UserPrincipal.FindByIdentity(PrincipalContext, userName);
            if (result != null)
            {
                var de = result.GetUnderlyingObject() as DirectoryEntry;
                return de != null && de.Properties.Contains(attribute) ? de.Properties[attribute].Value.ToString() : "0";
            }
            return "0";
        }

        public User GetUserByFullName(string firstName, string lastName)
        {
            var user =
                _userRepository.Get(
                    x =>
                    x.UserInfo.FirstName.ToUpper() == firstName.ToUpper()
                    && x.UserInfo.LastName.ToUpper() == lastName.ToUpper()).FirstOrDefault();
            return user;
        }

        /// <summary>
        /// Get user login by full name
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns>login, but if user doesn't exist then return full name</returns>
        public string GetUserLoginByFullName(string firstName, string lastName)
        {
            return GetUserByFullName(firstName, lastName)?.UserName ?? firstName+" "+lastName;
        }

        /// <summary>
        /// Get user Id by full user name
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns>user Id or empty string if user not exist</returns>
        public string GetUserIdByFullName(string firstName, string lastName)
        {
            return GetUserByFullName(firstName, lastName)?.Id ?? string.Empty;
        }
        /// <summary>
        /// Get user login by full name
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns>login, but if user doesn't exist then return full name</returns>
        public string GetUserLoginByFullName(string fullName)
        {
            var name = fullName.Trim().Replace(" ", "|");
            var splitChar = '|';
            if (fullName.Contains(".")) splitChar = '.';
            var arrName = name.Split(splitChar);
            return GetUserByFullName(arrName[0], arrName.Length<2? string.Empty: arrName[1])?.UserName ?? fullName;
        }

        public User GetUserByFullName(string fullName)
        {
            var name = fullName.Trim().Replace(" ", "|");
            var splitChar = '|';
            if (fullName.Contains(".")) splitChar = '.';
            var arrName = name.Split(splitChar);
            return GetUserByFullName(arrName[0], arrName.Length < 2 ? string.Empty : arrName[1]);
        }

        public string GetADUserEmailbyUserName(string userName)
        {
            var result = UserPrincipal.FindByIdentity(PrincipalContext, userName);
            return result != null ? result.EmailAddress : string.Empty;
        }

        /// <summary>
        /// Get user full name from active directory
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public string GetADNamebyLogin(string login)
        {
            var result = UserPrincipal.FindByIdentity(PrincipalContext, login);
            return result != null ? result.GivenName + " " +result.Surname : string.Empty;
        }

        /// <summary>
        /// Get user full name from database
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public string GetNameByLogin(string login)
        {
            var userName = _userRepository.Get(x => x.UserName == login).FirstOrDefault();
            return userName.GetFullName();
        }

        public List<User> GetAllUsers()
        {
            var list = this._userRepository.Get().Include(x=>x.UserInfo).Include(x=>x.UserOrderSettings).ToList();
            return list;
        }

      




        #endregion






    }
}