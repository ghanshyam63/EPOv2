namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class capexrouting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CapexApprovers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Role = c.String(),
                        Level = c.Int(nullable: false),
                        Limit = c.Double(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Entity_Id = c.Int(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Entities", t => t.Entity_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.Entity_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.CapexRoutes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Approver_Id = c.Int(),
                        Capex_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CapexApprovers", t => t.Approver_Id)
                .ForeignKey("dbo.Capexes", t => t.Capex_Id)
                .Index(t => t.Approver_Id)
                .Index(t => t.Capex_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CapexRoutes", "Capex_Id", "dbo.Capexes");
            DropForeignKey("dbo.CapexRoutes", "Approver_Id", "dbo.CapexApprovers");
            DropForeignKey("dbo.CapexApprovers", "User_Id", "dbo.Users");
            DropForeignKey("dbo.CapexApprovers", "Entity_Id", "dbo.Entities");
            DropIndex("dbo.CapexRoutes", new[] { "Capex_Id" });
            DropIndex("dbo.CapexRoutes", new[] { "Approver_Id" });
            DropIndex("dbo.CapexApprovers", new[] { "User_Id" });
            DropIndex("dbo.CapexApprovers", new[] { "Entity_Id" });
            DropTable("dbo.CapexRoutes");
            DropTable("dbo.CapexApprovers");
        }
    }
}
