namespace EPOv2.Business.Interfaces
{
    using System.Collections.Generic;
    using System.DirectoryServices.AccountManagement;

    using DomainModel.Entities;

    public interface IAd
    {
        int cnt { get; set; }
        User CurrentUser { get; set; }

        List<string> CurrentUserRoles { get; set; }

        string GetCurrentFullName();

        int CurrentEmployeeId { get; set; }

        User GetUser(string login);

        string GetUserIdByLogin(string login);

        string GetADName();

        string GetNameByLogin(string login);

        List<string> GetAllADUsersName();

        List<string> GetCurrentUserRoles();


        /// <summary>
        /// Get All Users from AD who has the Email
        /// </summary>
        /// <returns></returns>
        List<UserPrincipal> GetAllADUsers();

        List<UserPrincipal> GetAllADUsersWithoutPhone();

        string GetADUserLoginbyName(string userName);

        string GetADUserAttributebyUserName(string userName, Attribute attr);

        List<GroupPrincipal> TryToFindUserGroups(string userName);

        string GetADUserEmpIDbyUserName(string userName);

        string GetADUserEmailbyUserName(string userName);

        string GetADNamebyLogin(string login);

        List<User> GetAllUsers();

        string GetUserIdByFullName(string firstName, string lastName);

        User GetUserByFullName(string firstName, string lastName);

        string GetUserLoginByFullName(string firstName, string lastName);

        string GetUserLoginByFullName(string fullName);

        User GetUserByFullName(string fullName);
    }
    public enum Attribute { EmployeeID, PhoneMobile, PhoneWork, whenChanged }
}