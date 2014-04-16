namespace EquusModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.N_F_Relation",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FoodID = c.Int(nullable: false),
                        NutrientID = c.Int(nullable: false),
                        NutrientPerFood = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.N_F_Relation");
        }
    }
}
