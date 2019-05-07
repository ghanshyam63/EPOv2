namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class roles3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.IdentityUserClaims", "UserId", "dbo.Users");
            DropForeignKey("dbo.IdentityUserRoles", "IdentityRole_Id", "dbo.IdentityRoles");
            DropIndex("dbo.IdentityUserClaims", new[] { "UserId" });
            DropIndex("dbo.IdentityUserRoles", new[] { "IdentityRole_Id" });
            DropColumn("dbo.IdentityUserRoles", "RoleId");
            RenameColumn(table: "dbo.IdentityUserRoles", name: "IdentityRole_Id", newName: "RoleId");
           // DropPrimaryKey("dbo.IdentityUserRoles");
            AddColumn("dbo.Users", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            //AlterColumn("dbo.Users", "EmployeeId", c => c.Int());
            AlterColumn("dbo.Users", "UserName", c => c.String(nullable: false));
            AlterColumn("dbo.IdentityUserClaims", "UserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.IdentityUserRoles", "RoleId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.IdentityRoles", "Name", c => c.String(nullable: false));
            AddPrimaryKey("dbo.IdentityUserRoles", new[] { "UserId", "RoleId" });
            CreateIndex("dbo.IdentityUserClaims", "UserId");
            CreateIndex("dbo.IdentityUserRoles", "RoleId");
            AddForeignKey("dbo.IdentityUserClaims", "UserId", "dbo.Users", "Id", cascadeDelete: true);
            AddForeignKey("dbo.IdentityUserRoles", "RoleId", "dbo.IdentityRoles", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IdentityUserRoles", "RoleId", "dbo.IdentityRoles");
            DropForeignKey("dbo.IdentityUserClaims", "UserId", "dbo.Users");
            DropIndex("dbo.IdentityUserRoles", new[] { "RoleId" });
            DropIndex("dbo.IdentityUserClaims", new[] { "UserId" });
           // DropPrimaryKey("dbo.IdentityUserRoles");
            AlterColumn("dbo.IdentityRoles", "Name", c => c.String());
            AlterColumn("dbo.IdentityUserRoles", "RoleId", c => c.String(maxLength: 128));
            AlterColumn("dbo.IdentityUserClaims", "UserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Users", "UserName", c => c.String());
            //AlterColumn("dbo.Users", "EmployeeId", c => c.Int(nullable: false));
            DropColumn("dbo.Users", "Discriminator");
            AddPrimaryKey("dbo.IdentityUserRoles", new[] { "RoleId", "UserId" });
            RenameColumn(table: "dbo.IdentityUserRoles", name: "RoleId", newName: "IdentityRole_Id");
            AddColumn("dbo.IdentityUserRoles", "RoleId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.IdentityUserRoles", "IdentityRole_Id");
            CreateIndex("dbo.IdentityUserClaims", "UserId");
            AddForeignKey("dbo.IdentityUserRoles", "IdentityRole_Id", "dbo.IdentityRoles", "Id");
            AddForeignKey("dbo.IdentityUserClaims", "UserId", "dbo.Users", "Id");
        }
    }
}
