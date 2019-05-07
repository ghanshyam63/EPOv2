namespace EPOv2.ViewModels
{
    using System.Collections.Generic;

    using DomainModel.Entities;

    public class OrderViewModel
    {
        public int OrderId { get; set; }

        public string TempOrder { get; set; }

        public int OrderNumber { get; set; }

        public string SelectedApprover { get; set; }

        public List<UserViewModel> ApproverList { get; set; }

        public string SelectedAuthor { get; set; }

        public List<UserViewModel> AuthorList { get; set; }

        public SearchEPOResult EPOView { get; set; } //list for inhereting partial view from Search

        public int ApproverId { get; set; }

        public List<Status> StatusList { get; set; } 

        public int SelectedStatus { get; set; }

    }


}