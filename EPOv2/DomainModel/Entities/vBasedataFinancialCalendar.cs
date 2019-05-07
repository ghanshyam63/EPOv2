namespace DomainModel.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class vBasedataFinancialCalendar
    {
        [Key]
        public DateTime Date { get; set; }

        public string FinancialYear { get; set; }

        public string FinancialPeriod { get; set; }

        public string FinancialPeriodYear { get; set; }

        public string FinancialWeek { get; set; }

        public string FinancialWeekYear { get; set; }

        public DateTime FinancialStartingDate { get; set; }

        public DateTime FinancialEndingDate { get; set; }

    }
}