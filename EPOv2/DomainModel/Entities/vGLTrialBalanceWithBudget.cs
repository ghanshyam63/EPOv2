namespace DomainModel.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class vGLTrialBalanceWithBudget
    {
        [Column(Order = 0),Key]
        public string EntityCode { get; set; }
        [Column(Order = 1),Key]
        public string AccountCode { get; set; }
        [Column(Order = 2),Key]
        public string SubAccountCode { get; set; }
        [Column(Order = 3),Key]
        public string CostCentreCode { get; set; }

        public string Project { get; set; }

        public string AccountName { get; set; }

        public string AccountType { get; set; }
        [Column(Order = 4),Key]
        public int GLYear { get; set; }
        
        public DateTime GLPeriodDate { get; set; }
        [Column(Order = 5),Key]
        public int GLPeriod { get; set; }

        public decimal ActualPeriodOpenBalance { get; set; }

        public decimal ActualPeriodActivity { get; set; }

        public decimal ActualPeriodCloseBalance { get; set; }

        public decimal BudgetPeriod { get; set; }

        public decimal BudgetYearToDate { get; set; }

        public decimal BudgetFullYear { get; set; }

    }
}