using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using API_FarmaciaChavarria.Models;
using FarmaciaChavarria.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;

namespace FarmaciaChavarria.ViewModels
{
    public partial class CompraViewModel : ObservableObject
    {
        private readonly CompraService _compraService;

        public CompraViewModel(CompraService compraService)
        {
            _compraService = compraService;
            Compras = new ObservableCollection<Compra>();
            DetallesCompra = new ObservableCollection<DetalleCompra>();
        }

        [ObservableProperty]
        private ObservableCollection<Compra> compras;

        [ObservableProperty]
        private ObservableCollection<DetalleCompra> detallesCompra;

        [ObservableProperty]
        private Compra compraSeleccionada = new();

        [ObservableProperty]
        private string mensaje = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [RelayCommand]
        public async Task ObtenerCompras()
        {
            try
            {
                var lista = await _compraService.ObtenerComprasAsync();
                Compras = new ObservableCollection<Compra>(lista!);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al obtener compras: {ex.Message}";
            }
        }

        [RelayCommand]
        public async Task CrearCompra()
        {
            try
            {
                if (CompraSeleccionada == null || DetallesCompra.Count == 0)
                {
                    Mensaje = "Debe llenar los campos de la compra y agregar al menos un detalle.";
                    return;
                }

                var resultado = await _compraService.CrearCompraAsync(CompraSeleccionada, new List<DetalleCompra>(DetallesCompra));
                Mensaje = resultado;
                await ObtenerCompras();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al crear compra: {ex.Message}";
            }
        }

        [RelayCommand]
        public async Task ActualizarCompra()
        {
            try
            {
                if (CompraSeleccionada == null || CompraSeleccionada.id_compra == 0)
                {
                    Mensaje = "Debe seleccionar una compra válida para actualizar.";
                    return;
                }

                var resultado = await _compraService.ActualizarCompraAsync(CompraSeleccionada.id_compra, CompraSeleccionada);
                Mensaje = resultado;
                await ObtenerCompras();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al actualizar compra: {ex.Message}";
            }
        }

        [RelayCommand]
        public async Task EliminarCompra()
        {
            try
            {
                if (CompraSeleccionada == null || CompraSeleccionada.id_compra == 0)
                {
                    Mensaje = "Debe seleccionar una compra válida para eliminar.";
                    return;
                }

                var resultado = await _compraService.EliminarCompraAsync(CompraSeleccionada.id_compra);
                Mensaje = resultado;
                await ObtenerCompras();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al eliminar compra: {ex.Message}";
            }
        }

        [RelayCommand]
        public void LimpiarFormulario()
        {
            CompraSeleccionada = new Compra
            {
                fecha_compra = DateTime.Now,
                total = 0
            };
            DetallesCompra.Clear();
            Mensaje = "";
            ErrorMessage = "";
        }
    }
}
