namespace DomainModel.Entities
{
    using System;

    public class ap_mstr_oldpo
    {
        public int VoicherId { get; set; }

        public string VoucherNumber { get; set; }

        public string SupplierId { get; set; }

        public string InvoiceNumber { get; set; }

        public string UserComment { get; set; }

        public int Status { get; set; }

        public bool PORequired { get; set; }

        public string UserId { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }


    }
}