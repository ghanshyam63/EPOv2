namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class matchOrder : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MatchOrders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReceviedDate = c.DateTime(nullable: false),
                        Qty = c.Double(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Order_Id = c.Int(),
                        OrderItem_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.Order_Id)
                .ForeignKey("dbo.OrderItems", t => t.OrderItem_Id)
                .Index(t => t.Order_Id)
                .Index(t => t.OrderItem_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MatchOrders", "OrderItem_Id", "dbo.OrderItems");
            DropForeignKey("dbo.MatchOrders", "Order_Id", "dbo.Orders");
            DropIndex("dbo.MatchOrders", new[] { "OrderItem_Id" });
            DropIndex("dbo.MatchOrders", new[] { "Order_Id" });
            DropTable("dbo.MatchOrders");
        }
    }
}
