namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class orderCapexID3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Capex_Id", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "Capex_Id");
        }
    }
}
