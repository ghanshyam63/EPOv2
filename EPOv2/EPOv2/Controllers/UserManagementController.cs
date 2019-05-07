using System.Web.Mvc;

namespace EPOv2.Controllers
{
    using EPOv2.Business.Interfaces;
    using EPOv2.ViewModels;

    using Microsoft.AspNet.Identity.EntityFramework;

    public class UserManagementController : Controller
    {
        private readonly IUserManagement _userManagement;

        private readonly IData _data;

        public UserManagementController(IUserManagement userManagement, IData data)
        {
            this._userManagement = userManagement;
            this._data = data;
        }

        // GET: UserManagement
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllUsers()
        {
            var uList=this._userManagement.GetAllUsers();
            return this.View(uList);
        }

        #region User Roles

        //[Authorize(Roles = "Super Admin")]
        public ActionResult RolesList()
        {
            var rolesList = this._userManagement.GetAllUserRoles();
            return this.View(rolesList);
        }

        //Create Role Form
        [HttpGet]
        //[Authorize(Roles = "Super Admin")]
        public ActionResult CreateRole()
        {
            return this.View();
        }

        [HttpPost]
        //[Authorize(Roles = "Super Admin")]
        public ActionResult CreateRole(string roleName)
        {
            this._userManagement.CreateRole(roleName);
            return RedirectToAction("RolesList");
        }

        [HttpGet]
        //[Authorize(Roles = "Super Admin")]
        public ActionResult EditRole(string id)
        {
            var role = this._userManagement.GerUserRole(id);
            return this.View(role);
        }

        [HttpPost]
        //[Authorize(Roles = "Super Admin")]
        public ActionResult EditRole(IdentityRole role)
        {
            this._userManagement.SaveUserRole(role);
            return RedirectToAction("RolesList");
        }
        #endregion

        [HttpGet]
        public ActionResult EditUser(string id)
        {
            var user = _data.GetUserEditViewModel(id);
            return this.View(user);
        }

        [HttpPost]
        public ActionResult EditUser(UserEditViewModel model)
        {
            _data.SaveUser(model);
            return RedirectToAction("ManageUsers", "Maintenance");
        }

        [HttpGet]
        public ActionResult EditUserOrderSettings(string id)
        {
            var userSettings = _data.GetUserOrderSettingsViewModel(id);
            return this.View(userSettings);
        }

        [HttpPost]
        public ActionResult EditUserOrderSettings(UserOrderSettingsViewModel model)
        {
            _data.SaveUserOrderSettings(model);
            return RedirectToAction("ManageUsers", "Maintenance");
        }
    }
}