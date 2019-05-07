namespace EPOv2.ViewModels
{
    using System.Collections.Generic;

    public class ReceiptGroupViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool isDeleted { get; set; }

        public List<ReceiptGroupMemberViewModel> Members { get; set; }

        public List<UserViewModel> UserList { get; set; }

        public List<string> SelectedUsers { get; set; }
    }

    public class ReceiptGroupMemberViewModel
    {
        public int MemberId { get; set; }

        public bool isDeleted { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }
    }

    
}