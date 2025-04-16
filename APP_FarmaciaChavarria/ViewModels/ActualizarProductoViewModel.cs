using API_FarmaciaChavarria.Models;
using APP_FarmaciaChavarria.Models.ModelsDTO;
using CommunityToolkit.Mvvm.ComponentModel;
using FarmaciaChavarria.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP_FarmaciaChavarria.ViewModels
{
    public partial class ActualizarProductoViewModel: ObservableObject
    {
        private readonly ProductoService _productoService;
        private readonly CategoriaService _categoriaService;
        private readonly LaboratorioService _laboratorioService;

        public ActualizarProductoViewModel(
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
        private string mensajeError = string.Empty;

        [ObservableProperty]
        private string mensajeExito = string.Empty;

        [ObservableProperty]
        private ProductoDTO? producto;

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

        public async Task CargarProductoPorId(int id)
        {
            var producto = await _productoService.ObtenerProductoPorIdAsync(id);
            Producto = producto;

            if(producto is not null)
            {
                IdProducto = producto.IdProducto;
                Nombre = producto.Nombre;
                CategoriaId = producto.id_categoria;
                LaboratorioId = producto.id_laboratorio;
                Precio = producto.Precio;
                Stock = producto.Stock;
                FechaVencimiento = producto.FechaVencimiento;
                NombreCategoria = producto.CategoriaNombre;
                NombreLaboratorio = producto.LaboratorioNombre;
                StockInput = producto.Stock.ToString();
            }
        }

        public async Task ActualizarProducto()
        {
            try
            {
                if (!Validaciones())
                {
                    return;
                }

                var actualizarProducto = new Producto
                {
                    id_producto = producto.IdProducto,
                    nombre = Nombre,
                    id_categoria = CategoriaId,
                    id_laboratorio = LaboratorioId,
                    fecha_vencimiento = FechaVencimiento,
                    stock = Stock,
                    precio = Precio
                };

                var response = await _productoService.ActualizarProductoAsync(producto.IdProducto, actualizarProducto);

                if (!response.Contains("Error"))
                {
                    MensajeError = "";
                    MensajeExito = "Producto actualizado exitosamente";
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
            NombreCategoria = "- Seleccionar Categoría -";
            NombreLaboratorio = "- Seleccionar Laboratorio -";
            ComoUsar = string.Empty;
            EfectosSecundarios = string.Empty;
            Precio = 0;
            Stock = 0;
            FechaVencimiento = DateOnly.FromDateTime(DateTime.Today);
            IdProducto = 0;
            StockInput = "0";
        }

    }
}
