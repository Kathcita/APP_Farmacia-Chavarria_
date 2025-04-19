using API_FarmaciaChavarria.Models;
using APP_FarmaciaChavarria.Models.ReporteModels;
using CommunityToolkit.Mvvm.ComponentModel;
using FarmaciaChavarria.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        /*Función para cargar los datos de datos de ventas y mostrarlos en la tabla*/
        public async Task CargarVentas()
        {
            try
            {
                var response = await _facturaService.ObtenerFacturasPorAñoAsync(Año, UserId);

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

            }
        }

        /*Función para cargar los datos de ventas y mostrarlos en el gráfico*/
        public async Task CargarVentasGrafico()
        {
            try
            {
                if (!(Año is int))
                {
                    return;
                }

                var response = await _facturaService.ObtenerFacturasReporteAsync(Año, UserId);

                if (response.Any())
                {
                    RevenueData = response;
                }
                else
                {

                }
            }
            catch (Exception ex)
            {

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
                else
                {

                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
