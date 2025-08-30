using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP_FarmaciaChavarria.Models.ReporteModels
{
    public class ProductoVentasDTO
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public decimal TotalVentas { get; set; }
    }
}
