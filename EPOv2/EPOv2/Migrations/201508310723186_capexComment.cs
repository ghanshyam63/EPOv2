namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class capexComment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Capexes", "Comment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Capexes", "Comment");
        }
    }
}
