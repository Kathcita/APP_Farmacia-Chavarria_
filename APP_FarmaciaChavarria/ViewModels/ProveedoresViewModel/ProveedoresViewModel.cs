using API_FarmaciaChavarria.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using FarmaciaChavarria.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP_FarmaciaChavarria.ViewModels.ProveedoresViewModel
{
    public partial class ProveedoresViewModel: ObservableObject
    {

        private readonly ProveedorService _proveedorService;
        public ProveedoresViewModel(ProveedorService proveedorService)
        {
            _proveedorService = proveedorService;
        }

        [ObservableProperty]
        private List<Proveedor> proveedor = new();

        [ObservableProperty]
        private string mensajeError = string.Empty;

        [ObservableProperty]
        private string mensajeExito = string.Empty;

        // Atributos de proveedor

        [ObservableProperty]
        private string nombreProveedor = string.Empty;

        [ObservableProperty]
        private string telefono = string.Empty;

        [ObservableProperty]
        private string id = string.Empty;

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
        private bool filtroBusquedaProv = false;

        [ObservableProperty]
        private string busquedaProvedor;


        public async Task CargarProveedores()
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

        public async Task BuscarProveedor(int pagina)
        {
            try
            {

                if (BusquedaProvedor == "")
                {
                    NumeroPaginaprov = 1;
                    await CargarProveedores();
                    return;
                }

                var proveedor = await _proveedorService.BuscarProveedoresPorNombreAsync(BusquedaProvedor,pagina);
                if (proveedor is not null && proveedor.Proveedores.Any())
                {
                    FiltroBusquedaProv = true;
                    Proveedor = proveedor.Proveedores;
                    NumeroPaginaprov = proveedor.CurrentPage;
                    TotalDePaginasProv = proveedor.TotalPages;
                    TotalDeproveedores = proveedor.TotalItems;
                    TamañoDePaginaProv = proveedor.PageSize;
                }
                else
                {
                    MensajeError = "No se encontraron Proveedores.";
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar Proveedores: {ex.Message}";
            }
        }

        public async Task CrearProveedor()
        {
            try
            {
                if (!validaciones()) return;

                var proveedor = new Proveedor
                {
                    nombre = NombreProveedor,
                    telefono = Telefono,
                    direccion = ""
                };

                var response = await _proveedorService.CrearProveedorAsync(proveedor);

                if (!response.Contains("Error"))
                {
                    MensajeExito = "Proveedor creado exitosamente";
                    MensajeError = "";
                    NombreProveedor = "";
                    Telefono = "";
                }
                else
                {
                    MensajeError = response;
                }

            }
            catch (Exception e)
            {
                MensajeError = e.Message;
            }
        }

        public async Task CargarProveedor(int id)
        {
            try
            {
                var response = await _proveedorService.ObtenerProveedorPorIdAsync(id);

                if (response != null)
                {
                    NombreProveedor = response.nombre;
                    Telefono = response.telefono;
                    Id = response.id_proveedor.ToString();
                }
                else
                {
                    MensajeError = "Error: No fue posible cargar la información";
                }
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        public async Task ActualizarProveedor()
        {
            try
            {
                if (!validaciones()) return;

                var proveedor = new Proveedor
                {
                    id_proveedor = int.Parse(Id),
                    nombre = NombreProveedor,
                    telefono = Telefono,
                    direccion = ""
                };

                var response = await _proveedorService.ActualizarProveedorAsync(int.Parse(Id), proveedor);

                if (!response.Contains("Error"))
                {
                    MensajeExito = "Proveedor actualizado exitosamente";
                    MensajeError = "";
                }
                else
                {
                    MensajeError = response;
                }

            }
            catch (Exception e)
            {
                MensajeError = e.Message;
            }
        }

        public bool validaciones()
        {
            if (NombreProveedor == "")
            {
                MensajeError = "El campo nombre de proveedor no puede estar vacío";
                return false;
            }

            if (Telefono == "")
            {
                MensajeError = "El campo teléfono no puede estar vacío";
                return false;
            }

            if (Telefono.Length != 8)
            {
                MensajeError = "Ingrese un número de teléfono válido de 8 dígitos";
                return false;
            }

            if (!Telefono.All(char.IsDigit))
            {
                MensajeError = "El campo teléfono solo debe contener números";
                return false;
            }

            return true;
        }
    }
}
