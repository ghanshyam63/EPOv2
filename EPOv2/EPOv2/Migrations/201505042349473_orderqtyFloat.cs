namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class orderqtyFloat : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OrderItems", "Qty", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OrderItems", "Qty", c => c.Int(nullable: false));
        }
    }
}
