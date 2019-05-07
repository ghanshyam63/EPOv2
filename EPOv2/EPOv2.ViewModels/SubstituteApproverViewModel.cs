namespace EPOv2.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class SubstituteApproverViewModel
    {
        public string Approver { get; set; }

        public string SelectedApprover { get; set; }

        public List<UserViewModel> ApproverList { get; set; }

        public string SelectedSubstitution { get; set; }

        public List<UserViewModel> SubtitutionList { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public string StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public string EndDate { get; set; }

        public bool isPermanent { get; set; }

     }

    public class SubstituteApproverTableViewModel
    {
        public int Id { get; set; }
        public string Approver { get; set; }

        public string Substitution { get; set; }

        public string Start { get; set; }

        public string End { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public string UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public bool isDeleted { get; set; }
    }
}