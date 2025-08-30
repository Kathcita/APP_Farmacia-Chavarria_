using API_FarmaciaChavarria.Models;
using APP_FarmaciaChavarria.Models.ReporteModels;
using ClosedXML.Excel;
using CommunityToolkit.Mvvm.ComponentModel;
using FarmaciaChavarria.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;


namespace APP_FarmaciaChavarria.ViewModels.Reportes
{
    public partial class ReporteVentasViewModel: ObservableObject
    {
        private FacturaService _facturaService;
        private UsuarioService _usuarioService;

        public ReporteVentasViewModel(FacturaService facturaService, UsuarioService usuarioService, IJSRuntime jSRuntime)
        {
            _facturaService = facturaService;
            _usuarioService = usuarioService;

            // Inicializar fechas con datos por defecto, el rango de fecha es desde el inicio del año hasta la fecha actual
            PrimeraFecha = new DateTime(DateTime.Now.Year, 1, 1);
            UltimaFecha = DateTime.Now.Date;
            NumeroPagina = 1;
        }

        [ObservableProperty]
        private List<Factura> facturas = new();

        [ObservableProperty]
        private List<RevenueDataItem> revenueData = new();

        [ObservableProperty]
        private List<Usuario> usuarios = new();

        // Asignamos un valor por defecto para que cargue las ventas del año actual
        [ObservableProperty]
        private int año = DateTime.Now.Year;

        // Asignamos un valor por defecto para que cargue todos los datos sin importar el usuario
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

        // Paginación

        [ObservableProperty]
        private int numeroPagina;

        [ObservableProperty]
        private int totalDePaginas;


