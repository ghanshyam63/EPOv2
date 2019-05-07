namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class orderAuthor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Author_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Orders", "Author_Id");
            AddForeignKey("dbo.Orders", "Author_Id", "dbo.Users", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "Author_Id", "dbo.Users");
            DropIndex("dbo.Orders", new[] { "Author_Id" });
            DropColumn("dbo.Orders", "Author_Id");
        }
    }
}
