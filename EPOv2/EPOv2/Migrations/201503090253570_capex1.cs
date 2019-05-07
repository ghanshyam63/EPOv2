namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class capex1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OrderItems", "Capex_Id", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OrderItems", "Capex_Id", c => c.Int(nullable: false));
        }
    }
}
