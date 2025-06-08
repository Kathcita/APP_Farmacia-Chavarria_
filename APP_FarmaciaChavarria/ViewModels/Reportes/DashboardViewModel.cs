using APP_FarmaciaChavarria.Models.ReporteModels;
using ClosedXML.Excel;
using CommunityToolkit.Mvvm.ComponentModel;
using DocumentFormat.OpenXml.Spreadsheet;
using FarmaciaChavarria.Services;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colors = QuestPDF.Helpers.Colors;

namespace APP_FarmaciaChavarria.ViewModels.Reportes
{
    public partial class DashboardViewModel : ObservableObject
    {

        private readonly FacturaService _facturaService;

        public DashboardViewModel(FacturaService facturaService)
        {
            _facturaService = facturaService;
            DashboardData = new DashboardData
            {
                MedicamentosDisponibles = 0,
                MedicamentosEscasos = 0,
                VentasDelMes = 0,
                MedicamentosTotales = 0,
                TotalFacturasDelMes = 0,
                TotalMedicamentosVendidosDelMes = 0,
                CategoriasTotales = 0,
                TotalProveedores = 0,
                TotalUsuarios = 0,
                ProductoMasVendido = "No Disponible",
                EstadoInventario = "No Disponible"
            };
        }

        [ObservableProperty]
        public DashboardData dashboardData;

        [ObservableProperty]
        public bool cargando;

        [ObservableProperty]
        public string mensajeExito;

        [ObservableProperty]
        public string mensajeError;

