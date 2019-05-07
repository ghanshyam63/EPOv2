namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ordernumber2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "OrderNumber", c => c.Int(nullable: false, identity:true));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "OrderNumber", c => c.Int(nullable: false));
        }
    }
}
