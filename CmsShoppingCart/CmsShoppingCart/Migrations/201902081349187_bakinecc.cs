namespace CmsShoppingCart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bakinecc : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblOrderDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrdersId = c.Int(nullable: false),
                        UsersId = c.Int(nullable: false),
                        ProductsId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tblOrders", t => t.OrdersId, cascadeDelete: true)
                .ForeignKey("dbo.tblProducts", t => t.ProductsId, cascadeDelete: true)
                .ForeignKey("dbo.tblUsers", t => t.UsersId, cascadeDelete: true)
                .Index(t => t.OrdersId)
                .Index(t => t.UsersId)
                .Index(t => t.ProductsId);
            
            CreateTable(
                "dbo.tblOrders",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        CreatedAt = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.tblUsers", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tblOrderDetails", "UsersId", "dbo.tblUsers");
            DropForeignKey("dbo.tblOrderDetails", "ProductsId", "dbo.tblProducts");
            DropForeignKey("dbo.tblOrderDetails", "OrdersId", "dbo.tblOrders");
            DropForeignKey("dbo.tblOrders", "UserId", "dbo.tblUsers");
            DropIndex("dbo.tblOrders", new[] { "UserId" });
            DropIndex("dbo.tblOrderDetails", new[] { "ProductsId" });
            DropIndex("dbo.tblOrderDetails", new[] { "UsersId" });
            DropIndex("dbo.tblOrderDetails", new[] { "OrdersId" });
            DropTable("dbo.tblOrders");
            DropTable("dbo.tblOrderDetails");
        }
    }
}
