using API_FarmaciaChavarria.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FarmaciaChavarria.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP_FarmaciaChavarria.ViewModels
{
    public partial class CrearLaboratorioViewModel : ObservableObject
    {
        private readonly LaboratorioService _laboratorioService;

        public CrearLaboratorioViewModel(LaboratorioService laboratorioService)
        {
            _laboratorioService = laboratorioService;
        }

        [ObservableProperty]
        private string nombreLaboratorio = string.Empty;

        [ObservableProperty]
        private int idLaboratorio;

        [ObservableProperty]
        private string mensajeError = string.Empty;

        [ObservableProperty]
        private string mensajeExito = string.Empty;

        [ObservableProperty]
        private bool isLoading = false;

        [ObservableProperty]
        private List<Laboratorio> laboratorios = new();

        [RelayCommand]
        public async Task CrearLaboratorio()
        {
            try
            {
                if (!Validaciones())
                    return;

                IsLoading = true;

                var nuevoLaboratorio = new Laboratorio
                {
                    nombre = NombreLaboratorio
                };

                var respuesta = await _laboratorioService.CrearLaboratorioAsync(nuevoLaboratorio);

                if (!respuesta.Contains("Error"))
                {
                    MensajeExito = "Laboratorio registrado exitosamente.";
                    MensajeError = string.Empty;
                    LimpiarCampos();
                    await CargarLaboratoriosAsync(); // refrescar lista
                }
                else
                {
                    MensajeError = respuesta;
                    MensajeExito = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al crear laboratorio: {ex.Message}";
                MensajeExito = string.Empty;
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task CargarLaboratoriosAsync()
        {
            try
            {
                IsLoading = true;
                MensajeError = string.Empty;

                var resultado = await _laboratorioService.ObtenerLaboratoriosAsync();
                if (resultado != null && resultado.Laboratorios.Any())
                {
                    Laboratorios = resultado.Laboratorios;
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
            finally
            {
                IsLoading = false;
            }
        }

        private bool Validaciones()
        {
            if (string.IsNullOrWhiteSpace(NombreLaboratorio))
            {
                MensajeError = "El nombre del laboratorio no puede estar vacío.";
                return false;
            }

            return true;
        }

        private void LimpiarCampos()
        {
            NombreLaboratorio = string.Empty;
            IdLaboratorio = 0;
        }
        [RelayCommand]
        public async Task CargarLaboratorioPorId(int id)
        {
            try
            {
                IsLoading = true;
                MensajeError = string.Empty;

                var lab = await _laboratorioService.ObtenerLaboratorioPorIdAsync(id);

                if (lab != null)
                {
                    IdLaboratorio = lab.id_laboratorio;
                    NombreLaboratorio = lab.nombre;
                }
                else
                {
                    MensajeError = "No se encontró el laboratorio.";
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar laboratorio: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task ActualizarLaboratorio()
        {
            try
            {
                if (!Validaciones())
                    return;

                IsLoading = true;

                var labActualizado = new Laboratorio
                {
                    id_laboratorio = IdLaboratorio,
                    nombre = NombreLaboratorio
                };

                var respuesta = await _laboratorioService.ActualizarLaboratorioAsync(IdLaboratorio, labActualizado);

                if (!respuesta.Contains("Error"))
                {
                    MensajeExito = "Laboratorio actualizado correctamente.";
                    MensajeError = string.Empty;
                }
                else
                {
                    MensajeError = respuesta;
                    MensajeExito = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al actualizar laboratorio: {ex.Message}";
                MensajeExito = string.Empty;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public LaboratorioService LaboratorioService => _laboratorioService;
    }
}
