using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class InicialConGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lenguajes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CodigoJudge0 = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lenguajes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Problemas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    LimiteTiempo = table.Column<float>(type: "real", nullable: false),
                    LimiteMemoria = table.Column<int>(type: "integer", nullable: false),
                    EsEliminado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Problemas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CasosDePrueba",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Entrada = table.Column<string>(type: "text", nullable: false),
                    SalidaEsperada = table.Column<string>(type: "text", nullable: false),
                    EsOculto = table.Column<bool>(type: "boolean", nullable: false),
                    ProblemaId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasosDePrueba", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CasosDePrueba_Problemas_ProblemaId",
                        column: x => x.ProblemaId,
                        principalTable: "Problemas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Envios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CodigoFuente = table.Column<string>(type: "text", nullable: false),
                    LenguajeId = table.Column<int>(type: "integer", nullable: false),
                    EntradaEstandar = table.Column<string>(type: "text", nullable: true),
                    Token = table.Column<string>(type: "text", nullable: true),
                    VeredictoGlobal = table.Column<string>(type: "text", nullable: true),
                    ProblemaId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Envios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Envios_Problemas_ProblemaId",
                        column: x => x.ProblemaId,
                        principalTable: "Problemas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DetalleEnvios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EnvioId = table.Column<Guid>(type: "uuid", nullable: false),
                    CasoDePruebaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    Veredicto = table.Column<string>(type: "text", nullable: true),
                    Tiempo = table.Column<float>(type: "real", nullable: true),
                    Memoria = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleEnvios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetalleEnvios_CasosDePrueba_CasoDePruebaId",
                        column: x => x.CasoDePruebaId,
                        principalTable: "CasosDePrueba",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetalleEnvios_Envios_EnvioId",
                        column: x => x.EnvioId,
                        principalTable: "Envios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CasosDePrueba_ProblemaId",
                table: "CasosDePrueba",
                column: "ProblemaId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleEnvios_CasoDePruebaId",
                table: "DetalleEnvios",
                column: "CasoDePruebaId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleEnvios_EnvioId",
                table: "DetalleEnvios",
                column: "EnvioId");

            migrationBuilder.CreateIndex(
                name: "IX_Envios_ProblemaId",
                table: "Envios",
                column: "ProblemaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetalleEnvios");

            migrationBuilder.DropTable(
                name: "Lenguajes");

            migrationBuilder.DropTable(
                name: "CasosDePrueba");

            migrationBuilder.DropTable(
                name: "Envios");

            migrationBuilder.DropTable(
                name: "Problemas");
        }
    }
}
