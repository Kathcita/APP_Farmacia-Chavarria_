using API_FarmaciaChavarria.Models;
using APP_FarmaciaChavarria.Models.ModelsDTO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FarmaciaChavarria.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace APP_FarmaciaChavarria.ViewModels
{
    public partial class InventarioViewModel : ObservableObject
    {
        private readonly ProductoService _productoService;
        private readonly CategoriaService _categoriaService;

        public InventarioViewModel(ProductoService productoService, CategoriaService categoriaService)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
        }

        [ObservableProperty]
        private List<ProductoDTO> medicinas = new();

        [ObservableProperty]
        private List<Categoria> categorias = new();

        [ObservableProperty]
        private bool cargando;

        [ObservableProperty]
        private string mensajeError = string.Empty;


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
        private int idCategoria;

        [ObservableProperty]
        private bool filtroCategoria;

        [ObservableProperty]
        private bool filtroBusqueda;

        [ObservableProperty]
        private string busquedaActual;


        /*Fúnción para cargar las medicinas desde la base de datos
         y luego mostrarlas en la tabla correspondiente*/
        [RelayCommand]
        public async Task CargarMedicinasAsync()
        {
            try
            {
                Cargando = true;
                MensajeError = string.Empty;

                var productos = await _productoService.ObtenerProductosAsync(NumeroPagina);

                if (productos is not null && productos.Productos.Any())
                {
                    NumeroPagina = productos.CurrentPage;
                    TotalDeProductos = productos.TotalItems;
                    TotalDePaginas = productos.TotalPages;
                    TamañoDePagina = productos.PageSize;
                    Medicinas = productos.Productos;
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
            finally
            {
                Cargando = false;
            }
        }

        [RelayCommand]
        public async Task BuscarMedicina(string nombre)
        {
            try
            { 
                /*
                Si la búsqueda en un string vacio entonces cargamos todas las medicinas,
                reiniciamos el número de página a 1 y quitamos el filtro por categorías
                */
                if (nombre == "")
                {
                    NumeroPagina = 1;
                    await CargarMedicinasAsync();
                    FiltroCategoria = false;
                    FiltroBusqueda = false;
                    return;
                }

                if (BusquedaActual != nombre || FiltroCategoria)
                {
                    NumeroPagina = 1;
                }

                Cargando = true;
                MensajeError = string.Empty;

                var productos = await _productoService.ObtenerProductoPorNombreAsync(nombre, NumeroPagina);
                if (productos is not null && productos.Productos.Any())
                {
                    BusquedaActual = nombre;
                    FiltroBusqueda = true;
                    FiltroCategoria = false;
                    NumeroPagina = productos.CurrentPage;
                    TotalDeProductos = productos.TotalItems;
                    TotalDePaginas = productos.TotalPages;
                    TamañoDePagina = productos.PageSize;
                    Medicinas = productos.Productos;
                }
                else
                {
                    MensajeError = "No se encontró el producto.";
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar productos: {ex.Message}";
            }
            finally
            {
                Cargando = false;
            }
        }

        /*Función para cargar las categorías desde la base de datos
         y luego cargarlas en el select para realizar filtrados de datos*/
        [RelayCommand]
        public async Task CargarCategorias()
        {
            try
            {
                var categorias = await _categoriaService.ObtenerCategoriasAsync();
                if(categorias is not null && categorias.Categorias.Any())
                {
                    Categorias = categorias.Categorias;
                }
                else
                {
                    MensajeError = "No se encontraron categorias.";
                }
            }
            catch(Exception ex)
            {
                MensajeError = $"Error al cargar categorias: {ex.Message}";
            }
        }

        [RelayCommand]
        public async Task FiltrarProductosPorCategoria(int id)
        {
            try
            {

                /* Si el id proporcionado es 0, es decir la primera opción
                 entonces se eliminará el filtro por categoría y cargarán
                las medicinas
                 */

                if(id == 0)
                {
                    await CargarMedicinasAsync();
                    FiltroCategoria = false;
                    return;
                }

                /* Si la categoría a filtrar es diferente a la que estaba,
                  entonces el número de página a mostrar será la 1*/

                if (id != IdCategoria)
                {
                    NumeroPagina = 1;
                }

                var productos = await _productoService.ObtenerProductoPorCategoriaAsync(id, NumeroPagina);
                if (productos is not null && productos.Productos.Any())
                {
                    IdCategoria = id;
                    FiltroBusqueda = false;
                    FiltroCategoria = true;
                    NumeroPagina = productos.CurrentPage;
                    TotalDeProductos = productos.TotalItems;
                    TotalDePaginas = productos.TotalPages;
                    TamañoDePagina = productos.PageSize;
                    Medicinas = productos.Productos;
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
    }
}
