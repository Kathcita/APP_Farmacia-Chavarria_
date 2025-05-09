using APP_FarmaciaChavarria.Models.ModelsDTO;
using APP_FarmaciaChavarria.Models.PaginationModels;
using CommunityToolkit.Mvvm.ComponentModel;
using DocumentFormat.OpenXml.Spreadsheet;
using FarmaciaChavarria.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP_FarmaciaChavarria.ViewModels.Reportes
{
    public partial class ReporteMedicamentosEscasosViewModel : ObservableObject
    {
        private ProductoService _productoService;

        public ReporteMedicamentosEscasosViewModel(ProductoService productoService)
        {
            _productoService = productoService;

            NumeroPagina = 1;
        }

        [ObservableProperty]
        private List<ProductoDTO> productosEscasos = new();

        [ObservableProperty]
        private string mensajeError = "";

        [ObservableProperty]
        private string mensajeExito = "";

        // Paginación

        [ObservableProperty]
        private int numeroPagina;

        [ObservableProperty]
        private int totalDePaginas;

        public async Task CargarProductos()
        { 
            try {

                var response = await _productoService.ObtenerProductosEscasosAsync(NumeroPagina);

                if (response != null && response.Productos.Any())
                {
                    ProductosEscasos = response.Productos;
                    TotalDePaginas = response.TotalPages;
                    MensajeError = "";
                }
                else
                {
                    MensajeError = "No se encontraron datos";
                }
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

    }
}
