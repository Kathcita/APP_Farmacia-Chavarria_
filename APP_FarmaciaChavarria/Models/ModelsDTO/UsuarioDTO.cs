using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP_FarmaciaChavarria.Models.ModelsDTO
{
    public class UsuarioDTO
    {
        public int id_usuario { get; set; }
        public string nombre { get; set; }
        public int pin { get; set; }
        public string rol { get; set; }
    }
}
