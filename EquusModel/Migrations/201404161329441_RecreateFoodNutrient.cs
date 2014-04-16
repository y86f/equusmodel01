namespace EquusModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RecreateFoodNutrient : DbMigration
    {
        public override void Up()
        {
            #region manual insert
            DropTable("dbo.Foods");
            CreateTable(
               "dbo.Foods",
               c => new
               {
                   ID = c.Int(nullable: false, identity: true),
                   Description = c.String(),
               })
               .PrimaryKey(t => t.ID);

            DropTable("dbo.Nutrients");
            CreateTable(
                "dbo.Nutrients",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    Description = c.String(),
                })
                .PrimaryKey(t => t.ID);
            #endregion
        }
        
        public override void Down()
        {
        }
    }
}
