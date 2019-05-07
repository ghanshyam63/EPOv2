namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class entityToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Entities", "ACN", c => c.String());
            AlterColumn("dbo.Entities", "ABN", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Entities", "ABN", c => c.Int(nullable: false));
            AlterColumn("dbo.Entities", "ACN", c => c.Int(nullable: false));
        }
    }
}
