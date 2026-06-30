using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSolarScreenToComponentSubtype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:database.optical_component_subtype", "acid_etched_glass,applied_film,cellular_shade,chromogenic,coated,coating,diffusing_shade,embedded_coating,film,fritted_glass,interlayer,laminate,monolithic,perforated_screen,pleated_shade,roller_shade,roman_shade,sandblasted_glass,shade_material,solar_screen,venetian_blind,vertical_louver,woven_shade")
                .OldAnnotation("Npgsql:Enum:database.optical_component_subtype", "acid_etched_glass,applied_film,cellular_shade,chromogenic,coated,coating,diffusing_shade,embedded_coating,film,fritted_glass,interlayer,laminate,monolithic,perforated_screen,pleated_shade,roller_shade,roman_shade,sandblasted_glass,shade_material,venetian_blind,vertical_louver,woven_shade");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:database.optical_component_subtype", "acid_etched_glass,applied_film,cellular_shade,chromogenic,coated,coating,diffusing_shade,embedded_coating,film,fritted_glass,interlayer,laminate,monolithic,perforated_screen,pleated_shade,roller_shade,roman_shade,sandblasted_glass,shade_material,venetian_blind,vertical_louver,woven_shade")
                .OldAnnotation("Npgsql:Enum:database.optical_component_subtype", "acid_etched_glass,applied_film,cellular_shade,chromogenic,coated,coating,diffusing_shade,embedded_coating,film,fritted_glass,interlayer,laminate,monolithic,perforated_screen,pleated_shade,roller_shade,roman_shade,sandblasted_glass,shade_material,solar_screen,venetian_blind,vertical_louver,woven_shade");
        }
    }
}