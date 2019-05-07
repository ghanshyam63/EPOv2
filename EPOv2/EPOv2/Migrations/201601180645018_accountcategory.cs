namespace EPOv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class accountcategory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
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
                "dbo.AccountToCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Account_Id = c.Int(),
                        Category_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.Account_Id)
                .ForeignKey("dbo.AccountCategories", t => t.Category_Id)
                .Index(t => t.Account_Id)
                .Index(t => t.Category_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountToCategories", "Category_Id", "dbo.AccountCategories");
            DropForeignKey("dbo.AccountToCategories", "Account_Id", "dbo.Accounts");
            DropIndex("dbo.AccountToCategories", new[] { "Category_Id" });
            DropIndex("dbo.AccountToCategories", new[] { "Account_Id" });
            DropTable("dbo.AccountToCategories");
            DropTable("dbo.AccountCategories");
        }
    }
}
