namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class capex11 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Capexes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CapexNumber = c.String(),
                        Title = c.String(),
                        RevisionQty = c.Int(nullable: false),
                        Description = c.String(),
                        CapexType = c.String(),
                        Reference = c.String(),
                        TotalExGST = c.Double(nullable: false),
                        TotalGST = c.Double(nullable: false),
                        Total = c.Double(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Author_Id = c.String(maxLength: 128),
                        CostCentre_Id = c.Int(),
                        Entity_Id = c.Int(),
                        Owner_Id = c.String(maxLength: 128),
                        Status_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Author_Id)
                .ForeignKey("dbo.CostCentres", t => t.CostCentre_Id)
                .ForeignKey("dbo.Entities", t => t.Entity_Id)
                .ForeignKey("dbo.Users", t => t.Owner_Id)
                .ForeignKey("dbo.Status", t => t.Status_Id)
                .Index(t => t.Author_Id)
                .Index(t => t.CostCentre_Id)
                .Index(t => t.Entity_Id)
                .Index(t => t.Owner_Id)
                .Index(t => t.Status_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Capexes", "Status_Id", "dbo.Status");
            DropForeignKey("dbo.Capexes", "Owner_Id", "dbo.Users");
            DropForeignKey("dbo.Capexes", "Entity_Id", "dbo.Entities");
            DropForeignKey("dbo.Capexes", "CostCentre_Id", "dbo.CostCentres");
            DropForeignKey("dbo.Capexes", "Author_Id", "dbo.Users");
            DropIndex("dbo.Capexes", new[] { "Status_Id" });
            DropIndex("dbo.Capexes", new[] { "Owner_Id" });
            DropIndex("dbo.Capexes", new[] { "Entity_Id" });
            DropIndex("dbo.Capexes", new[] { "CostCentre_Id" });
            DropIndex("dbo.Capexes", new[] { "Author_Id" });
            DropTable("dbo.Capexes");
        }
    }
}
