namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ordernewfields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Comment", c => c.String());
            AddColumn("dbo.Orders", "InternalComment", c => c.String());
            AddColumn("dbo.Orders", "TotalExGST", c => c.Double(nullable: false));
            AddColumn("dbo.Orders", "TotalGST", c => c.Double(nullable: false));
            AddColumn("dbo.Orders", "Total", c => c.Double(nullable: false));
            AlterColumn("dbo.OrderItems", "UnitPrice", c => c.Double(nullable: false));
            AlterColumn("dbo.OrderItems", "TotalExTax", c => c.Double(nullable: false));
            AlterColumn("dbo.OrderItems", "TotalTax", c => c.Double(nullable: false));
            AlterColumn("dbo.OrderItems", "Total", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OrderItems", "Total", c => c.Int(nullable: false));
            AlterColumn("dbo.OrderItems", "TotalTax", c => c.Int(nullable: false));
            AlterColumn("dbo.OrderItems", "TotalExTax", c => c.Int(nullable: false));
            AlterColumn("dbo.OrderItems", "UnitPrice", c => c.Int(nullable: false));
            DropColumn("dbo.Orders", "Total");
            DropColumn("dbo.Orders", "TotalGST");
            DropColumn("dbo.Orders", "TotalExGST");
            DropColumn("dbo.Orders", "InternalComment");
            DropColumn("dbo.Orders", "Comment");
        }
    }
}
