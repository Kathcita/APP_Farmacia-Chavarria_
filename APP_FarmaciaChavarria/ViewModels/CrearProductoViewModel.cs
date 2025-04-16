using API_FarmaciaChavarria.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using FarmaciaChavarria.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP_FarmaciaChavarria.ViewModels
{
    public partial class CrearProductoViewModel : ObservableObject
    {

        private readonly ProductoService _productoService;
        private readonly CategoriaService _categoriaService;
        private readonly LaboratorioService _laboratorioService;

        public CrearProductoViewModel(
            ProductoService productoService,
            CategoriaService categoriaService,
            LaboratorioService laboratorioService)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
            _laboratorioService = laboratorioService;
        }

        [ObservableProperty]
        private List<Categoria> categorias = new();

        [ObservableProperty]
        private List<Laboratorio> laboratorios = new();

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string mensajeError = string.Empty;

        [ObservableProperty]
        private string mensajeExito = string.Empty;

        [ObservableProperty]
        private string busquedaCategoria = "";

        [ObservableProperty]
        private string busquedaLaboratorio = "";

        // Propiedades del producto

        [ObservableProperty]
        private string nombre = "";

        [ObservableProperty]
        private int idProducto;

        [ObservableProperty]
        private int categoriaId = 0;

        [ObservableProperty]
        private string nombreCategoria;

        [ObservableProperty]
        private int laboratorioId = 0;
        
        [ObservableProperty]
        private string nombreLaboratorio;

        [ObservableProperty]
        private string stockInput = "0";

        [ObservableProperty]
        private int stock;

        [ObservableProperty]
        private decimal precio = 0;

        [ObservableProperty]
        private DateOnly fechaVencimiento = DateOnly.FromDateTime(DateTime.Today);

        [ObservableProperty]
        private string comoUsar;

        [ObservableProperty]
        private string efectosSecundarios;

        public async Task CrearProducto()
        {
            try
            {
                if (!Validaciones()){
                    return;
                }

                var nuevoProducto = new Producto
                {
                    nombre = Nombre,
                    id_categoria = CategoriaId,
                    id_laboratorio = LaboratorioId,
                    fecha_vencimiento = FechaVencimiento,
                    stock = Stock,
                    precio = Precio
                };

                var response = await _productoService.CrearProductoAsync(nuevoProducto);

                if (!response.Contains("Error"))
                {
                    MensajeError = "";
                    MensajeExito = "Producto registrado exitosamente";
                    LimpiarCampos();
                }
                else
                {
                    MensajeError = response;
                }
            }
            catch (Exception ex)
            {
                MensajeError = $":{ex.Message}";
            }
        }

        private Boolean Validaciones()
        {
            if (Nombre == "")
            {
                MensajeError = "El campo nombre no puede estar vacío";
                return false;
            }
            if (CategoriaId == 0)
            {
                MensajeError = "Seleccione una categoría para el producto";
                return false;
            }
            if (LaboratorioId == 0)
            {
                MensajeError = "Seleccione un laboratorio para el producto";
                return false;
            }
            if (FechaVencimiento < DateOnly.FromDateTime(DateTime.Today))
            {
                MensajeError = "Ingrese una fecha de vencimiento válida";
                return false;
            }
            if (!decimal.TryParse(StockInput, out var parsedValue) || parsedValue % 1 != 0 || parsedValue < 0)
            {
                MensajeError = "Ingrese una cantidad válida (entero positivo)";
                return false;
            }

            Stock = (int)parsedValue;
            return true;
        }

        public async Task CargarCategorias()
        {
            try
            {
                var categorias = await _categoriaService.ObtenerCategoriasAsync();
                if (categorias is not null && categorias.Any())
                {
                    Categorias = categorias;
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

        public async Task BuscarCategoria()
        {
            try
            {
                if (BusquedaCategoria == "")
                {
                    await CargarCategorias();
                    return;
                }

                var categorias = await _categoriaService.ObtenerCategoriaPorNombreAsync(BusquedaCategoria);
                if (categorias is not null && categorias.Any())
                {
                    Categorias = categorias;
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

        public async Task CargarLaboratorios()
        {
            try
            {
                var laboratorios = await _laboratorioService.ObtenerLaboratoriosAsync();
                if (laboratorios is not null && laboratorios.Any())
                {
                    Laboratorios = laboratorios;
                }
                else
                {
                    MensajeError = "No se encontraron laboratorios.";
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar laboratorios: {ex.Message}";
            }
        }

        public async Task BuscarLaboratorios()
        {
            try
            {
                if (BusquedaLaboratorio == "")
                {
                    await CargarLaboratorios();
                    return;
                }

                var laboratorios = await _laboratorioService.ObtenerLaboratorioPorNombreAsync(BusquedaLaboratorio);
                if (laboratorios is not null && laboratorios.Any())
                {
                    Laboratorios = laboratorios;
                }
                else
                {
                    MensajeError = "No se encontraron laboratorios.";
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar laboratorios: {ex.Message}";
            }
        }

        private void LimpiarCampos()
        {
            Nombre = string.Empty;
            CategoriaId = 0;
            LaboratorioId = 0;
            ComoUsar = string.Empty;
            EfectosSecundarios = string.Empty;
            Precio = 0;
            Stock = 0;
            FechaVencimiento = DateOnly.FromDateTime(DateTime.Today);
            NombreLaboratorio = string.Empty;
            NombreCategoria = string.Empty;
            StockInput = "0";
        }
    }
}
