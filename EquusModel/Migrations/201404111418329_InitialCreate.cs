namespace EquusModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FoodCharacteristics",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FoodID = c.Int(nullable: false),
                        Price = c.Double(nullable: false),
                        MinServings = c.Double(nullable: false),
                        MaxServings = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Foods",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.NutrientCharacteristics",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        NutrientID = c.Int(nullable: false),
                        MinNutrient = c.Double(nullable: false),
                        MaxNutrient = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Nutrients",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.V_FoodQuantity",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FoodID = c.Int(nullable: false),
                        Quantity = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.V_FoodQuantity");
            DropTable("dbo.Nutrients");
            DropTable("dbo.NutrientCharacteristics");
            DropTable("dbo.Foods");
            DropTable("dbo.FoodCharacteristics");
        }
    }
}
