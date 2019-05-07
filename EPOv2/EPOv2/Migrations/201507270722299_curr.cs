namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class curr : DbMigration
    {
        public override void Up()
        {
            
            AddColumn("dbo.OrderItems", "CurrencyRate", c => c.Double(nullable: false, defaultValue: 1));
            AddColumn("dbo.OrderItemLogs", "CurrencyRate", c => c.Double(nullable: false, defaultValue:1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderItemLogs", "CurrencyRate");
            DropColumn("dbo.OrderItems", "CurrencyRate");
        }
    }
}
