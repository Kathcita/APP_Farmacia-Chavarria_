using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP_FarmaciaChavarria.Models.ModelsDTO
{
    public class DetalleCompraDTO
    {
        [Key]
        public int id_detalle { get; set; }
        public int id_compra { get; set; }
        public int id_producto { get; set; }
        public int cantidad { get; set; }
        public decimal precio_unitario { get; set; }

        public string nombreProducto { get; set; }
        [NotMapped]
        public decimal subtotal { get { return cantidad * precio_unitario; } }
    }
}