        /*Función para cargar los datos de datos de ventas y mostrarlos en la tabla*/
        public async Task CargarVentas()
        {
            try
            {
                if (PrimeraFecha > UltimaFecha)
                {
                    MensajeError = "La fecha de inicio no puede ser superior a la fecha final";
                    return;
                }

                var fechaInicioString = PrimeraFecha.ToString("yyyy-MM-dd");
                var fechaFinString = UltimaFecha.ToString("yyyy-MM-dd");

                var response = await _facturaService.ObtenerFacturasPorAñoAsync(fechaInicioString, fechaFinString, UserId, NumeroPagina);

                if (response != null && response.Facturas.Any())
                {
                    Facturas = response.Facturas;
                    TotalDePaginas = response.TotalPages;
                    MensajeError = "";
                }
                else
                {
                    MensajeError = "No se encontraron facturas";
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error: {ex.Message}";
            }
        }

        /*Función para cargar los datos de ventas y mostrarlos en el gráfico*/
        public async Task CargarVentasGrafico()
        {
            try
            {
                if (PrimeraFecha > UltimaFecha)
                {
                    return;
                }

                var fechaInicioString = PrimeraFecha.ToString("yyyy-MM-dd");
                var fechaFinString = UltimaFecha.ToString("yyyy-MM-dd");

                var response = await _facturaService.ObtenerFacturasReporteAsync(fechaInicioString, fechaFinString, UserId);
                if (response.Any())
                {
                    RevenueData = response;
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error: {ex.Message}";
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


public byte[] GenerarExcelYDescargar(List<RevenueDataItem> data, string nombreArchivo = "ReporteDeVentas")
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
        worksheet.Cell(2, 1).Value = "Fecha";
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

            worksheet.Cell(row, 1).Value = data[i].Date;
            worksheet.Cell(row, 2).Value = data[i].Revenue;

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

        public byte[] GenerarPdf(List<RevenueDataItem> data, byte[] imagenGrafico = null, string nombreArchivo = "ReporteDeVentas")
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

                        page.Header().Text("Reporte de Ventas")
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
                                    table.Cell().Text($"{item.Date:MMMM yyyy}");
                                    table.Cell().Text($"C$ {item.Revenue:#,##0.00}");
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
                                col.Item().AlignCenter().Text("Gráfico de Ventas")
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

        public byte[] GenerarPdfConPdfSharpCore(List<RevenueDataItem> data, byte[] imagenGrafico = null, string nombreArchivo = "ReporteDeVentas")
        {
            try
            {
                if (data == null || data.Count == 0)
                {
                    MensajeError = "No hay datos para generar el reporte.";
                    return null;
                }

                var usuario = UserId != 0 ? Usuarios.Find(u => u.id_usuario == UserId)?.nombre : null;

                using var stream = new MemoryStream();
                var document = new PdfDocument();

                // --- Página 1: Datos y tabla ---
                var page = document.AddPage();
                page.Size = PdfSharpCore.PageSize.A4;
                var gfx = XGraphics.FromPdfPage(page);

                double margin = 40;
                double y = margin;

                var titleFont = new XFont("Arial", 20, XFontStyle.Bold);
                var headerFont = new XFont("Arial", 12, XFontStyle.Bold);
                var normalFont = new XFont("Arial", 12, XFontStyle.Regular);

                // Título
                gfx.DrawString("Reporte de Ventas", titleFont, XBrushes.DarkBlue, new XRect(0, y, page.Width, 30), XStringFormats.TopCenter);
                y += 50;

                // Fechas y usuario
                gfx.DrawString($"Fecha Inicial: {PrimeraFecha:dd/MM/yyyy}", normalFont, XBrushes.Black, new XPoint(margin, y));
                y += 20;
                gfx.DrawString($"Fecha Final: {UltimaFecha:dd/MM/yyyy}", normalFont, XBrushes.Black, new XPoint(margin, y));
                y += 20;

                if (!string.IsNullOrEmpty(usuario))
                {
                    gfx.DrawString($"Usuario: {usuario}", normalFont, XBrushes.Black, new XPoint(margin, y));
                    y += 30;
                }

                // Tabla
                gfx.DrawString("Fecha", headerFont, XBrushes.Black, new XPoint(margin, y));
                gfx.DrawString("Total", headerFont, XBrushes.Black, new XPoint(margin + 250, y));
                y += 20;

                foreach (var item in data)
                {
                    gfx.DrawString($"{item.Date:MMMM yyyy}", normalFont, XBrushes.Black, new XPoint(margin, y));
                    gfx.DrawString($"C$ {item.Revenue:#,##0.00}", normalFont, XBrushes.Black, new XPoint(margin + 250, y));
                    y += 20;
                }

                y += 30;
                gfx.DrawString($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}", normalFont, XBrushes.Gray, new XPoint(margin, page.Height - margin));

                // --- Página 2: Imagen (gráfico) en orientación horizontal ---
                if (imagenGrafico != null)
                {
                    var page2 = document.AddPage();
                    page2.Size = PdfSharpCore.PageSize.A4;
                    page2.Orientation = PdfSharpCore.PageOrientation.Landscape;
                    var gfx2 = XGraphics.FromPdfPage(page2);

                    var imageStream = new MemoryStream(imagenGrafico);
                    var image = XImage.FromStream(() => imageStream);

                    // Título
                    gfx2.DrawString("Gráfico de Ventas", titleFont, XBrushes.DarkBlue, new XRect(0, 30, page2.Width, 30), XStringFormats.TopCenter);

                    // Imagen centrada
                    double imageMaxWidth = page2.Width - 100;
                    double imageMaxHeight = page2.Height - 100;
                    double imgWidth = image.PixelWidth;
                    double imgHeight = image.PixelHeight;
                    double ratio = Math.Min(imageMaxWidth / imgWidth, imageMaxHeight / imgHeight);

                    double drawWidth = imgWidth * ratio;
                    double drawHeight = imgHeight * ratio;

                    gfx2.DrawImage(image,
                        (page2.Width - drawWidth) / 2,
                        (page2.Height - drawHeight) / 2 + 20,
                        drawWidth,
                        drawHeight
                    );
                }

                // Guardar
                document.Save(stream, false);
                MensajeExito = "El archivo pdf se guardó exitosamente en memoria";
                MensajeError = "";
                return stream.ToArray();
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
