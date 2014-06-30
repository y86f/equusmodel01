namespace a//EquusModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FoodCharacteristics", "MinServings", c => c.Double());
            AlterColumn("dbo.FoodCharacteristics", "MaxServings", c => c.Double());
            AlterColumn("dbo.N_F_Relation", "NutrientPerFood", c => c.Double());
            AlterColumn("dbo.NutrientCharacteristics", "MinNutrient", c => c.Double());
            AlterColumn("dbo.NutrientCharacteristics", "MaxNutrient", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.NutrientCharacteristics", "MaxNutrient", c => c.Double(nullable: false));
            AlterColumn("dbo.NutrientCharacteristics", "MinNutrient", c => c.Double(nullable: false));
            AlterColumn("dbo.N_F_Relation", "NutrientPerFood", c => c.Double(nullable: false));
            AlterColumn("dbo.FoodCharacteristics", "MaxServings", c => c.Double(nullable: false));
            AlterColumn("dbo.FoodCharacteristics", "MinServings", c => c.Double(nullable: false));
        }
    }
}
