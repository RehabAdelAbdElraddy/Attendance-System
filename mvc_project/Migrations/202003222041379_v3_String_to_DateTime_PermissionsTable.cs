namespace mvc_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v3_String_to_DateTime_PermissionsTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Permissions", "PermDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Permissions", "PermDate", c => c.String());
        }
    }
}
