using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameTechnologyToSkill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename the junction table first (it has FKs to Technologies)
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTechnologies_Projects_ProjectId",
                table: "ProjectTechnologies");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTechnologies_Technologies_TechnologyId",
                table: "ProjectTechnologies");

            // Drop the old PK on the junction table
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectTechnologies",
                table: "ProjectTechnologies");

            // Rename the main table
            migrationBuilder.RenameTable(
                name: "Technologies",
                newName: "Skills");

            // Rename the junction table
            migrationBuilder.RenameTable(
                name: "ProjectTechnologies",
                newName: "ProjectSkills");

            // Rename TechnologyId column to SkillId in junction table
            migrationBuilder.RenameColumn(
                name: "TechnologyId",
                table: "ProjectSkills",
                newName: "SkillId");

            // Rename the index on the junction table
            migrationBuilder.RenameIndex(
                name: "IX_ProjectTechnologies_TechnologyId",
                table: "ProjectSkills",
                newName: "IX_ProjectSkills_SkillId");

            // Re-add the PK with new column name
            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectSkills",
                table: "ProjectSkills",
                columns: new[] { "ProjectId", "SkillId" });

            // Re-add foreign keys
            migrationBuilder.AddForeignKey(
                name: "FK_ProjectSkills_Projects_ProjectId",
                table: "ProjectSkills",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectSkills_Skills_SkillId",
                table: "ProjectSkills",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectSkills_Projects_ProjectId",
                table: "ProjectSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectSkills_Skills_SkillId",
                table: "ProjectSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectSkills",
                table: "ProjectSkills");

            migrationBuilder.RenameColumn(
                name: "SkillId",
                table: "ProjectSkills",
                newName: "TechnologyId");

            migrationBuilder.RenameTable(
                name: "ProjectSkills",
                newName: "ProjectTechnologies");

            migrationBuilder.RenameTable(
                name: "Skills",
                newName: "Technologies");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectSkills_SkillId",
                table: "ProjectTechnologies",
                newName: "IX_ProjectTechnologies_TechnologyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectTechnologies",
                table: "ProjectTechnologies",
                columns: new[] { "ProjectId", "TechnologyId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTechnologies_Projects_ProjectId",
                table: "ProjectTechnologies",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTechnologies_Technologies_TechnologyId",
                table: "ProjectTechnologies",
                column: "TechnologyId",
                principalTable: "Technologies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
