namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class roles4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.IdentityUserClaims", "UserId", "dbo.Users");
            DropIndex("dbo.IdentityUserClaims", new[] { "UserId" });
            AlterColumn("dbo.Users", "UserName", c => c.String());
            AlterColumn("dbo.Users", "EmployeeId", c => c.Int(nullable: false));
            AlterColumn("dbo.IdentityUserClaims", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.IdentityUserClaims", "UserId");
            AddForeignKey("dbo.IdentityUserClaims", "UserId", "dbo.Users", "Id");
            DropColumn("dbo.Users", "Discriminator");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            DropForeignKey("dbo.IdentityUserClaims", "UserId", "dbo.Users");
            DropIndex("dbo.IdentityUserClaims", new[] { "UserId" });
            AlterColumn("dbo.IdentityUserClaims", "UserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Users", "EmployeeId", c => c.Int());
            AlterColumn("dbo.Users", "UserName", c => c.String(nullable: false));
            CreateIndex("dbo.IdentityUserClaims", "UserId");
            AddForeignKey("dbo.IdentityUserClaims", "UserId", "dbo.Users", "Id", cascadeDelete: true);
        }
    }
}
