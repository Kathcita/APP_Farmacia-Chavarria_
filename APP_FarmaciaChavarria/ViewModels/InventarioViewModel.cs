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
        private bool isLoading;

        [ObservableProperty]
        private string mensajeError = string.Empty;



        [RelayCommand]
        public async Task CargarMedicinasAsync()
        {
            try
            {
                IsLoading = true;
                MensajeError = string.Empty;

                var productos = await _productoService.ObtenerProductosAsync();

                if (productos is not null && productos.Any())
                {
                    Medicinas = productos;
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
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task BuscarMedicina(string nombre)
        {
            if(nombre == "")
            {
                await CargarMedicinasAsync();
                Debug.WriteLine($"cargando...");
                return;
            }

            try
            {
                IsLoading = true;
                MensajeError = string.Empty;

                var productos = await _productoService.ObtenerProductoPorNombreAsync(nombre);
                if (productos is not null && productos.Any())
                {
                    Medicinas = productos;
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
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task CargarCategorias()
        {
            try
            {
                var categorias = await _categoriaService.ObtenerCategoriasAsync();
                if(categorias is not null && categorias.Any())
                {
                    Categorias = categorias;
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
                var productos = await _productoService.ObtenerProductoPorCategoriaAsync(id);
                if (productos is not null && productos.Any())
                {
                    Medicinas = productos;
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
