namespace mvc_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V4_addingColumnsinPermissionsAndAttendance : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Permissions", "permDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Attendances", "AttDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Attendances", "AttDate", c => c.String());
            DropColumn("dbo.Permissions", "permDateTime");
        }
    }
}
