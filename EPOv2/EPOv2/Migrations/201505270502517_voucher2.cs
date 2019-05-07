namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class voucher2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VoucherDocuments", "Voucher_Id", c => c.Int());
            AlterColumn("dbo.VoucherDocuments", "AuthorisedDate", c => c.DateTime());
            CreateIndex("dbo.VoucherDocuments", "Voucher_Id");
            AddForeignKey("dbo.VoucherDocuments", "Voucher_Id", "dbo.Vouchers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VoucherDocuments", "Voucher_Id", "dbo.Vouchers");
            DropIndex("dbo.VoucherDocuments", new[] { "Voucher_Id" });
            AlterColumn("dbo.VoucherDocuments", "AuthorisedDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.VoucherDocuments", "Voucher_Id");
        }
    }
}
