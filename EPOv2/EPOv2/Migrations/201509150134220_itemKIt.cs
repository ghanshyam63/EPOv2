namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class itemKIt : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderItemKits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Part = c.String(),
                        Description = c.String(),
                        Price = c.Double(nullable: false),
                    Account_Id = c.Int(),
                    CostCentre_Id = c.Int(),
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
                .Index(t => t.Account_Id)
                .Index(t => t.CostCentre_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderItemKits", "CostCentre_Id", "dbo.CostCentres");
            DropForeignKey("dbo.OrderItemKits", "Account_Id", "dbo.Accounts");
            DropIndex("dbo.OrderItemKits", new[] { "CostCentre_Id" });
            DropIndex("dbo.OrderItemKits", new[] { "Account_Id" });
            DropTable("dbo.OrderItemKits");
        }
    }
}
