using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using API_FarmaciaChavarria.Models;
using FarmaciaChavarria.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;

namespace FarmaciaChavarria.ViewModels
{
    public partial class FacturaViewModel : ObservableObject
    {
        private readonly FacturaService _facturaService;

        public FacturaViewModel(FacturaService facturaService)
        {
            _facturaService = facturaService;
            Facturas = new ObservableCollection<Factura>();
            DetallesFactura = new ObservableCollection<DetalleFactura>();
            NuevaFactura = new Factura { fecha_venta = DateTime.Now };
            NuevoDetalle = new DetalleFactura();
        }

        [ObservableProperty]
        private ObservableCollection<Factura> facturas;

        [ObservableProperty]
        private ObservableCollection<DetalleFactura> detallesFactura;

        [ObservableProperty]
        private Factura nuevaFactura;

        [ObservableProperty]
        private DetalleFactura nuevoDetalle;

        [ObservableProperty]
        private string mensaje = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [RelayCommand]
        public async Task ObtenerFacturas()
        {
            try
            {
                var lista = await _facturaService.ObtenerFacturasAsync();
                Facturas = new ObservableCollection<Factura>(lista!);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al obtener facturas: {ex.Message}";
            }
        }

        [RelayCommand]
        public void AgregarDetalle()
        {
            if (NuevoDetalle.id_producto == 0 || NuevoDetalle.cantidad <= 0 || NuevoDetalle.precio_unitario <= 0)
            {
                Mensaje = "Todos los campos del detalle deben estar llenos.";
                return;
            }

            DetallesFactura.Add(new DetalleFactura
            {
                id_producto = NuevoDetalle.id_producto,
                cantidad = NuevoDetalle.cantidad,
                precio_unitario = NuevoDetalle.precio_unitario
            });

            CalcularTotal();
            NuevoDetalle = new DetalleFactura(); 
        }

        [RelayCommand]
        public void EliminarDetalle(DetalleFactura detalle)
        {
            DetallesFactura.Remove(detalle);
            CalcularTotal();
        }

        private void CalcularTotal()
        {
            decimal total = 0;
            foreach (var item in DetallesFactura)
            {
                total += item.subtotal;
            }
            NuevaFactura.total = total;
        }

        [RelayCommand]
        public async Task CrearFactura()
        {
            try
            {
                if (DetallesFactura.Count == 0)
                {
                    Mensaje = "Debe agregar al menos un detalle.";
                    return;
                }

                NuevaFactura.fecha_venta = DateTime.Now;
                CalcularTotal();

                var resultado = await _facturaService.CrearFacturaAsync(NuevaFactura, new List<DetalleFactura>(DetallesFactura));
                Mensaje = resultado;

                await ObtenerFacturas();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al crear factura: {ex.Message}";
            }
        }

        [RelayCommand]
        public void LimpiarFormulario()
        {
            NuevaFactura = new Factura { fecha_venta = DateTime.Now };
            DetallesFactura.Clear();
            NuevoDetalle = new DetalleFactura();
            Mensaje = "";
            ErrorMessage = "";
        }
    }
}
