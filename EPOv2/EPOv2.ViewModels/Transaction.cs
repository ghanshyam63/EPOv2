namespace EPOv2.ViewModels
{
    public class Transaction
    {
        public enum TransactionType {Created, Route, Approved, Received, Changes}

        public string Type { get; set; }
        public string Description { get; set; }

        public string Date { get; set; }

        public string User { get; set; }

        public int Order { get; set; }
    }

    public class CapexTransaction
    {
        public enum TransactionType { Created, Route, Approved }

        public string Type { get; set; }
        public string Description { get; set; }

        public string Date { get; set; }

        public string User { get; set; }

        public int Capex { get; set; }
    }
}