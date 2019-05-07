namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class init2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        ParentId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DeliveryAddresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Address = c.String(),
                        City = c.String(),
                        PostCode = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        State_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.States", t => t.State_Id)
                .Index(t => t.State_Id);
            
            CreateTable(
                "dbo.States",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShortName = c.String(),
                        Name = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GroupMembers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Group_Id = c.Int(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Groups", t => t.Group_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.Group_Id)
                .Index(t => t.User_Id);
            
            AddColumn("dbo.UserInfoes", "Address", c => c.String());
            AddColumn("dbo.UserInfoes", "City", c => c.String());
            AddColumn("dbo.UserInfoes", "PostCode", c => c.Int(nullable: false));
            AddColumn("dbo.UserInfoes", "State_Id", c => c.Int());
            CreateIndex("dbo.UserInfoes", "State_Id");
            AddForeignKey("dbo.UserInfoes", "State_Id", "dbo.States", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupMembers", "User_Id", "dbo.Users");
            DropForeignKey("dbo.UserInfoes", "State_Id", "dbo.States");
            DropForeignKey("dbo.GroupMembers", "Group_Id", "dbo.Groups");
            DropForeignKey("dbo.DeliveryAddresses", "State_Id", "dbo.States");
            DropIndex("dbo.UserInfoes", new[] { "State_Id" });
            DropIndex("dbo.GroupMembers", new[] { "User_Id" });
            DropIndex("dbo.GroupMembers", new[] { "Group_Id" });
            DropIndex("dbo.DeliveryAddresses", new[] { "State_Id" });
            DropColumn("dbo.UserInfoes", "State_Id");
            DropColumn("dbo.UserInfoes", "PostCode");
            DropColumn("dbo.UserInfoes", "City");
            DropColumn("dbo.UserInfoes", "Address");
            DropTable("dbo.GroupMembers");
            DropTable("dbo.States");
            DropTable("dbo.DeliveryAddresses");
            DropTable("dbo.Accounts");
        }
    }
}
