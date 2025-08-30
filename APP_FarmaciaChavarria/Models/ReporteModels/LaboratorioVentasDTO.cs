using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP_FarmaciaChavarria.Models.ReporteModels
{
    public class LaboratorioVentasDTO
    {
        public int IdLaboratorio { get; set; }
        public string NombreLaboratorio { get; set; }
        public decimal TotalVentas { get; set; }
    }
}
