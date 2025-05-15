using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP_FarmaciaChavarria.ViewModels.Reportes
{
    public class DashboardData
    {
        public decimal VentasDelMes { get; set; }
        public int MedicamentosDisponibles { get; set; }
        public int MedicamentosEscasos { get; set; }

        public int MedicamentosTotales { get; set; }
        public int CategoriasTotales { get; set; }
        public int TotalFacturasDelMes { get; set; }

        public int TotalMedicamentosVendidosDelMes { get; set; }

        public int TotalProveedores { get; set; }
        public int TotalUsuarios { get; set; }
        public string ProductoMasVendido { get; set; }

        public string EstadoInventario { get; set; }
    }
}
