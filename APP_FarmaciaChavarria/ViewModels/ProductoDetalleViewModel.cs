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
    public partial class ProductoDetalleViewModel : ObservableObject
    {
        private readonly ProductoService _productoService;

        public ProductoDetalleViewModel(ProductoService productoService)
        {
            _productoService = productoService;
        }

        [ObservableProperty]
        private ProductoDTO? producto;

        public async Task CargarProductoPorId(int id)
        {
            var lista = await _productoService.ObtenerProductoPorIdAsync(id);
            Producto = lista;
        }
    }
}
