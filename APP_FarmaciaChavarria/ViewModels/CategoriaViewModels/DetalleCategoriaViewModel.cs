using API_FarmaciaChavarria.Models;
using APP_FarmaciaChavarria.Models.ModelsDTO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FarmaciaChavarria.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP_FarmaciaChavarria.ViewModels.CategoriaViewModels
{
    public partial class DetalleCategoriaViewModel: ObservableObject
    {

        private readonly CategoriaService _categoriaService;
        private readonly ProductoService _productoService;
        public DetalleCategoriaViewModel(CategoriaService categoriaService, ProductoService productoService)
        {
            _categoriaService = categoriaService;
            _productoService = productoService;
        }

        [ObservableProperty]
        private string nombreCategoria = string.Empty;

        [ObservableProperty]
        private int idCategoria;

        [ObservableProperty]
        private string mensajeError = string.Empty;

        [ObservableProperty]
        private string mensajeExito = string.Empty;

        [ObservableProperty]
        private bool isLoading = false;

        [ObservableProperty]
        private List<ProductoDTO> productos = new();

        // Información de paginación
        [ObservableProperty]
        private int numeroPagina = 1;

        [ObservableProperty]
        private int totalDeProductos;

        [ObservableProperty]
        private int totalDePaginas;

        [ObservableProperty]
        private int tamañoDePagina;

        // Filtro activado

        [ObservableProperty]
        private bool filtroBusqueda;

        [ObservableProperty]
        private string busquedaActual = "";

        public async Task CargarCategoriaPorId(int id)
        {
            var categoria = await _categoriaService.ObtenerCategoriaPorIdAsync(id);

            if (categoria != null)
            {
                NombreCategoria = categoria.nombre;
                IdCategoria = categoria.id_categoria;
            }
            else
            {
                MensajeError = "No se encontró la categoría";
            }
        }

        public async Task CrearCategoria()
        {
            try
            {
                // Comprobamos las validaciones antes de continuar con la acción de actualizar
                if (!Validacion()) return;


                var categoria = new Categoria
                {
                    nombre = NombreCategoria
                };

                var response = await _categoriaService.CrearCategoriaAsync(categoria);

                if (!response.Contains("Error"))
                {
                    MensajeExito = response;
                    LimpiarCampos();

                    // Función ejecutada en segundo plano para dejar el mensaje de éxito vacío luego de dos segundos
                    await LimpiarMensaje(true);

                }
                else
                {
                    MensajeError = response;

                    // Función ejecutada en segundo plano para dejar el mensaje de error vacío luego de dos segundos
                    await LimpiarMensaje(false);
                }
            }
            catch (Exception e)
            {
                MensajeError = e.Message;

                // Función ejecutada en segundo plano para dejar el mensaje de error vacío luego de dos segundos
                await LimpiarMensaje(false);
            }
        }

        public async Task ActualizarCategoria()
        {
            try
            {
                // Comprobamos las validaciones antes de continuar con la acción de actualizar
                if (!Validacion()) return;
          

                var categoria = new Categoria
                {
                    id_categoria = IdCategoria,
                    nombre = NombreCategoria
                };

                var response = await _categoriaService.ActualizarCategoriaAsync(IdCategoria, categoria);

                if (!response.Contains("Error"))
                {
                    MensajeExito = response;
                    LimpiarCampos();
                    // Función ejecutada en segundo plano para dejar el mensaje de éxito vacío luego de dos segundos
                    await LimpiarMensaje(true);
                    
                }
                else
                {
                    MensajeError = response;

                    // Función ejecutada en segundo plano para dejar el mensaje de error vacío luego de dos segundos
                    await LimpiarMensaje(false);
                }
            }
            catch (Exception e)
            {
                MensajeError = e.Message;

                // Función ejecutada en segundo plano para dejar el mensaje de error vacío luego de dos segundos
                await LimpiarMensaje(false);
            }
        }

        /*Fúnción para cargar las medicinas desde la base de datos
         y luego mostrarlas en la tabla correspondiente*/
        public async Task CargarMedicinasAsync()
        {
            try
            {
                MensajeError = string.Empty;

                var productos = await _productoService.ObtenerProductoPorCategoriaAsync(IdCategoria,NumeroPagina);

                if (productos is not null && productos.Productos.Any())
                {
                    Productos = productos.Productos;
                    NumeroPagina = productos.CurrentPage;
                    TotalDeProductos = productos.TotalItems;
                    TotalDePaginas = productos.TotalPages;
                    TamañoDePagina = productos.PageSize;
                }
                else
                {
                    MensajeError = "No se encontraron productos.";
                    await LimpiarMensaje(false);
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar productos: {ex.Message}";
                await LimpiarMensaje(false);
            }
        }

        public async Task BuscarMedicina(int pagina)
        {
            try
            {
                /*
                Si la búsqueda en un string vacio entonces cargamos todas las medicinas,
                reiniciamos el número de página a 1 y quitamos el filtro por categorías
                */
                if (BusquedaActual == "")
                {
                    NumeroPagina = 1;
                    await CargarMedicinasAsync();
                    FiltroBusqueda = false;
                    return;
                }

                MensajeError = string.Empty;

                var productos = await _productoService.ObtenerProductoPorCategoriaYNombreAsync(IdCategoria,BusquedaActual, pagina);
                if (productos is not null && productos.Productos.Any())
                {
                    FiltroBusqueda = true;
                    NumeroPagina = productos.CurrentPage;
                    TotalDeProductos = productos.TotalItems;
                    TotalDePaginas = productos.TotalPages;
                    TamañoDePagina = productos.PageSize;
                    Productos = productos.Productos;
                }
                else
                {
                    MensajeError = "No se encontró el producto.";
                    await LimpiarMensaje(false);
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar productos: {ex.Message}";
                await LimpiarMensaje(false);
            }
        }

        private bool Validacion()
        {
            if (NombreCategoria == "")
            {
                MensajeError = "El campo nombre de categoría no puede estar vacío";
                return false;
            }
            return true;
        }

        private void LimpiarCampos()
        {
            NombreCategoria = string.Empty;
            IdCategoria = 0;
        }

        // Si el valor es true se limpia el mensaje de éxito, de lo contrario, el mensaje de error
        private async Task LimpiarMensaje( bool limpiarExito)
        {
            if (limpiarExito)
            {
                _ = Task.Run(async () =>
                {
                    await Task.Delay(500);
                    MensajeExito = string.Empty;
                });

            }
            else
            {
                _ = Task.Run(async () =>
                {
                    await Task.Delay(500);
                    MensajeError = string.Empty;
                });
            }
            
        }

    }
}
