using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class SistemaCompetencias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompetenciaId",
                table: "Envios",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEnvio",
                table: "Envios",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Competencias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaCongelamiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VerVeredictoDuranteFreeze = table.Column<bool>(type: "boolean", nullable: false),
                    EsPublica = table.Column<bool>(type: "boolean", nullable: false),
                    PropietarioId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Competencias_Usuarios_PropietarioId",
                        column: x => x.PropietarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Aclaraciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompetenciaId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProblemaId = table.Column<Guid>(type: "uuid", nullable: true),
                    Pregunta = table.Column<string>(type: "text", nullable: false),
                    Respuesta = table.Column<string>(type: "text", nullable: true),
                    EsGlobal = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aclaraciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Aclaraciones_Competencias_CompetenciaId",
                        column: x => x.CompetenciaId,
                        principalTable: "Competencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Aclaraciones_Problemas_ProblemaId",
                        column: x => x.ProblemaId,
                        principalTable: "Problemas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Aclaraciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompetenciaGestores",
                columns: table => new
                {
                    CompetenciaId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rol = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetenciaGestores", x => new { x.CompetenciaId, x.UsuarioId });
                    table.ForeignKey(
                        name: "FK_CompetenciaGestores_Competencias_CompetenciaId",
                        column: x => x.CompetenciaId,
                        principalTable: "Competencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompetenciaGestores_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompetenciaParticipantes",
                columns: table => new
                {
                    CompetenciaId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaInscripcion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetenciaParticipantes", x => new { x.CompetenciaId, x.UsuarioId });
                    table.ForeignKey(
                        name: "FK_CompetenciaParticipantes_Competencias_CompetenciaId",
                        column: x => x.CompetenciaId,
                        principalTable: "Competencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompetenciaParticipantes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompetenciaProblemas",
                columns: table => new
                {
                    CompetenciaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProblemaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Letra = table.Column<string>(type: "text", nullable: false),
                    ColorGlobo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetenciaProblemas", x => new { x.CompetenciaId, x.ProblemaId });
                    table.ForeignKey(
                        name: "FK_CompetenciaProblemas_Competencias_CompetenciaId",
                        column: x => x.CompetenciaId,
                        principalTable: "Competencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompetenciaProblemas_Problemas_ProblemaId",
                        column: x => x.ProblemaId,
                        principalTable: "Problemas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Envios_CompetenciaId",
                table: "Envios",
                column: "CompetenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Aclaraciones_CompetenciaId",
                table: "Aclaraciones",
                column: "CompetenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Aclaraciones_ProblemaId",
                table: "Aclaraciones",
                column: "ProblemaId");

            migrationBuilder.CreateIndex(
                name: "IX_Aclaraciones_UsuarioId",
                table: "Aclaraciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetenciaGestores_UsuarioId",
                table: "CompetenciaGestores",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetenciaParticipantes_UsuarioId",
                table: "CompetenciaParticipantes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetenciaProblemas_ProblemaId",
                table: "CompetenciaProblemas",
                column: "ProblemaId");

            migrationBuilder.CreateIndex(
                name: "IX_Competencias_PropietarioId",
                table: "Competencias",
                column: "PropietarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Envios_Competencias_CompetenciaId",
                table: "Envios",
                column: "CompetenciaId",
                principalTable: "Competencias",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Envios_Competencias_CompetenciaId",
                table: "Envios");

            migrationBuilder.DropTable(
                name: "Aclaraciones");

            migrationBuilder.DropTable(
                name: "CompetenciaGestores");

            migrationBuilder.DropTable(
                name: "CompetenciaParticipantes");

            migrationBuilder.DropTable(
                name: "CompetenciaProblemas");

            migrationBuilder.DropTable(
                name: "Competencias");

            migrationBuilder.DropIndex(
                name: "IX_Envios_CompetenciaId",
                table: "Envios");

            migrationBuilder.DropColumn(
                name: "CompetenciaId",
                table: "Envios");

            migrationBuilder.DropColumn(
                name: "FechaEnvio",
                table: "Envios");
        }
    }
}
