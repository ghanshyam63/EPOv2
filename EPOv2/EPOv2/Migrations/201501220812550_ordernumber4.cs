namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ordernumber4 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Orders", "OrderNumber", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Orders", new[] { "OrderNumber" });
        }
    }
}