        public async Task CargarDatos()
        {
            try
            {
                Cargando = true;
                var response = await _facturaService.ObtenerDatosDashboard();

                if (response != null)
                {
                    DashboardData = response;
                }
                else
                {
                    DashboardData = new DashboardData
                    {
                        MedicamentosDisponibles = 0,
                        MedicamentosEscasos = 0,
                        VentasDelMes = 0,
                        MedicamentosTotales = 0,
                        TotalFacturasDelMes = 0,
                        TotalMedicamentosVendidosDelMes = 0,
                        CategoriasTotales = 0,
                        TotalProveedores = 0,
                        TotalUsuarios = 0,
                        ProductoMasVendido = "No Disponible",
                        EstadoInventario = "No Disponible"
                    };
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                Cargando = false;
            }
        }

        public byte[]? GenerarExcelYDescargar(DashboardData data, string nombreArchivo = "ReporteGeneral")
        {
            try
            {
                var date = DateTime.Now;

                if (data.EstadoInventario == "")
                {
                    return null;
                }
                else
                {
                    // Generar nombre único para evitar sobreescribir archivos
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string fileName = $"{nombreArchivo}_{timestamp}.xlsx";

                    using var workbook = new XLWorkbook();
                    var worksheet = workbook.Worksheets.Add("Reporte General");

                    worksheet.Cell(1, 1).Value = $"Fecha Inicial: {new DateTime(date.Year, date.Month, 1)}";
                    worksheet.Cell(1, 3).Value = $"Fecha Final: {new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month))}";

                    var headerRangeDates = worksheet.Range("A1:C1");
                    headerRangeDates.Style.Font.Bold = true;
                    headerRangeDates.Style.Font.FontSize = 20;
                    headerRangeDates.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                    // Primera Fila de datos
                    // Escribir encabezados
                    worksheet.Cell(3, 1).Value = "Total de Facturas";
                    worksheet.Cell(3, 2).Value = "Total de Ventas";
                    worksheet.Cell(3, 3).Value = "Total de Medicamentos Vendidos";

                    // Estilos para encabezados para la primera Fila
                    var headerRange = worksheet.Range("A3:C3");
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Font.FontSize = 20;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Llenar datos

                    worksheet.Cell(4, 1).Value = DashboardData.TotalFacturasDelMes;
                    worksheet.Cell(4, 2).Value = DashboardData.VentasDelMes;
                    worksheet.Cell(4, 3).Value = DashboardData.TotalMedicamentosVendidosDelMes;

                    var firstRowRange = worksheet.Range("A4:C4");
                    firstRowRange.Style.Font.FontSize = 16;
                    firstRowRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell(4, 2).Style.Font.FontSize = 16;
                    worksheet.Cell(4, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(4, 2).Style.NumberFormat.Format = "C$ #,##0.00";


                    // Segunda Fila de datos

                    // Estilos para encabezados para la segunda Fila
                    var secondHeaderRange = worksheet.Range("A6:C6");
                    secondHeaderRange.Style.Font.Bold = true;
                    secondHeaderRange.Style.Font.FontSize = 20;
                    secondHeaderRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell(6, 1).Value = "Total de Medicamentos Disponibles";
                    worksheet.Cell(6, 2).Value = "Total de Medicamentos Escasos";
                    worksheet.Cell(6, 3).Value = "Producto Más Vendido Del Mes";

                    // Estilos para datos de la segunda Fila
                    var secondRowRange = worksheet.Range("A7:C7");
                    secondRowRange.Style.Font.FontSize = 16;
                    secondRowRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell(7, 1).Value = DashboardData.MedicamentosDisponibles;
                    worksheet.Cell(7, 2).Value = DashboardData.MedicamentosEscasos;
                    worksheet.Cell(7, 3).Value = DashboardData.ProductoMasVendido;

                    // Tercera Fila

                    // Estilos para encabezados para la tercera Fila
                    var thirdHeaderRange = worksheet.Range("A9:C9");
                    thirdHeaderRange.Style.Font.Bold = true;
                    thirdHeaderRange.Style.Font.FontSize = 20;
                    thirdHeaderRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell(9, 1).Value = "Total de Categorías";
                    worksheet.Cell(9, 2).Value = "Total de Proveedores";
                    worksheet.Cell(9, 3).Value = "Total de Usuarios";

                    // Estilos para datos de la tercera Fila
                    var thirdRowRange = worksheet.Range("A10:C10");
                    thirdRowRange.Style.Font.FontSize = 16;
                    thirdRowRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell(10, 1).Value = DashboardData.CategoriasTotales;
                    worksheet.Cell(10, 2).Value = DashboardData.TotalProveedores;
                    worksheet.Cell(10, 3).Value = DashboardData.TotalUsuarios;

                    // Cuarta Fila

                    // Estilos para encabezados para la tercera Fila
                    var fourthHeaderRange = worksheet.Range("A12:C12");
                    fourthHeaderRange.Style.Font.Bold = true;
                    fourthHeaderRange.Style.Font.FontSize = 20;
                    fourthHeaderRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell(12, 1).Value = "Medicamentos totales";
                    worksheet.Cell(12, 3).Value = "Estado del inventario";

                    // Estilos para datos de la cuarta Fila
                    var fourthRowRange = worksheet.Range("A13:C13");
                    fourthRowRange.Style.Font.FontSize = 16;
                    fourthRowRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell(13, 1).Value = DashboardData.MedicamentosTotales;
                    worksheet.Cell(13, 3).Value = DashboardData.EstadoInventario;

                    worksheet.Columns().AdjustToContents(); // Autoajustar columnas

                    // Guardar archivo en memoria (en formato byte array)
                    using var memoryStream = new MemoryStream();
                    workbook.SaveAs(memoryStream);
                    //MensajeExito = "El archivo de excel se guardó exitosamente en memoria";
                    return memoryStream.ToArray();
                }
            }
            catch (Exception e)
            {
                MensajeError = $"Error: {e.Message}";
                return null;
            }

        }

        public byte[]? GenerarPdf(DashboardData data, byte[] imagenGrafico = null, string nombreArchivo = "ReporteGeneral")
        {
            try
            {
                Debug.WriteLine($"ImagenViewModel: {imagenGrafico}");
                var date = DateTime.Now;

                if (data.EstadoInventario == "")
                {
                    return null;
                }
                else
                {
                    var document = Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Margin(30);

                            page.Header().Text("Reporte General")
                                .SemiBold().FontSize(20).FontColor(Colors.Blue.Darken2);

                            page.Content().Column(col =>
                            {
                                col.Spacing(35);

                                // Fecha Inicial y Fecha Final
                                col.Item().Row(row =>
                                {
                                    row.Spacing(25);
                                    row.RelativeItem().Text($"Fecha Inicial: {new DateTime(date.Year, date.Month, 1):dd/MM/yyyy}")
                                        .FontSize(14).Bold();
                                    row.RelativeItem().Text($"Fecha Final: {new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month)):dd/MM/yyyy}")
                                        .FontSize(14).Bold();
                                });

                                // Primera Fila: Totales de facturas, ventas y medicamentos vendidos
                                col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Text("Total de Facturas").Bold().FontSize(16).AlignCenter();
                                        header.Cell().Text("Total de Ventas").Bold().FontSize(16).AlignCenter();
                                        header.Cell().Text("Total de Medicamentos Vendidos").Bold().FontSize(16).AlignCenter();
                                    });

                                    table.Cell().Text($"{DashboardData.TotalFacturasDelMes}").FontSize(14).AlignCenter();
                                    table.Cell().Text($"C$ {DashboardData.VentasDelMes:#,##0.00}").FontSize(14).AlignCenter();
                                    table.Cell().Text($"{DashboardData.TotalMedicamentosVendidosDelMes}").FontSize(14).AlignCenter();
                                });

                                // Segunda Fila: Medicamentos disponibles, escasos y producto más vendido
                                col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Text("Total de Medicamentos Disponibles").Bold().FontSize(16).AlignCenter();
                                        header.Cell().Text("Total de Medicamentos Escasos").Bold().FontSize(16).AlignCenter();
                                        header.Cell().Text("Producto Más Vendido Del Mes").Bold().FontSize(16).AlignCenter();
                                    });

                                    table.Cell().Text($"{DashboardData.MedicamentosDisponibles}").FontSize(14).AlignCenter();
                                    table.Cell().Text($"{DashboardData.MedicamentosEscasos}").FontSize(14).AlignCenter();
                                    table.Cell().Text($"{DashboardData.ProductoMasVendido}").FontSize(14).AlignCenter();
                                });

