using API_FarmaciaChavarria.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using FarmaciaChavarria.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP_FarmaciaChavarria.ViewModels.CategoriaViewModels
{
    public partial class CategoriaViewModel: ObservableObject
    {
        private readonly CategoriaService _categoriaService;
        public CategoriaViewModel(CategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [ObservableProperty]
        private string mensajeError = string.Empty;

        [ObservableProperty]
        private string mensajeExito = string.Empty;

        [ObservableProperty]
        private bool isLoading = false;

        [ObservableProperty]
        private List<Categoria> categorias = new();

        [ObservableProperty]
        private string busquedaCategoria;

        // Información de paginación de categorias
        [ObservableProperty]
        private int numeroPagina = 1;

        [ObservableProperty]
        private int totalDeCategorias;

        [ObservableProperty]
        private int totalDePaginas;

        [ObservableProperty]
        private int tamañoDePagina;

        [ObservableProperty]
        private bool filtroBusqueda;

        public async Task CargarCategorias()
        {
            try
            {
                var categorias = await _categoriaService.ObtenerCategoriasAsync(NumeroPagina);
                if (categorias is not null && categorias.Categorias.Any())
                {
                    Categorias = categorias.Categorias;
                    NumeroPagina = categorias.CurrentPage;
                    TotalDeCategorias = categorias.TotalItems;
                    TotalDePaginas = categorias.TotalPages;
                    TamañoDePagina = categorias.PageSize;
                }
                else
                {
                    MensajeError = "No se encontraron categorias.";
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar categorias: {ex.Message}";
            }
        }

        public async Task BuscarCategoria(int pagina)
        {
            try
            {
                MensajeError = string.Empty;

                if (BusquedaCategoria == "")
                {
                    NumeroPagina = 1;
                    await CargarCategorias();
                    FiltroBusqueda = false;
                    return;
                }

                /*Si la última búsqueda es diferente a la nueva entonces se realiza
                 y se reinicia el número de página a uno*/

                var categorias = await _categoriaService.ObtenerCategoriaPorNombreAsync(BusquedaCategoria, pagina, 2);

                if (categorias is not null && categorias.Categorias.Any())
                {
                    Categorias = categorias.Categorias;
                    NumeroPagina = categorias.CurrentPage;
                    TotalDeCategorias = categorias.TotalItems;
                    TotalDePaginas = categorias.TotalPages;
                    TamañoDePagina = categorias.PageSize;
                    FiltroBusqueda = true;
                }
                else
                {
                    MensajeError = "No se encontraron categorias.";
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar categorias: {ex.Message}";
            }
        }
    }
}
