using APP_FarmaciaChavarria.Models.ModelsDTO;
using CommunityToolkit.Mvvm.ComponentModel;
using FarmaciaChavarria.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP_FarmaciaChavarria.ViewModels.Reportes
{
    public partial class MedicamentosPorCaducarViewModel: ObservableObject
    {
        private readonly ProductoService _productoService;

        public MedicamentosPorCaducarViewModel(ProductoService productoService)
        {
            _productoService = productoService;

            NumeroPagina = 1;
        }

        [ObservableProperty]
        private List<ProductoDTO> productosPorCaducar = new();

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
            try
            {

                var response = await _productoService.ObtenerProductoPorCaducar(NumeroPagina);

                if (response != null && response.Productos.Any())
                {
                    ProductosPorCaducar = response.Productos;
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
