using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using API_FarmaciaChavarria.Models;
using APP_FarmaciaChavarria.Models.ModelsDTO;
using FarmaciaChavarria.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace FarmaciaChavarria.ViewModels
{
    public partial class FacturacionViewModel : ObservableObject
    {
        private readonly ProductoService _productoService;
        private readonly FacturaService _facturaService;

        public FacturacionViewModel(ProductoService productoService, FacturaService facturaService)
        {
            _productoService = productoService;
            _facturaService = facturaService;

            DetallesFactura = new ObservableCollection<DetalleFacturaDTO>();
            NuevaFactura = new Factura { fecha_venta = DateTime.Now };
        }

        [ObservableProperty]
        private List<ProductoDTO> productosEncontrados = new();

        [ObservableProperty]
        private ObservableCollection<DetalleFacturaDTO> detallesFactura;

        [ObservableProperty]
        private Factura nuevaFactura;

        [ObservableProperty]
        private string textoBusqueda = string.Empty;

        [ObservableProperty]
        private string mensaje = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private string nombreProd = string.Empty;

        [ObservableProperty]
        private decimal precioProd;

        [ObservableProperty]
        private int idproducto;

        [ObservableProperty]
        private int stock;

        [ObservableProperty]
        private string lab;

        [ObservableProperty]
        private List<ProductoDTO> productos = new();

        [ObservableProperty]
        private string mensajeError;

        [ObservableProperty]
        private int numeroPagina = 1;

        [ObservableProperty]
        private int totalProductos;

        [ObservableProperty]
        private int totalDePaginas;

        [ObservableProperty]
        private int tamañoDePagina;

        [ObservableProperty]
        private bool filtroBusqueda;

        [ObservableProperty]
        private string busquedaProductos = "";

        [ObservableProperty]
        private bool filtrobusqueda;

        [ObservableProperty]
        private string productoselect;

        [ObservableProperty]
        private int cant;

        [ObservableProperty]
        private int numeroFactura;

        [ObservableProperty]
        private DateTime fechaVenta = DateTime.Now;

        [ObservableProperty]
        private int idUsuario = 3;

        [ObservableProperty]
        private decimal total;

        [ObservableProperty]
        private string mensajeExito;


        [RelayCommand]
        public async Task ObtenerIdFactAsync()
        {
            try
            {
                var facturas = await _facturaService.ObtenerFacturasAsync();

                if (facturas != null && facturas.Any())
                {
                    var ultimoId = facturas.Max(f => f.id_factura); 
                    NumeroFactura = ultimoId + 1; 
                }
                else
                {
                    NumeroFactura = 1; 
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al obtener facturas: {ex.Message}");
                NumeroFactura = 1;
            }
        }

        [RelayCommand]
        public async Task GuardarFacturaCommandAsync()
        {
            var factura = new Factura
            {
                fecha_venta = DateTime.Now,
                total = DetallesFactura.Sum(d => d.subtotal),
                id_usuario = 3,
            };

            var idFactura = await _facturaService.GuardarFacturaAsync(factura);

            bool todosExitosos = true;

            foreach (var detalle in DetallesFactura)
            {

                var detalleACrear = new DetalleFactura
                {
                    id_factura = idFactura.Value,
                    cantidad = detalle.cantidad,
                    id_producto = detalle.id_producto,
                    precio_unitario = detalle.precio_unitario
                };

                Console.WriteLine($"ID de factura asignado al detalle: {detalleACrear.id_factura}");
                var resultado = await _facturaService.GuardarDetalleFacturaAsync(detalleACrear);
                if (!resultado)
                {
                    todosExitosos = false;
                    break;
                }
            }

            if (todosExitosos)
            {
                MensajeExito = "Factura y todos los detalles guardados exitosamente.";
                LimpiarFormulario();
                await ObtenerIdFactAsync();            
            }
            else
            {
                MensajeError = "Factura guardada, pero uno o más detalles fallaron.";
            }


        }

        public async Task CargarProductos()
        {
            try
            {
                var productos = await _productoService.ObtenerProductosAsync(NumeroPagina);
                if (productos is not null && productos.Productos.Any())
                {
                    Productos = productos.Productos;
                    NumeroPagina = productos.CurrentPage;
                    TotalDePaginas = productos.TotalPages;
                    TotalProductos = productos.TotalItems;
                    TamañoDePagina = productos.PageSize;
                }
                else
                {
                    MensajeError = "No se encontraron productos.";
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar productos: {ex.Message}";
            }
        }

        public async Task BuscarProductos(int pagina)
        {
            try
            {
                MensajeError = string.Empty;

                if (BusquedaProductos == "")
                {
                    NumeroPagina = 1;
                    await CargarProductos();
                    Filtrobusqueda = false;
                    return;
                }

                var productos = await _productoService.ObtenerProductoPorNombreAsync(BusquedaProductos, pagina);
                if (productos is not null && productos.Productos.Any())
                {
                    Productos = productos.Productos;
                    NumeroPagina = productos.CurrentPage;
                    TotalProductos = productos.TotalItems;
                    TotalDePaginas = productos.TotalPages;
                    TamañoDePagina = productos.PageSize;
                    FiltroBusqueda = true;
                }
                else
                {
                    MensajeError = "No se encontraron productos.";
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al buscar productos: {ex.Message}";
            }
        }

        [RelayCommand]
        public void LimpiarFormulario()
        {
            NuevaFactura = new Factura { fecha_venta = DateTime.Now };
            DetallesFactura.Clear();
            ProductosEncontrados.Clear();
            TextoBusqueda = "";
            Mensaje = "";
            ErrorMessage = "";
            Idproducto = 0;
            NombreProd = "";
            PrecioProd = 0;
            Cant = 0;
            NumeroFactura = 0;
        }

        [RelayCommand]
        public void AgregarProductoAlDetalle()
        {
            if (Idproducto <= 0 || Cant <= 0)
            {
                MensajeError = "Seleccione un producto válido y cantidad.";
                return;
            }

            foreach(var producto in DetallesFactura)
            {
                if(producto.id_producto == Idproducto)
                {
                    MensajeError = "El producto ya se encuentra en el detalle.";
                    return;
                }
            }

            if (Cant > Stock)
            {
                MensajeError = "La cantidad ingresada es superior al stock del producto.";
                return;
            }

            var detalle = new DetalleFacturaDTO
            {
                id_producto = Idproducto,
                cantidad = Cant,
                precio_unitario = PrecioProd,
                nombreProducto = NombreProd
            };

            DetallesFactura.Add(detalle);


            Idproducto = 0;
            NombreProd = "";
            PrecioProd = 0;
            Cant = 0;
        }
    }
}
