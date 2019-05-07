namespace EPOv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orderlog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    LatestOrder_Id = c.Int(),
                    Status_Id = c.Int(),
                    Subject = c.String(),
                        OrderDateCreated = c.DateTime(nullable: false),
                        OrderLastModifiedDate = c.DateTime(nullable: false),
                        OrderCreatedBy = c.String(),
                        OrderLastModifiedBy = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.LatestOrder_Id)
                .ForeignKey("dbo.Status", t => t.Status_Id)
                .Index(t => t.LatestOrder_Id)
                .Index(t => t.Status_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderLogs", "Status_Id", "dbo.Status");
            DropForeignKey("dbo.OrderLogs", "LatestOrder_Id", "dbo.Orders");
            DropIndex("dbo.OrderLogs", new[] { "Status_Id" });
            DropIndex("dbo.OrderLogs", new[] { "LatestOrder_Id" });
            DropTable("dbo.OrderLogs");
        }
    }
}
