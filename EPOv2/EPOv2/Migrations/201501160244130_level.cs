namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class level : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Levels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.Int(nullable: false),
                        Value = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.CostCentres", "Owner_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.CostCentres", "Owner_Id");
            AddForeignKey("dbo.CostCentres", "Owner_Id", "dbo.Users", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CostCentres", "Owner_Id", "dbo.Users");
            DropIndex("dbo.CostCentres", new[] { "Owner_Id" });
            DropColumn("dbo.CostCentres", "Owner_Id");
            DropTable("dbo.Levels");
        }
    }
}
