namespace mvc_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v2_removeUser_NameCol : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "User_Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "User_Name", c => c.String());
        }
    }
}
