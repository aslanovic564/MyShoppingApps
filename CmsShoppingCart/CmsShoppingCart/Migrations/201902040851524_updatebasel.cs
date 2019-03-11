namespace CmsShoppingCart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatebasel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblProducts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Slug = c.String(),
                        Description = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CategoryName = c.String(),
                        CategoryDTOId = c.Int(nullable: false),
                        ImageName = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tblCategories", t => t.CategoryDTOId, cascadeDelete: true)
                .Index(t => t.CategoryDTOId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tblProducts", "CategoryDTOId", "dbo.tblCategories");
            DropIndex("dbo.tblProducts", new[] { "CategoryDTOId" });
            DropTable("dbo.tblProducts");
        }
    }
}
