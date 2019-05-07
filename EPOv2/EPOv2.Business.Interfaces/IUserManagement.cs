namespace EPOv2.Business.Interfaces
{
    using System.Collections.Generic;
    using System.DirectoryServices.AccountManagement;

    using DomainModel.Entities;

    using Microsoft.AspNet.Identity.EntityFramework;

    public interface IUserManagement
    {
        List<User> GetAllUsers();

        void DeleteNotExistingUsers(List<UserPrincipal> userList, List<User> existUsers);

        bool IsUserNeedsForUpdates(string samAccountName, User existingUser);

        void TestConnection();

        List<string> GetAllUsersWithoutPhone();

        List<IdentityRole> GetAllUserRoles();

        void CreateRole(string roleName);

        IdentityRole GerUserRole(string id);

        void SaveUserRole(IdentityRole role);
    }
}