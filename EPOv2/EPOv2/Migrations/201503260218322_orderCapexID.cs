namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class orderCapexID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "CapexId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "CapexId");
        }
    }
}
