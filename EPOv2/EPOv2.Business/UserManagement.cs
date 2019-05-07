namespace EPOv2.Business
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices.AccountManagement;
    using System.Linq;
    using System.Web;

    using DomainModel.DataContext;
    using DomainModel.Entities;

    using EPOv2.Business.Interfaces;
    using EPOv2.Repositories.Interfaces;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    using Attribute = EPOv2.Business.Interfaces.Attribute;

    public class UserManagement : IUserManagement
    {
        #region Fields

        public UserManager<User> UserManager = new UserManager<User>(new UserStore<User>(new PurchaseOrderContext()));

        private readonly IAd _ad;

        private readonly IDataContext _db;

        private readonly IUserRepository _userRepository;

        private readonly IRoleRepository _roleRepository;

        private readonly IMain _main;

        public UserManagement(IAd ad, IDataContext db, IUserRepository userRepository, IRoleRepository roleRepository, IMain main)
        {
            this._ad = ad;
            this._db = db;
            this._userRepository = userRepository;
            this._roleRepository = roleRepository;
            _main = main;
        }

        #endregion

        //{ get; private set; }

        #region Public Methods and Operators

        public List<User> GetAllUsers()
        {
            var usersNotAdded = new List<User>();
            var existUsers = this._userRepository.Get().ToList();
            List<UserPrincipal> userList = this._ad.GetAllADUsers();
            
            foreach (UserPrincipal u in userList)
            {
                var userDisplayName = u.DisplayName;
               
                var existingUser = existUsers.FirstOrDefault(x => x.UserName.ToUpper() == u.SamAccountName.ToUpper());
                
                if (existingUser != null)
                {
                    if (!this.IsUserNeedsForUpdates(u.SamAccountName, existingUser)) { continue; }

                    try
                    {
                        existingUser.Email = u.EmailAddress;
                        existingUser.EmployeeId = Convert.ToInt32(u.EmployeeId);
                        existingUser.UserInfo.Email = u.EmailAddress;
                        existingUser.UserInfo.EmployeeId = Convert.ToInt32(this._ad.GetADUserAttributebyUserName(u.SamAccountName, Attribute.EmployeeID));
                        existingUser.UserInfo.FirstName = userDisplayName?.Substring(
                            0,userDisplayName.IndexOf(" ", StringComparison.Ordinal)) ?? string.Empty;
                        existingUser.UserInfo.LastName =
                            userDisplayName?.Substring(userDisplayName.IndexOf(" ", StringComparison.Ordinal) + 1) ?? string.Empty;
                        existingUser.UserInfo.PhoneMobile = this._ad.GetADUserAttributebyUserName(
                            u.SamAccountName,
                            Attribute.PhoneMobile);
                        existingUser.UserInfo.PhoneWork = this._ad.GetADUserAttributebyUserName(
                            u.SamAccountName,
                            Attribute.PhoneWork);
                        existingUser.UserInfo.IsDeleted = false;
                        this._db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        _main.LogError("UserManagement.GetAllUsers().UserInfo(empId:" + u.EmployeeId + "; userName: " + u.SamAccountName + ")", e);
                    }
                }
                else
                {
                    var user = new User
                    {
                        UserName = u.SamAccountName,
                        Email = u.EmailAddress,
                        EmployeeId = Convert.ToInt32(u.EmployeeId),
                        SecurityStamp = Guid.NewGuid().ToString()
                    };
                    string empId = _ad.GetADUserAttributebyUserName(u.SamAccountName, Attribute.EmployeeID);
                   
                    if (!string.IsNullOrEmpty(empId) && empId != "0")
                    {
                        try
                        {
                            var userInfo = new UserInfo
                                               {
                                                   EmployeeId = Convert.ToInt32(empId),
                                                   Email = u.EmailAddress,
                                                   PhoneMobile =
                                                       this._ad.GetADUserAttributebyUserName(
                                                           u.SamAccountName,
                                                           Attribute.PhoneMobile),
                                                   PhoneWork =
                                                       this._ad.GetADUserAttributebyUserName(
                                                           u.SamAccountName,
                                                           Attribute.PhoneWork),
                                                   FirstName = !userDisplayName.Contains("-") ? userDisplayName.Substring(0, userDisplayName.IndexOf(" ", StringComparison.Ordinal)) : userDisplayName.Substring(0, userDisplayName.IndexOf("-", StringComparison.Ordinal)),
                                                   LastName = !userDisplayName.Contains("-") ? userDisplayName.Substring(userDisplayName.IndexOf(" ", StringComparison.Ordinal) + 1) : userDisplayName.Substring(userDisplayName.IndexOf("-", StringComparison.Ordinal) + 1),

                                                   CreatedBy = HttpContext.Current.User.Identity.Name,
                                                   DateCreated = DateTime.Now,
                                                   LastModifiedBy = HttpContext.Current.User.Identity.Name,
                                                   LastModifiedDate = DateTime.Now
                                               };
                            user.UserInfo = userInfo;
                        }
                        catch (Exception e)
                        {
                            _main.LogError("UserManagement.GetAllUsers().UserInfo(empId:" + empId + "; userName: "+ u.SamAccountName + ")", e);
                        }
                        try
                        {
                            //UserManager = new UserManager<User>(new UserStore<User>(new PurchaseOrderContext()));
                            this.UserManager.Create(user);
                        }
                        catch (Exception ex)
                        {
                            _main.LogError("UserManagement.GetAllUsers().Create(user:"+user.UserName+")", ex);
                        }
                    }
                    else
                    {
                        empId = u.EmployeeId;
                        var userI = new UserInfo { FirstName = userDisplayName };
                        user.UserInfo = userI;
                        usersNotAdded.Add(user);
                    }

                }

               
            }
            existUsers = this._userRepository.Get(x=>!x.UserInfo.IsDeleted).ToList();
            //var areDeletedUsersExist = existUsers.Count > userList.Count; //NOT 100% Accurate, maybe 99.8%
            DeleteNotExistingUsers(userList,existUsers);
            return usersNotAdded;
        }

        public void DeleteNotExistingUsers(List<UserPrincipal> userList, List<User> existUsers)
        {
            var adUserNameList = userList.OrderBy(x=>x.SamAccountName).Select(x => x.SamAccountName).ToList();
            var epoUserNameList = existUsers.OrderBy(x => x.UserName).Select(x => x.UserName).ToList();
            var usersToDelete = epoUserNameList.Except(adUserNameList,StringComparer.OrdinalIgnoreCase).ToList();
            foreach (var userName in usersToDelete)
            {
                var user = this._userRepository.Get(x => x.UserName == userName).FirstOrDefault();
                if (user == null)
                {
                    continue;
                }
                user.UserInfo.IsDeleted = true;
                user.UserInfo.LastModifiedDate = DateTime.Now;
                user.UserInfo.LastModifiedBy = HttpContext.Current.User.Identity.Name;
            }
            this._db.SaveChanges();
        }

        public bool IsUserNeedsForUpdates(string samAccountName, User existingUser)
        {
            var adWhenChanged = Convert.ToDateTime(this._ad.GetADUserAttributebyUserName(samAccountName, Attribute.whenChanged));
            if (adWhenChanged.Date > existingUser.UserInfo.LastModifiedDate.Date) return true;
            return false;
        }

        public void TestConnection()
        {
            List<User> t = this._userRepository.Get().ToList();
        }

        #endregion

        public List<string> GetAllUsersWithoutPhone()
        {
            var list = new List<string>();
            List<UserPrincipal> userList = this._ad.GetAllADUsersWithoutPhone();
            foreach (var user in userList)
            {
                //if (ad.GetADUserAttributebyUserName(user.SamAccountName, AD.Attribute.PhoneWork) == String.Empty)
               // {
                list.Add(user.GivenName + " " + user.Surname + " " + user.SamAccountName);
              //  }
            }
            return list.OrderBy(x=>x).ToList();

        }

        public List<IdentityRole> GetAllUserRoles()
        {
            return this._roleRepository.Get().ToList();
        }

        public void CreateRole(string roleName)
        {
            var role = new IdentityRole { Name = roleName };
            this._roleRepository.Add(role);
            this._db.SaveChanges();
        }

        public IdentityRole GerUserRole(string id)
        {
            return this._roleRepository.Find(id);
        }

        public void SaveUserRole(IdentityRole role)
        {
            this._roleRepository.Update(role);
            this._db.SaveChanges();
        }

        
    }
}