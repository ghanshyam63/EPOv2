namespace EPOv2.ViewModels
{
    using System.Collections.Generic;

    public class ApproverViewModel
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string FullName { get; set; }

        public int Level { get; set; }

        public int Limit { get; set; }
    }

    public class ApproverChoiceViewModel
    {
        public int ApproverChoiceOrderId { get; set; }
        public string SelectedApprover { get; set; }

        public List<ApproverViewModel> ApproversShortList { get; set; } //enought level
        
        public List<ApproverViewModel> ApproversFullList { get; set; } // enought level + 1
    }
}