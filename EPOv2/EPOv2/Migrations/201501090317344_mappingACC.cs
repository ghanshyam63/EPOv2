namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class mappingACC : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountToCostCentres",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
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
            
            CreateTable(
                "dbo.CostCentreToEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CostCentre_Id = c.Int(),
                        Entity_Id = c.Int(),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CostCentres", t => t.CostCentre_Id)
                .ForeignKey("dbo.Entities", t => t.Entity_Id)
                .Index(t => t.CostCentre_Id)
                .Index(t => t.Entity_Id);
            
            AddColumn("dbo.Accounts", "Type", c => c.Int(nullable: false));
            DropColumn("dbo.Accounts", "ParentId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Accounts", "ParentId", c => c.Int(nullable: false));
            DropForeignKey("dbo.CostCentreToEntities", "Entity_Id", "dbo.Entities");
            DropForeignKey("dbo.CostCentreToEntities", "CostCentre_Id", "dbo.CostCentres");
            DropForeignKey("dbo.AccountToCostCentres", "CostCentre_Id", "dbo.CostCentres");
            DropForeignKey("dbo.AccountToCostCentres", "Account_Id", "dbo.Accounts");
            DropIndex("dbo.CostCentreToEntities", new[] { "Entity_Id" });
            DropIndex("dbo.CostCentreToEntities", new[] { "CostCentre_Id" });
            DropIndex("dbo.AccountToCostCentres", new[] { "CostCentre_Id" });
            DropIndex("dbo.AccountToCostCentres", new[] { "Account_Id" });
            DropColumn("dbo.Accounts", "Type");
            DropTable("dbo.CostCentreToEntities");
            DropTable("dbo.AccountToCostCentres");
        }
    }
}
