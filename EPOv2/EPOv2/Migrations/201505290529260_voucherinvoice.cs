namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class voucherinvoice : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Vouchers", "InvoiceNumber", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Vouchers", "InvoiceNumber", c => c.Int(nullable: false));
        }
    }
}
