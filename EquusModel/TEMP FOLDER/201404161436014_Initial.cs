namespace a//EquusModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FoodCharacteristics",
                c => new
                    {
                        FoodID = c.Int(nullable: false),
                        Price = c.Double(nullable: false),
                        MinServings = c.Double(nullable: false),
                        MaxServings = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.FoodID);
            
            CreateTable(
                "dbo.Foods",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.N_F_Relation",
                c => new
                    {
                        FoodID = c.Int(nullable: false),
                        NutrientID = c.Int(nullable: false),
                        NutrientPerFood = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.FoodID, t.NutrientID });
            
            CreateTable(
                "dbo.NutrientCharacteristics",
                c => new
                    {
                        NutrientID = c.Int(nullable: false),
                        MinNutrient = c.Double(nullable: false),
                        MaxNutrient = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.NutrientID);
            
            CreateTable(
                "dbo.Nutrients",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.V_FoodQuantity",
                c => new
                    {
                        FoodID = c.Int(nullable: false),
                        Quantity = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.FoodID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.V_FoodQuantity");
            DropTable("dbo.Nutrients");
            DropTable("dbo.NutrientCharacteristics");
            DropTable("dbo.N_F_Relation");
            DropTable("dbo.Foods");
            DropTable("dbo.FoodCharacteristics");
        }
    }
}
