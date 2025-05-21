using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using API_FarmaciaChavarria.Models;
using APP_FarmaciaChavarria.Models.ModelsDTO;
using FarmaciaChavarria.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;

namespace FarmaciaChavarria.ViewModels
{
    public partial class CompraViewModel : ObservableObject
    {
        private readonly CompraService _compraService;
        private readonly ProductoService _productoService;
        private readonly ProveedorService _proveedorService;

        public CompraViewModel(CompraService compraService, ProductoService productoService, ProveedorService proveedorService)
        {
            _compraService = compraService;
            Compras = new ObservableCollection<Compra>();
            DetallesCompra = new ObservableCollection<DetalleCompra>();
            _productoService = productoService;
            _proveedorService = proveedorService;
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
        private string mensajeError = string.Empty;
        [ObservableProperty]
        private string mensajeExito = string.Empty;

        [ObservableProperty]
        private List<ProductoDTO> productos = new();

        [ObservableProperty]
        private List<Proveedor> proveedor = new();

        [ObservableProperty]
        private int numeroCompra;
        [ObservableProperty]
        private int numeroPagina = 1;

        [ObservableProperty]
        private string nombreProd = string.Empty;

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
        private int idproduct;

        [ObservableProperty]
        private int cant;

        [ObservableProperty]
        private decimal preciounit;


        // Información de paginación de Proveedores

        [ObservableProperty]
        private int numeroPaginaprov = 1;

        [ObservableProperty]
        private int totalDeproveedores;

        [ObservableProperty]
        private int totalDePaginasProv;

        [ObservableProperty]
        private int tamañoDePaginaProv;

        [ObservableProperty]
        private bool filtroBusquedaProv;

        [ObservableProperty]
        private string busquedaProvedor;


        [ObservableProperty]
        private string nombreProv;

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
                MensajeError = $"Error al obtener compras: {ex.Message}";
            }
        }
        [RelayCommand]
        public async Task ObtenerIdComprasAsync()
        {
            try
            {
                var compras = await _compraService.ObtenerComprasAsync();

                if (compras != null && compras.Any())
                {
                    var ultimoId = compras.Max(c => c.id_compra);
                    NumeroCompra = ultimoId + 1;
                }
                else
                {
                    NumeroCompra = 1;
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al obtener facturas: {ex.Message}");
                NumeroCompra = 1;
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
                MensajeError = $"Error al crear compra: {ex.Message}";
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
                MensajeError = $"Error al actualizar compra: {ex.Message}";
            }
        }


        #region Proveedores Funcion

        public async Task CargarProveedor()
        {
            try
            {
                var proveedor = await _proveedorService.ObtenerProveedoresAsync(NumeroPagina);
                if (proveedor is not null && proveedor.Proveedores.Any())
                {
                    Proveedor = proveedor.Proveedores;
                    NumeroPaginaprov = proveedor.CurrentPage;
                    TotalDePaginasProv = proveedor.TotalPages;
                    TotalDeproveedores = proveedor.TotalItems;
                    TamañoDePaginaProv = proveedor.PageSize;
                }
                else
                {
                    MensajeError = "No se encontraron Proveedor.";
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar Proveedor: {ex.Message}";
            }
        }

        public async Task BuscarProveedores(int pagina)
        {
            try
            {
                MensajeError = string.Empty;

                if (BusquedaProvedor == "")
                {
                    NumeroPagina = 1;
                    await CargarProveedor();
                    Filtrobusqueda = false;
                    return;
                }

                var proveedor = await _proveedorService.BuscarProveedoresPorNombreAsync(BusquedaProvedor, pagina);
                if (proveedor is not null && proveedor.Proveedores.Any())
                {
                    Proveedor = proveedor.Proveedores;
                    NumeroPagina = proveedor.CurrentPage;
                    TotalProductos = proveedor.TotalPages;
                    TotalDePaginas = proveedor.TotalPages;
                    TamañoDePagina = proveedor.PageSize;
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


        #endregion

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



        #region Funcion Productos


        [RelayCommand]
        public void AgregarProductoAlDetalle()
        {
            if (Idproduct <= 0 || Cant <= 0)
            {
                MensajeError = "Seleccione un producto válido y cantidad.";
                return;
            }

            var detalle = new DetalleCompra
            {
                id_producto = Idproduct,
                cantidad = Cant,
                precio_unitario = Preciounit
            };

            DetallesCompra.Add(detalle);


            Idproduct = 0;
            Preciounit = 0;
            Cant = 0;
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

        #endregion

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
                MensajeError = $"Error al eliminar compra: {ex.Message}";
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
            MensajeError = "";
        }
    }
}
