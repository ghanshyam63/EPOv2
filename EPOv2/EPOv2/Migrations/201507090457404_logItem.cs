namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class logItem : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderItemLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LineNumber = c.Int(nullable: false),
                        RevisionQty = c.Int(nullable: false),
                        Capex_Id = c.Int(),
                        DueDate = c.DateTime(nullable: false),
                        Description = c.String(),
                        Qty = c.Double(nullable: false),
                        UnitPrice = c.Double(nullable: false),
                        IsGSTInclusive = c.Boolean(nullable: false),
                        IsTaxable = c.Boolean(nullable: false),
                        TotalExTax = c.Double(nullable: false),
                        TotalTax = c.Double(nullable: false),
                        Total = c.Double(nullable: false),
                        ItemDateCreated = c.DateTime(nullable: false),
                        ItemLastModifiedDate = c.DateTime(nullable: false),
                        ItemCreatedBy = c.String(),
                        ItemLastModifiedBy = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Account_Id = c.Int(),
                        Currency_Id = c.Int(),
                        LatestOrderItem_Id = c.Int(),
                        Status_Id = c.Int(),
                        SubAccount_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.Account_Id)
                .ForeignKey("dbo.Currencies", t => t.Currency_Id)
                .ForeignKey("dbo.OrderItems", t => t.LatestOrderItem_Id)
                .ForeignKey("dbo.Status", t => t.Status_Id)
                .ForeignKey("dbo.Accounts", t => t.SubAccount_Id)
                .Index(t => t.Account_Id)
                .Index(t => t.Currency_Id)
                .Index(t => t.LatestOrderItem_Id)
                .Index(t => t.Status_Id)
                .Index(t => t.SubAccount_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderItemLogs", "SubAccount_Id", "dbo.Accounts");
            DropForeignKey("dbo.OrderItemLogs", "Status_Id", "dbo.Status");
            DropForeignKey("dbo.OrderItemLogs", "LatestOrderItem_Id", "dbo.OrderItems");
            DropForeignKey("dbo.OrderItemLogs", "Currency_Id", "dbo.Currencies");
            DropForeignKey("dbo.OrderItemLogs", "Account_Id", "dbo.Accounts");
            DropIndex("dbo.OrderItemLogs", new[] { "SubAccount_Id" });
            DropIndex("dbo.OrderItemLogs", new[] { "Status_Id" });
            DropIndex("dbo.OrderItemLogs", new[] { "LatestOrderItem_Id" });
            DropIndex("dbo.OrderItemLogs", new[] { "Currency_Id" });
            DropIndex("dbo.OrderItemLogs", new[] { "Account_Id" });
            DropTable("dbo.OrderItemLogs");
        }
    }
}
