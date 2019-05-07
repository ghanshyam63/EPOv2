namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class approvechain : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Approvers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Level = c.Int(nullable: false),
                        Limit = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Routes",
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
                        Order_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Approvers", t => t.Approver_Id)
                .ForeignKey("dbo.Orders", t => t.Order_Id)
                .Index(t => t.Approver_Id)
                .Index(t => t.Order_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Routes", "Order_Id", "dbo.Orders");
            DropForeignKey("dbo.Routes", "Approver_Id", "dbo.Approvers");
            DropForeignKey("dbo.Approvers", "User_Id", "dbo.Users");
            DropIndex("dbo.Routes", new[] { "Order_Id" });
            DropIndex("dbo.Routes", new[] { "Approver_Id" });
            DropIndex("dbo.Approvers", new[] { "User_Id" });
            DropTable("dbo.Routes");
            DropTable("dbo.Approvers");
        }
    }
}
