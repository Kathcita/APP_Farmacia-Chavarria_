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
            DetallesCompra = new ObservableCollection<DetalleCompraDTO>();
            _productoService = productoService;
            _proveedorService = proveedorService;
        }

        [ObservableProperty]
        private ObservableCollection<Compra> compras;

        [ObservableProperty]
        private ObservableCollection<DetalleCompraDTO> detallesCompra;

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
        private int stock;

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

        [ObservableProperty]
        private int idProv;

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
                if (DetallesCompra.Count == 0)
                {
                    Mensaje = "Debe llenar los campos de la compra y agregar al menos un detalle.";
                    return;
                }

                var compra = new Compra
                {
                    id_proveedor = IdProv,
                    fecha_compra = DateTime.Now,
                    total = DetallesCompra.Sum(d => d.subtotal),
                };

                var resultado = await _compraService.CrearCompraAsync(compra);

                if (resultado != null)
                {
                    foreach (var detalle in DetallesCompra)
                    {
                        var detalleCrear = new DetalleCompra
                        {
                            id_compra = resultado.Value,
                            id_producto = detalle.id_producto,
                            cantidad = detalle.cantidad,
                            precio_unitario = detalle.precio_unitario,
                        };

                        var resultadoDetalle = await _compraService.GuardarDetalleCompraAsync(detalleCrear);

                        if (!resultadoDetalle)
                        {
                        }
                    }
                    MensajeExito = "Compra creada exitosamente";
                    LimpiarFormulario();
                    await ObtenerCompras();
                }
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
                var proveedor = await _proveedorService.ObtenerProveedoresAsync(NumeroPaginaprov);
                if (proveedor is not null && proveedor.Proveedores.Any())
                {
                    Proveedor = proveedor.Proveedores;
                    NumeroPaginaprov = proveedor.CurrentPage;
                    TotalDePaginasProv = proveedor.TotalPages;
                    TotalDeproveedores = proveedor.TotalItems;
                    TamañoDePaginaProv = proveedor.PageSize;
                    FiltroBusquedaProv = false;
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
                    NumeroPaginaprov = 1;
                    await CargarProveedor();
                    FiltroBusquedaProv = false;
                    return;
                }

                var proveedor = await _proveedorService.BuscarProveedoresPorNombreAsync(BusquedaProvedor, pagina);
                if (proveedor is not null && proveedor.Proveedores.Any())
                {
                    Proveedor = proveedor.Proveedores;
                    NumeroPaginaprov = proveedor.CurrentPage;
                    TotalDeproveedores = proveedor.TotalPages;
                    TotalDePaginasProv = proveedor.TotalPages;
                    TamañoDePaginaProv = proveedor.PageSize;
                    FiltroBusquedaProv = true;
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
            if(NombreProv == "")
            {
                MensajeError = "Seleccione un proveedor.";
                return;
            }

            if (Idproduct <= 0 || Cant <= 0)
            {
                MensajeError = "Seleccione un producto válido y cantidad.";
                return;
            }

            foreach(var producto in DetallesCompra)
            {
                if (producto.id_producto == Idproduct)
                {
                    MensajeError = "El producto ya se encuentra en el detalle.";
                    return;
                }
            }

            var detalle = new DetalleCompraDTO
            {
                id_producto = Idproduct,
                cantidad = Cant,
                precio_unitario = Preciounit,
                nombreProducto = NombreProd
            };

            DetallesCompra.Add(detalle);

            Idproduct = 0;
            Preciounit = 0;
            NombreProd = "";
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
        public async Task LimpiarFormulario()
        {
            CompraSeleccionada = new Compra
            {
                fecha_compra = DateTime.Now,
                total = 0
            };
            DetallesCompra.Clear();
            Mensaje = "";
            MensajeError = "";
            await ObtenerIdComprasAsync();
            NombreProv = "";
            IdProv = 0;
        }
    }
}
