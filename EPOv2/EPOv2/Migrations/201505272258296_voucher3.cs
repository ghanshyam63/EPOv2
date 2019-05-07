namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class voucher3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.VoucherDocuments", "AuthorisedDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.VoucherDocuments", "AuthorisedDate", c => c.DateTime());
        }
    }
}
