using API_FarmaciaChavarria.Models;
using APP_FarmaciaChavarria.Models.ReporteModels;
using ClosedXML.Excel;
using CommunityToolkit.Mvvm.ComponentModel;
using FarmaciaChavarria.Services;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClosedXML.Excel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using DocumentFormat.OpenXml.Bibliography;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace APP_FarmaciaChavarria.ViewModels.Reportes
{
    public partial class ReporteVentasLaboratoriosViewModel : ObservableObject
    {
        private FacturaService _facturaService;
        private UsuarioService _usuarioService;

        public ReporteVentasLaboratoriosViewModel(FacturaService facturaService, UsuarioService usuarioService)
        {
            _facturaService = facturaService;
            _usuarioService = usuarioService;

            // Inicializar fechas con datos por defecto, el rango de fecha es desde el inicio del año hasta la fecha actual
            PrimeraFecha = new DateTime(DateTime.Now.Year, 1, 1);
            UltimaFecha = DateTime.Now.Date;
        }

        [ObservableProperty]
        private List<LaboratorioVentasDTO> ventasLaboratorio = new();

        [ObservableProperty]
        private List<Usuario> usuarios = new();

        [ObservableProperty]
        private int userId = 0;

        [ObservableProperty]
        private DateTime primeraFecha;

        [ObservableProperty]
        private DateTime ultimaFecha;


        [ObservableProperty]
        private string mensajeExito;

        [ObservableProperty]
        private string mensajeError;


        public async Task CargarTotalVentas()
        {
            try
            {
                if (PrimeraFecha > UltimaFecha)
                {
                    return;
                }

                var fechaInicioString = PrimeraFecha.ToString("yyyy-MM-dd");
                var fechaFinString = UltimaFecha.ToString("yyyy-MM-dd");

                var response = await _facturaService.ObtenerLaboratorioMasVentasAsync(fechaInicioString, fechaFinString, UserId);

                if (response.Any())
                {
                    VentasLaboratorio = response;
                }
                else
                {
                    MensajeError = "No se encontraron datos";
                }
            }
            catch(Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        public async Task CargarUsuarios()
        {
            try
            {
                var response = await _usuarioService.ObtenerUsuariosAsync();

                if (response.Any())
                {
                    Usuarios = response;
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error: {ex.Message}";
            }
        }

        public byte[] GenerarExcelYDescargar(List<LaboratorioVentasDTO> data, string nombreArchivo = "ReporteDeVentasPorLaboratorio")
        {
            if (data.Count == 0)
            {
                MensajeError = "No hay datos para crear un reporte";
                return [];
            }

            // Generar nombre único para evitar sobreescribir archivos
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"{nombreArchivo}_{timestamp}.xlsx";

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Reporte de Ventas");

            worksheet.Cell(1, 1).Value = $"Fecha Inicial: {PrimeraFecha.Date}";
            worksheet.Cell(1, 2).Value = $"Fecha Final: {UltimaFecha.Date}";

            var headerRangeDates = worksheet.Range("A1:C1");
            headerRangeDates.Style.Font.Bold = true;
            headerRangeDates.Style.Font.FontSize = 20;
            headerRangeDates.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Escribir encabezados
            worksheet.Cell(2, 1).Value = "Laboratorio";
            worksheet.Cell(2, 2).Value = "Ventas";

            // Estilos para encabezados
            var headerRange = worksheet.Range("A2:C2");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.FontSize = 20;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Llenar datos
            for (int i = 0; i < data.Count; i++)
            {
                var row = i + 3;

                worksheet.Cell(row, 1).Value = data[i].NombreLaboratorio;
                worksheet.Cell(row, 2).Value = data[i].TotalVentas;

                worksheet.Cell(row, 1).Style.Font.FontSize = 16;
                worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(row, 2).Style.Font.FontSize = 16;
                worksheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 2).Style.NumberFormat.Format = "C$ #,##0.00";
            }

            if (UserId != 0)
            {
                worksheet.Cell(1, 3).Value = "Usuario";
                var usuario = Usuarios.Find(u => u.id_usuario == UserId);
                worksheet.Cell(2, 3).Value = usuario?.nombre;
                worksheet.Cell(2, 3).Style.Font.FontSize = 16;
            }

            worksheet.Columns().AdjustToContents(); // Autoajustar columnas

            // Guardar archivo en memoria (en formato byte array)
            Debug.WriteLine("Guardar archivo");
            using var memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            MensajeExito = "El archivo de excel se guardó exitosamente en memoria";
            return memoryStream.ToArray();
        }

        public byte[] GenerarPdf(List<LaboratorioVentasDTO> data, byte[] imagenGrafico = null, string nombreArchivo = "ReporteDeVentasPorLaboratorio")
        {
            try
            {
                if (data.Count == 0)
                {
                    MensajeError = "No hay datos para generar el reporte.";
                    return null;
                }

                var usuario = UserId != 0 ? Usuarios.Find(u => u.id_usuario == UserId)?.nombre : null;

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);

                        page.Header().Text("Reporte de Ventas por Laboratorio")
                            .SemiBold().FontSize(20).FontColor(QuestPDF.Helpers.Colors.Blue.Darken2);

                        page.Content().Column(col =>
                        {
                            col.Spacing(10);

                            col.Item().Text($"Fecha Inicial: {PrimeraFecha:dd/MM/yyyy}");
                            col.Item().Text($"Fecha Final: {UltimaFecha:dd/MM/yyyy}");

                            if (!string.IsNullOrEmpty(usuario))
                                col.Item().Text($"Usuario: {usuario}");

                            // Tabla
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(150);
                                    columns.RelativeColumn();
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Text("Fecha").Bold();
                                    header.Cell().Text("Total").Bold();
                                });

                                foreach (var item in data)
                                {
                                    table.Cell().Text($"{item.NombreLaboratorio}");
                                    table.Cell().Text($"C$ {item.TotalVentas:#,##0.00}");
                                }
                            });
                        });

                        page.Footer().AlignCenter().Text(text =>
                        {
                            text.Span("Generado el ").SemiBold();
                            text.Span($"{DateTime.Now:dd/MM/yyyy HH:mm}");
                        });
                    });

                    // Segunda página (horizontal) solo para el gráfico
                    if (imagenGrafico != null)
                    {
                        container.Page(page =>
                        {
                            page.Size(PageSizes.A4.Landscape());
                            page.Margin(30);

                            page.Content().Column(col =>
                            {
                                col.Item().AlignCenter().Text("Gráfico de Ventas por Laboratorio")
                                    .SemiBold().FontSize(18).FontColor(QuestPDF.Helpers.Colors.Blue.Darken2);

                                // Ajusta el tamaño de la imagen para que ocupe la mayor parte de la página
                                col.Item().PaddingTop(20).Image(imagenGrafico, ImageScaling.FitArea);
                            });
                        });
                    }

                });

                MensajeExito = "El archivo pdf se guardó exitosamente en memoria";
                MensajeError = "";

                using var memoryStream = new MemoryStream();
                document.GeneratePdf(memoryStream);
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al generar el PDF: " + ex.Message);
                MensajeError = "Error al generar el PDF: " + ex.Message;
                return null;
            }
        }
    }
}
