// <copyright file="AccountController.Windows.cs" auther="Mohammad Younes">
// Copyright 2013 Mohammad Younes.
// 
// Released under the MIT license
// http://opensource.org/licenses/MIT
//
// </copyright>

using System.Web.Mvc;

namespace EPOv2.Controllers
{
    [Authorize]
  public partial class AccountController : Controller
  {
      
    
    //
    // POST: /Account/WindowsLogin
    //[AllowAnonymous]
    //[ValidateAntiForgeryToken]
    //[HttpPost]
    //public async Task<ActionResult> WindowsLogin(string userName, string returnUrl)
    //{
    //  if (!Request.LogonUserIdentity.IsAuthenticated)
    //  {
    //    return RedirectToAction("Login");
    //  }
    //  var loginInfo = GetWindowsLoginInfo();

    //  // Sign in the user with this external login provider if the user already has a login
    //  var user = await UserManager.FindAsync(loginInfo);
    //  if (user != null)
    //  {
    //    await SignInAsync(user, isPersistent: false);

    //    return RedirectToLocal(returnUrl);
    //  }
    //  else
    //  {
    //    // If the user does not have an account, then prompt the user to create an account
    //    var login = userName;
    //    if (string.IsNullOrEmpty(login))
    //    login = Request.LogonUserIdentity.Name.Split('\\')[1];
    //    var employeeId = Convert.ToInt32(_routing.GetADUserAttributebyUserName(login, AD.Attribute.EmployeeID));
    //    var phoneMobile = _routing.GetADUserAttributebyUserName(login, AD.Attribute.PhoneMobile);
    //    var phoneWork = _routing.GetADUserAttributebyUserName(login, AD.Attribute.PhoneWork);
    //    var userEmail = _routing.GetADUserEmailbyUserName(login);
    //    var userDisplayName = _routing.GetADNamebyLogin(login);
    //    var userFirstName = userDisplayName.Substring(0, userDisplayName.IndexOf(" ", System.StringComparison.Ordinal));
    //    var userLastName = userDisplayName.Substring(userDisplayName.IndexOf(" ", System.StringComparison.Ordinal)+1);
    //    var userInfo = new UserInfo()
    //    {
    //        EmployeeId = employeeId,
    //        Email = userEmail,
    //        PhoneMobile = phoneMobile,
    //        PhoneWork = phoneWork,
    //        FirstName = userFirstName,
    //        LastName = userLastName,
    //    };
        
    //    var appUser = new User()
    //    {
    //        UserName = login,
    //        EmployeeId = employeeId,
    //       // UserInfo = userInfo
    //        //UserSettings = userSetting
    //    };
    //    var result = await UserManager.CreateAsync(appUser);
    //    if (result.Succeeded)
    //    {
    //      result = await UserManager.AddLoginAsync(appUser.Id, loginInfo);
    //      if (result.Succeeded)
    //      {
    //        await SignInAsync(appUser, isPersistent: false);
    //        return RedirectToLocal(returnUrl);
    //      }
    //    }
    //    AddErrors(result);
    //    ViewBag.ReturnUrl = returnUrl;
    //    ViewBag.LoginProvider = "Windows";
    //    return View("WindowsLoginConfirmation", new WindowsLoginConfirmationViewModel { UserName = login });
    //  }
    //}

    //
    // POST: /Account/WindowsLogOff
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public void WindowsLogOff()
    //{
    //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
    //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
    //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie); 
    //}

    //
    // POST: /Account/LinkWindowsLogin
    //[AllowAnonymous]
    //[HttpPost]
    //public async Task<ActionResult> LinkWindowsLogin()
    //{
    //  string userId = HttpContext.ReadUserId();

    //  //didn't get here through handler
    //  if (string.IsNullOrEmpty(userId))
    //    return RedirectToAction("Login");

    //  HttpContext.Items.Remove("windows.userId");

    //  //not authenticated.
    //  var loginInfo = GetWindowsLoginInfo();
    //  if (loginInfo == null)
    //    return RedirectToAction("Manage");

    //  //add linked login
    //  var result = await UserManager.AddLoginAsync(userId, loginInfo);

    //  //sign the user back in.
    //  var user = await UserManager.FindByIdAsync(userId);
    //  if (user != null)
    //    await SignInAsync(user, false);

    //  if (result.Succeeded)
    //    return RedirectToAction("Manage");

    //  return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
    //}

    #region helpers
    //private UserLoginInfo GetWindowsLoginInfo()
    //{
    //  if (!Request.LogonUserIdentity.IsAuthenticated)
    //    return null;
    //  AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
      
    //  return new UserLoginInfo("Windows", Request.LogonUserIdentity.User.ToString());
    //}
    #endregion
  }
}