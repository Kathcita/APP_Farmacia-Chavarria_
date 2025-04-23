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
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;


namespace APP_FarmaciaChavarria.ViewModels.Reportes
{
    public partial class ReporteVentasViewModel: ObservableObject
    {
        private FacturaService _facturaService;
        private UsuarioService _usuarioService;
        public ReporteVentasViewModel(FacturaService facturaService, UsuarioService usuarioService)
        {
            _facturaService = facturaService;
            _usuarioService = usuarioService;

            // Inicializar fechas con datos por defecto, el rango de fecha es desde el inicio del año hasta la fecha actual
            PrimeraFecha = new DateTime(DateTime.Now.Year, 1, 1);
            UltimaFecha = DateTime.Now.Date;
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


        /*Función para cargar los datos de datos de ventas y mostrarlos en la tabla*/
        public async Task CargarVentas()
        {
            try
            {
                if (PrimeraFecha > UltimaFecha)
                {
                    return;
                }

                var fechaInicioString = PrimeraFecha.ToString("yyyy-MM-dd");
                var fechaFinString = UltimaFecha.ToString("yyyy-MM-dd");

                var response = await _facturaService.ObtenerFacturasPorAñoAsync(fechaInicioString, fechaFinString, UserId);

                if (response.Any())
                {
                    Facturas = response;
                }
                else
                {

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

        public void GenerarExcel(List<RevenueDataItem> data, string nombreArchivo = "ReporteDeVentas")
        {

            if (data.Count == 0)
            {
                MensajeError = "No hay datos para crear un reporte";
                return;
            }

            // Generar nombre único para evitar sobreescribir archivos
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"{nombreArchivo}_{timestamp}.xlsx";

            // Ruta de guardado según plataforma
            string filePath;
            #if ANDROID
            filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
            #elif WINDOWS
            filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
            #else
            filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
            #endif

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

                // Fila de fecha
                worksheet.Cell(row, 1).Style.Font.FontSize = 16;
                worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Fila de Total
                worksheet.Cell(row, 2).Style.Font.FontSize = 16;
                worksheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 2).Style.NumberFormat.Format = "C$ #,##0.00";
            }

            if(UserId != 0)
            {
                worksheet.Cell(1, 3).Value = "Usuario";
                var usuario = Usuarios.Find(u => u.id_usuario == UserId);
                worksheet.Cell(2, 3).Value = usuario?.nombre;
                worksheet.Cell(2, 3).Style.Font.FontSize = 16;
            }
            

            // Opcional: Formato bonito
            worksheet.Columns().AdjustToContents(); // Autoajustar columnas

            // Guardar archivo
            workbook.SaveAs(filePath);

            MensajeExito = "El archivo de excel se guardó exitosamente";
        }




    }
}