                                // Tercera Fila: Categorías, Proveedores, Usuarios
                                col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Text("Total de Categorías").Bold().FontSize(16).AlignCenter();
                                        header.Cell().Text("Total de Proveedores").Bold().FontSize(16).AlignCenter();
                                        header.Cell().Text("Total de Usuarios").Bold().FontSize(16).AlignCenter();
                                    });

                                    table.Cell().Text($"{DashboardData.CategoriasTotales}").FontSize(14).AlignCenter();
                                    table.Cell().Text($"{DashboardData.TotalProveedores}").FontSize(14).AlignCenter();
                                    table.Cell().Text($"{DashboardData.TotalUsuarios}").FontSize(14).AlignCenter();
                                });

                                // Cuarta Fila: Medicamentos totales y Estado del inventario
                                col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Text("Medicamentos Totales").Bold().FontSize(16).AlignCenter();
                                        header.Cell().Text(""); // Celda vacía
                                        header.Cell().Text("Estado del Inventario").Bold().FontSize(16).AlignCenter();
                                    });

                                    table.Cell().Text($"{DashboardData.MedicamentosTotales}").FontSize(14).AlignCenter();
                                    table.Cell().Text(""); // Celda vacía
                                    table.Cell().Text($"{DashboardData.EstadoInventario}").FontSize(14).AlignCenter();
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



                    using var memoryStream = new MemoryStream();
                    document.GeneratePdf(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception e)
            {
                MensajeError = $"Error: {e.Message}";
                return null;
            }
        }

        public byte[]? GenerarPdfConPdfSharp(DashboardData data, byte[] imagenGrafico = null, string nombreArchivo = "ReporteGeneral")
        {
            try
            {
                if (string.IsNullOrEmpty(data.EstadoInventario))
                    return null;

                using var document = new PdfDocument();
                var page = document.AddPage();
                page.Size = PdfSharpCore.PageSize.A4;
                page.Orientation = PdfSharpCore.PageOrientation.Portrait;

                var gfx = XGraphics.FromPdfPage(page);
                var fontTitle = new XFont("Arial", 20, XFontStyle.Bold);
                var fontSubTitle = new XFont("Arial", 14, XFontStyle.Bold);
                var fontText = new XFont("Arial", 12);

                double y = 40;

                // Título
                gfx.DrawString("Reporte General", fontTitle, XBrushes.Blue, new XRect(0, y, page.Width, 30), XStringFormats.TopCenter);
                y += 40;

                // Fecha inicial y final
                var fecha = DateTime.Now;
                gfx.DrawString($"Fecha Inicial: {new DateTime(fecha.Year, fecha.Month, 1):dd/MM/yyyy}", fontText, XBrushes.Black, new XPoint(40, y));
                gfx.DrawString($"Fecha Final: {new DateTime(fecha.Year, fecha.Month, DateTime.DaysInMonth(fecha.Year, fecha.Month)):dd/MM/yyyy}", fontText, XBrushes.Black, new XPoint(300, y));
                y += 30;

                // Totales (Facturas, Ventas, Medicamentos Vendidos)
                gfx.DrawString($"Total de Facturas: {data.TotalFacturasDelMes}", fontText, XBrushes.Black, new XPoint(40, y));
                y += 20;
                gfx.DrawString($"Total de Ventas: C$ {data.VentasDelMes:#,##0.00}", fontText, XBrushes.Black, new XPoint(40, y));
                y += 20;
                gfx.DrawString($"Total de Medicamentos Vendidos: {data.TotalMedicamentosVendidosDelMes}", fontText, XBrushes.Black, new XPoint(40, y));
                y += 30;

                // Medicamentos disponibles, escasos y más vendido
                gfx.DrawString($"Medicamentos Disponibles: {data.MedicamentosDisponibles}", fontText, XBrushes.Black, new XPoint(40, y));
                y += 20;
                gfx.DrawString($"Medicamentos Escasos: {data.MedicamentosEscasos}", fontText, XBrushes.Black, new XPoint(40, y));
                y += 20;
                gfx.DrawString($"Producto Más Vendido del Mes: {data.ProductoMasVendido}", fontText, XBrushes.Black, new XPoint(40, y));
                y += 30;

                // Categorías, proveedores, usuarios
                gfx.DrawString($"Total Categorías: {data.CategoriasTotales}", fontText, XBrushes.Black, new XPoint(40, y));
                y += 20;
                gfx.DrawString($"Total Proveedores: {data.TotalProveedores}", fontText, XBrushes.Black, new XPoint(40, y));
                y += 20;
                gfx.DrawString($"Total Usuarios: {data.TotalUsuarios}", fontText, XBrushes.Black, new XPoint(40, y));
                y += 30;

                // Totales e inventario
                gfx.DrawString($"Medicamentos Totales: {data.MedicamentosTotales}", fontText, XBrushes.Black, new XPoint(40, y));
                y += 20;
                gfx.DrawString($"Estado del Inventario: {data.EstadoInventario}", fontText, XBrushes.Black, new XPoint(40, y));
                y += 30;

                // Fecha de generación
                gfx.DrawString($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}", fontText, XBrushes.Gray, new XPoint(40, y));

                // Si hay imagen para el gráfico, la insertamos en una nueva página
                if (imagenGrafico != null)
                {
                    var imgPage = document.AddPage();
                    imgPage.Orientation = PdfSharpCore.PageOrientation.Landscape;
                    var gfxImg = XGraphics.FromPdfPage(imgPage);

                    using var ms = new MemoryStream(imagenGrafico);
                    var image = XImage.FromStream(() => ms);

                    double maxWidth = imgPage.Width - 60;
                    double maxHeight = imgPage.Height - 60;

                    double scaleX = maxWidth / image.PixelWidth;
                    double scaleY = maxHeight / image.PixelHeight;
                    double scale = Math.Min(scaleX, scaleY);

                    double imgWidth = image.PixelWidth * scale;
                    double imgHeight = image.PixelHeight * scale;

                    gfxImg.DrawImage(image, (imgPage.Width - imgWidth) / 2, (imgPage.Height - imgHeight) / 2, imgWidth, imgHeight);
                }

                using var outputStream = new MemoryStream();
                document.Save(outputStream);
                return outputStream.ToArray();
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al generar PDF: {ex.Message}";
                return null;
            }
        }

    }
}
