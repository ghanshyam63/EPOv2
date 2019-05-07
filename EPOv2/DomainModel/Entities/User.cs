namespace DomainModel.Entities
{
    using Microsoft.AspNet.Identity.EntityFramework;

    public class User : IdentityUser
    {
        public int EmployeeId { get; set; }
        public virtual UserInfo UserInfo { get; set; }
        public virtual UserOrderSettings UserOrderSettings { get; set; }

        public virtual UserDashboardSettings UserDashboardSettings { get; set; }

        public string GetFullName()
        {
            if (UserInfo != null)
            {
                return UserInfo.FirstName + " " + UserInfo.LastName;
            }
            return string.Empty;
        }
    }
   
}