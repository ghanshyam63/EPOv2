namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class usersettings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserOrderSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AutoApproveItemKit = c.Boolean(nullable: false),
                        DefaultSupplierId = c.Int(nullable: false),
                    DefaultCostCentre_Id = c.Int(),
                    DefaultEntity_Id = c.Int(),
                    DefaultGroup_Id = c.Int(),
                    DefualtDeliveryAddress_Id = c.Int(),
                    DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CostCentres", t => t.DefaultCostCentre_Id)
                .ForeignKey("dbo.Entities", t => t.DefaultEntity_Id)
                .ForeignKey("dbo.Groups", t => t.DefaultGroup_Id)
                .ForeignKey("dbo.DeliveryAddresses", t => t.DefualtDeliveryAddress_Id)
                .Index(t => t.DefaultCostCentre_Id)
                .Index(t => t.DefaultEntity_Id)
                .Index(t => t.DefaultGroup_Id)
                .Index(t => t.DefualtDeliveryAddress_Id);
            
            AddColumn("dbo.Users", "UserOrderSettings_Id", c => c.Int());
            CreateIndex("dbo.Users", "UserOrderSettings_Id");
            AddForeignKey("dbo.Users", "UserOrderSettings_Id", "dbo.UserOrderSettings", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "UserOrderSettings_Id", "dbo.UserOrderSettings");
            DropForeignKey("dbo.UserOrderSettings", "DefualtDeliveryAddress_Id", "dbo.DeliveryAddresses");
            DropForeignKey("dbo.UserOrderSettings", "DefaultGroup_Id", "dbo.Groups");
            DropForeignKey("dbo.UserOrderSettings", "DefaultEntity_Id", "dbo.Entities");
            DropForeignKey("dbo.UserOrderSettings", "DefaultCostCentre_Id", "dbo.CostCentres");
            DropIndex("dbo.UserOrderSettings", new[] { "DefualtDeliveryAddress_Id" });
            DropIndex("dbo.UserOrderSettings", new[] { "DefaultGroup_Id" });
            DropIndex("dbo.UserOrderSettings", new[] { "DefaultEntity_Id" });
            DropIndex("dbo.UserOrderSettings", new[] { "DefaultCostCentre_Id" });
            DropIndex("dbo.Users", new[] { "UserOrderSettings_Id" });
            DropColumn("dbo.Users", "UserOrderSettings_Id");
            DropTable("dbo.UserOrderSettings");
        }
    }
}
