namespace ProyectoDistri2.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddPasswordToUsuario : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Usuarios", "Password", c => c.String(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Usuarios", "Password");
        }
    }
}
