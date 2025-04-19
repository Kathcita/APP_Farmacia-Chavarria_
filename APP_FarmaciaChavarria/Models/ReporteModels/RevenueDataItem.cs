using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP_FarmaciaChavarria.Models.ReporteModels
{
    public class RevenueDataItem
    {
        public string Date { get; set; } // Ej: "Ene", "Feb", etc.
        public decimal Revenue { get; set; } // Total de ventas
    }
}
