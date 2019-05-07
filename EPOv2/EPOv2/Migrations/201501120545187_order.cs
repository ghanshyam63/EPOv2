namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class order : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LineNumber = c.Int(nullable: false),
                        RevisionQty = c.Int(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        Description = c.String(),
                        Qty = c.Int(nullable: false),
                        UnitPrice = c.Int(nullable: false),
                        IsGSTInclusive = c.Boolean(nullable: false),
                        IsTaxable = c.Boolean(nullable: false),
                        TotalExTax = c.Int(nullable: false),
                        TotalTax = c.Int(nullable: false),
                        Total = c.Int(nullable: false),
                        Account_Id = c.Int(),
                        CostCentre_Id = c.Int(),
                        Currency_Id = c.Int(),
                        Status_Id = c.Int(),
                        SubAccount_Id = c.Int(),
                        Order_Id = c.Int(),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.Account_Id)
                .ForeignKey("dbo.CostCentres", t => t.CostCentre_Id)
                .ForeignKey("dbo.Currencies", t => t.Currency_Id)
                .ForeignKey("dbo.Status", t => t.Status_Id)
                .ForeignKey("dbo.Accounts", t => t.SubAccount_Id)
                .ForeignKey("dbo.Orders", t => t.Order_Id)
                .Index(t => t.Account_Id)
                .Index(t => t.CostCentre_Id)
                .Index(t => t.Currency_Id)
                .Index(t => t.Status_Id)
                .Index(t => t.SubAccount_Id)
                .Index(t => t.Order_Id);
            
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderNumber = c.Int(nullable: false),
                        TempOrderNumber = c.String(),
                        RevisionQty = c.Int(nullable: false),
                        OrderDate = c.DateTime(nullable: false),
                        CostCentre_Id = c.Int(),
                        DeliveryAddress_Id = c.Int(),
                        Entity_Id = c.Int(),
                        ReceiptGroup_Id = c.Int(),
                        Status_Id = c.Int(),
                        User_Id = c.String(maxLength: 128),
                        SupplierId = c.Int(nullable: false),
                        TransmissionMethod = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CostCentres", t => t.CostCentre_Id)
                .ForeignKey("dbo.DeliveryAddresses", t => t.DeliveryAddress_Id)
                .ForeignKey("dbo.Entities", t => t.Entity_Id)
                .ForeignKey("dbo.Groups", t => t.ReceiptGroup_Id)
                .ForeignKey("dbo.Status", t => t.Status_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.CostCentre_Id)
                .Index(t => t.DeliveryAddress_Id)
                .Index(t => t.Entity_Id)
                .Index(t => t.ReceiptGroup_Id)
                .Index(t => t.Status_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Orders", "Status_Id", "dbo.Status");
            DropForeignKey("dbo.Orders", "ReceiptGroup_Id", "dbo.Groups");
            DropForeignKey("dbo.OrderItems", "Order_Id", "dbo.Orders");
            DropForeignKey("dbo.Orders", "Entity_Id", "dbo.Entities");
            DropForeignKey("dbo.Orders", "DeliveryAddress_Id", "dbo.DeliveryAddresses");
            DropForeignKey("dbo.Orders", "CostCentre_Id", "dbo.CostCentres");
            DropForeignKey("dbo.OrderItems", "SubAccount_Id", "dbo.Accounts");
            DropForeignKey("dbo.OrderItems", "Status_Id", "dbo.Status");
            DropForeignKey("dbo.OrderItems", "Currency_Id", "dbo.Currencies");
            DropForeignKey("dbo.OrderItems", "CostCentre_Id", "dbo.CostCentres");
            DropForeignKey("dbo.OrderItems", "Account_Id", "dbo.Accounts");
            DropIndex("dbo.Orders", new[] { "User_Id" });
            DropIndex("dbo.Orders", new[] { "Status_Id" });
            DropIndex("dbo.Orders", new[] { "ReceiptGroup_Id" });
            DropIndex("dbo.Orders", new[] { "Entity_Id" });
            DropIndex("dbo.Orders", new[] { "DeliveryAddress_Id" });
            DropIndex("dbo.Orders", new[] { "CostCentre_Id" });
            DropIndex("dbo.OrderItems", new[] { "Order_Id" });
            DropIndex("dbo.OrderItems", new[] { "SubAccount_Id" });
            DropIndex("dbo.OrderItems", new[] { "Status_Id" });
            DropIndex("dbo.OrderItems", new[] { "Currency_Id" });
            DropIndex("dbo.OrderItems", new[] { "CostCentre_Id" });
            DropIndex("dbo.OrderItems", new[] { "Account_Id" });
            DropTable("dbo.Orders");
            DropTable("dbo.Status");
            DropTable("dbo.OrderItems");
        }
    }
}
