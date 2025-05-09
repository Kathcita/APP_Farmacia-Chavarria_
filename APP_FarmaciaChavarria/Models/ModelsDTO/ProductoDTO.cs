using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP_FarmaciaChavarria.Models.ModelsDTO
{
    public class ProductoDTO
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string CategoriaNombre { get; set; }
        public string LaboratorioNombre { get; set; }
        public int id_categoria { get; set; }
        public int id_laboratorio { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int Stock_Minimo { get; set; }
        public string Efectos_Secundarios { get; set; }
        public string Como_Usar { get; set; }
        public DateOnly FechaVencimiento { get; set; }
    }

}
