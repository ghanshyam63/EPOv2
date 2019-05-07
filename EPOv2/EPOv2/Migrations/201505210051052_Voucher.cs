namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Voucher : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VoucherDocuments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsAuthorised = c.Boolean(nullable: false),
                        AuthorisedDate = c.DateTime(nullable: false),
                        Reference = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Authoriser_Id = c.String(maxLength: 128),
                        DocumentType_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Authoriser_Id)
                .ForeignKey("dbo.VoucherDocumentTypes", t => t.DocumentType_Id)
                .Index(t => t.Authoriser_Id)
                .Index(t => t.DocumentType_Id);
            
            CreateTable(
                "dbo.VoucherDocumentTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Vouchers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VoucherNumber = c.Int(nullable: false),
                        SupplierCode = c.String(),
                        InvoiceNumber = c.Int(nullable: false),
                        Comment = c.String(),
                        Terms = c.String(),
                        DueDate = c.DateTime(nullable: false),
                        Amount = c.Double(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Account_Id = c.Int(),
                        CostCentre_Id = c.Int(),
                        Status_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.Account_Id)
                .ForeignKey("dbo.CostCentres", t => t.CostCentre_Id)
                .ForeignKey("dbo.VoucherStatus", t => t.Status_Id)
                .Index(t => t.Account_Id)
                .Index(t => t.CostCentre_Id)
                .Index(t => t.Status_Id);
            
            CreateTable(
                "dbo.VoucherStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Vouchers", "Status_Id", "dbo.VoucherStatus");
            DropForeignKey("dbo.Vouchers", "CostCentre_Id", "dbo.CostCentres");
            DropForeignKey("dbo.Vouchers", "Account_Id", "dbo.Accounts");
            DropForeignKey("dbo.VoucherDocuments", "DocumentType_Id", "dbo.VoucherDocumentTypes");
            DropForeignKey("dbo.VoucherDocuments", "Authoriser_Id", "dbo.Users");
            DropIndex("dbo.Vouchers", new[] { "Status_Id" });
            DropIndex("dbo.Vouchers", new[] { "CostCentre_Id" });
            DropIndex("dbo.Vouchers", new[] { "Account_Id" });
            DropIndex("dbo.VoucherDocuments", new[] { "DocumentType_Id" });
            DropIndex("dbo.VoucherDocuments", new[] { "Authoriser_Id" });
            DropTable("dbo.VoucherStatus");
            DropTable("dbo.Vouchers");
            DropTable("dbo.VoucherDocumentTypes");
            DropTable("dbo.VoucherDocuments");
        }
    }
}
