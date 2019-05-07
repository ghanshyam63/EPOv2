namespace EPOv2.ViewModels
{
    using System.Collections.Generic;

    using DomainModel.Entities;

    using EPOv2.ViewModels.Interfaces;

    // using EPOv2.Migrations;


    public class UserViewModel:IUserViewModel
    {
        public string Id { get; set; }

        public string Login { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string Work { get; set; }

        public string EmployeeId { get; set; }

        public List<string> RolesList { get; set; }
        public bool isDeleted { get; set; }

        public int? DUserGroup { get; set; }
    }


    public class UserEditViewModel:UserViewModel
    {
        public List<string> SelectedRoles { get; set; }

        public List<UserRoleViewModel> UserRoles { get; set; }

        public string SelectedUserGroup { get; set; }

        public List<DUserGroupViewModel> UserGroups { get; set; }

        
    }

    public class UserRoleViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }    
    }

    public class UserViewModeDdl : IUserViewModel
    {
        public string Id { get; set; }

        public string FullName { get; set; }
    }

    public class UserOrderSettingsViewModel : IUserViewModel
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public int OrderSettingsId { get; set; }

        public bool AutoApproveItemKit { get; set; }
    }

    public class DUserGroupViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}