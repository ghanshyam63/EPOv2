namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class orderCapexID2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Orders", "CapexId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "CapexId", c => c.Int(nullable: false));
        }
    }
}
