namespace EquusModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration3 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Foods");
            DropPrimaryKey("dbo.Nutrients");
            AlterColumn("dbo.Foods", "ID", c => c.Int(nullable: false));
            AlterColumn("dbo.Nutrients", "ID", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Foods", "ID");
            AddPrimaryKey("dbo.Nutrients", "ID");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Nutrients");
            DropPrimaryKey("dbo.Foods");
            AlterColumn("dbo.Nutrients", "ID", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Foods", "ID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Nutrients", "ID");
            AddPrimaryKey("dbo.Foods", "ID");
        }
    }
}
