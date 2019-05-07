namespace EPOv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class vroute : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VoucherRoutes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                    Approver_Id = c.Int(),
                    Voucher_Id = c.Int(),
                    DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Approvers", t => t.Approver_Id)
                .ForeignKey("dbo.Vouchers", t => t.Voucher_Id)
                .Index(t => t.Approver_Id)
                .Index(t => t.Voucher_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VoucherRoutes", "Voucher_Id", "dbo.Vouchers");
            DropForeignKey("dbo.VoucherRoutes", "Approver_Id", "dbo.Approvers");
            DropIndex("dbo.VoucherRoutes", new[] { "Voucher_Id" });
            DropIndex("dbo.VoucherRoutes", new[] { "Approver_Id" });
            DropTable("dbo.VoucherRoutes");
        }
    }
}
