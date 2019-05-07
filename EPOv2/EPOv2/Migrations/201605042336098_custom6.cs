namespace EPOv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class custom6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DTiles", "TileStyle", c => c.Int(nullable: false));
            AddColumn("dbo.DTiles", "TileSubType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DTiles", "TileSubType");
            DropColumn("dbo.DTiles", "TileStyle");
        }
    }
}
