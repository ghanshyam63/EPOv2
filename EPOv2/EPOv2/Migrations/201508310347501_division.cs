namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class division : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CapexApprovers", "Entity_Id", "dbo.Entities");
            DropIndex("dbo.CapexApprovers", new[] { "Entity_Id" });
            CreateTable(
                "dbo.Divisions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CostCentreRangeFrom = c.Int(nullable: false),
                        CostCentreRangeTo = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Owner_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Owner_Id)
                .Index(t => t.Owner_Id);
            
            AddColumn("dbo.CapexApprovers", "Division_Id", c => c.Int());
            CreateIndex("dbo.CapexApprovers", "Division_Id");
            AddForeignKey("dbo.CapexApprovers", "Division_Id", "dbo.Divisions", "Id");
            DropColumn("dbo.CapexApprovers", "Entity_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CapexApprovers", "Entity_Id", c => c.Int());
            DropForeignKey("dbo.CapexApprovers", "Division_Id", "dbo.Divisions");
            DropForeignKey("dbo.Divisions", "Owner_Id", "dbo.Users");
            DropIndex("dbo.Divisions", new[] { "Owner_Id" });
            DropIndex("dbo.CapexApprovers", new[] { "Division_Id" });
            DropColumn("dbo.CapexApprovers", "Division_Id");
            DropTable("dbo.Divisions");
            CreateIndex("dbo.CapexApprovers", "Entity_Id");
            AddForeignKey("dbo.CapexApprovers", "Entity_Id", "dbo.Entities", "Id");
        }
    }
}
